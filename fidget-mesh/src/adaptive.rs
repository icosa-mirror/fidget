//! Regional derivative bounds for the experimental single-threaded VM sampler.
use crate::cell::CellBounds;
use fidget_core::{
    compiler::RegOp, shape::ShapeVars, types::Interval, var::Var,
    vm::VmFunction,
};
use nalgebra::Matrix4;

#[derive(Clone, Copy)]
struct Gradient {
    value: Interval,
    d: [Interval; 3],
}

impl Gradient {
    fn constant(k: f32) -> Self {
        Self {
            value: k.into(),
            d: [0.0.into(); 3],
        }
    }
    fn unary(self, value: Interval, derivative: Interval) -> Self {
        Self {
            value,
            d: self.d.map(|d| d * derivative),
        }
    }
    fn add(self, rhs: Self) -> Self {
        Self {
            value: self.value + rhs.value,
            d: std::array::from_fn(|i| self.d[i] + rhs.d[i]),
        }
    }
    fn neg(self) -> Self {
        Self {
            value: -self.value,
            d: self.d.map(|d| -d),
        }
    }
    fn mul(self, rhs: Self) -> Self {
        Self {
            value: self.value * rhs.value,
            d: std::array::from_fn(|i| {
                self.value * rhs.d[i] + rhs.value * self.d[i]
            }),
        }
    }
}

pub(crate) enum Patch {
    Plane,
    Sheet {
        axis: usize,
        max_field_residual: f32,
    },
}

#[derive(Default)]
pub(crate) struct GradientWorkspace {
    inputs: Vec<Gradient>,
    slots: Vec<Gradient>,
}

impl GradientWorkspace {
    /// Evaluate only operations for which both the field and its derivatives
    /// can be bounded throughout the cell. Discontinuities and singularities
    /// fall back to ordinary subdivision.
    pub fn classify(
        &mut self,
        f: &VmFunction,
        cell: CellBounds<3>,
        transform: Option<&Matrix4<f32>>,
        vars: &ShapeVars<f32>,
        tolerance: f32,
    ) -> Option<Patch> {
        let tape = f.data();
        if tolerance == 0.0 {
            // Simplified VM tapes retain the original variable map. Only
            // Input instructions tell us which axes this region still uses.
            let axes = [Var::X, Var::Y, Var::Z].map(|v| tape.vars.get(&v));
            let mut spatial_input = None;
            for op in tape.iter_asm() {
                if let RegOp::Input(_, i) = op
                    && axes.contains(&Some(i as usize))
                {
                    if spatial_input.is_some_and(|previous| previous != i) {
                        return None;
                    }
                    spatial_input = Some(i);
                }
            }
            spatial_input?;
        }
        let region: [Interval; 3] = std::array::from_fn(|axis| {
            let interval = cell.bounds[axis];
            match transform {
                Some(m) => {
                    interval * m[(axis, axis)] + Interval::from(m[(axis, 3)])
                }
                None => interval,
            }
        });
        self.inputs.resize(tape.vars.len(), Gradient::constant(0.0));
        for (var, index) in tape.vars.iter() {
            if let Var::V(id) = var {
                self.inputs[index] = Gradient::constant(*vars.get(id)?);
            } else {
                let axis = match var {
                    Var::X => 0,
                    Var::Y => 1,
                    Var::Z => 2,
                    Var::V(_) => unreachable!(),
                };
                let mut input = Gradient::constant(0.0);
                input.value = region[axis];
                input.d[axis] = 1.0.into();
                self.inputs[index] = input;
            }
        }
        self.slots
            .resize(tape.slot_count(), Gradient::constant(0.0));
        let s = &mut self.slots;
        for op in tape.iter_asm() {
            use RegOp::*;
            let (out, g) = match op {
                Output(a, 0) => return patch(s[a as usize], region, tolerance),
                Input(o, i) => (o, self.inputs[i as usize]),
                CopyImm(o, k) => (o, Gradient::constant(k)),
                CopyReg(o, a) => (o, s[a as usize]),
                NegReg(o, a) => (o, s[a as usize].neg()),
                AddRegImm(o, a, k) => {
                    (o, s[a as usize].add(Gradient::constant(k)))
                }
                SubRegImm(o, a, k) => {
                    (o, s[a as usize].add(Gradient::constant(-k)))
                }
                SubImmReg(o, a, k) => {
                    (o, s[a as usize].neg().add(Gradient::constant(k)))
                }
                AddRegReg(o, a, b) => (o, s[a as usize].add(s[b as usize])),
                SubRegReg(o, a, b) => {
                    (o, s[a as usize].add(s[b as usize].neg()))
                }
                MulRegReg(o, a, b) => (o, s[a as usize].mul(s[b as usize])),
                MulRegImm(o, a, k) => {
                    (o, s[a as usize].unary(s[a as usize].value * k, k.into()))
                }
                SquareReg(o, a) => {
                    let g = s[a as usize];
                    (o, g.unary(g.value.square(), g.value * 2.0))
                }
                AbsReg(o, a) => {
                    let g = s[a as usize];
                    let deriv = if g.value.lower() >= 0.0 {
                        1.0
                    } else if g.value.upper() <= 0.0 {
                        -1.0
                    } else {
                        return None;
                    };
                    (o, g.unary(g.value.abs(), deriv.into()))
                }
                SqrtReg(o, a) => {
                    let g = s[a as usize];
                    if g.value.lower() <= 0.0 {
                        return None;
                    }
                    let value = g.value.sqrt();
                    (o, g.unary(value, Interval::from(0.5) / value))
                }
                _ => return None,
            };
            if !g.value.lower().is_finite() || !g.value.upper().is_finite() {
                return None;
            }
            s[out as usize] = g;
        }
        None
    }
}

fn patch(g: Gradient, region: [Interval; 3], tolerance: f32) -> Option<Patch> {
    if g.d
        .iter()
        .any(|v| !v.lower().is_finite() || !v.upper().is_finite())
    {
        return None;
    }
    let mut best = None;
    let mut strongest = 0.0;
    for axis in 0..3 {
        if g.d[axis].contains(0.0) {
            continue;
        }
        let other = [(axis + 1) % 3, (axis + 2) % 3];
        if other.iter().all(|&b| g.d[b] == Interval::from(0.0)) {
            return Some(Patch::Plane);
        }
        // For a sheet expressed as height along this axis, the two slopes
        // are -df/db / df/da. Their interval widths bound deviation from
        // a plane across this footprint in shape-local length units.
        let variation: f32 = other
            .into_iter()
            .map(|b| (g.d[b] / g.d[axis]).width() * region[b].width())
            .sum();
        // Anchor a plane at the footprint center using the midpoint of
        // each slope interval. Across half an edge its deviation is at
        // most one quarter of width(slope) * width(footprint). Comparing
        // the surface and interpolated points costs twice this bound.
        // Reserve the remaining quarter of the target for vertex residuals.
        if tolerance > 0.0 && variation <= 1.5 * tolerance {
            let min_derivative =
                g.d[axis].lower().abs().min(g.d[axis].upper().abs());
            if min_derivative > strongest {
                strongest = min_derivative;
                best = Some(Patch::Sheet {
                    axis,
                    max_field_residual: min_derivative * tolerance * 0.25,
                });
            }
        }
    }
    best
}
