# Adaptive sampling for Fidget meshes

Design proposal for the Rust mesher and Unity wrapper.

A model containing a large flat face and a small rounded cut should spend most of its sampling work around the cut. A union of widely spaced parts should retain a specified local detail level as their separation grows, within explicit depth and work limits. Both cases should produce a single mesh, including when a thin bridge connects the parts.

## Implemented sampling controls

Unity supports explicit detail through 11 and opt-in feature spacing in shape-local units.
The longest bounds axis determines the depth needed for that spacing. High-detail,
spacing-based and adaptive builds have a configurable cell budget. An interrupted
request preserves the last completed mesh in the stream.

The first adaptive policy, **Adaptive Planes**, uses the VM's regionally simplified
expression. It accepts a cell early when a unary affine chain, optionally folded
once by absolute value, has opposite signs on the two end faces of its varying axis. It uses
axis-aligned sampling bounds and limits early acceptance to four levels above the
finest depth. Curved regions continue to the selected depth. Rust fixtures check
a drilled block at three grid offsets, disconnected spheres, and a thin bridge.
The Unity Editor integration check exercises a drilled block with surface normals,
depth 11, and recovery after budget exhaustion.

General curved-patch acceptance using a local surface-error tolerance and a
parallel adaptive builder remain open.

## Fixed-depth behavior

Fidget already produces an adaptive output octree. In [octree.rs](fidget-mesh/src/octree.rs), the builder evaluates the field over each cell using interval arithmetic. A strictly positive or negative interval settles that cell immediately. For intervals that include zero, the recursive path descends to the requested depth before evaluating the leaf. The leaf uses corner signs to select edge crossings. A small enclosed surface can therefore disappear when every corner has the same sign. The existing depth-zero sphere test illustrates this case.

On the way back up, the builder checks child topology, merges Hermite data (surface intersections and normals), and solves a quadratic error function (QEF) for a parent vertex. It rejects a collapse when the vertex lies outside the cell or the error reaches twice the stored child error. The [dual contouring traversal](fidget-mesh/src/dc.rs) already consumes this mixed-depth tree. Collapsing cells can save triangles after their children have been sampled. It cannot recover a crossing missed at the finest depth.

The [native wrapper](fidget-native/src/shape.rs) maps the canonical cube onto the local bounds independently along each axis. The finest cell width on each axis is its bounds length divided by two raised to the depth. At Unity detail 6, expanding an axis from 2 to 16 units increases its cell width from 0.03125 to 0.25 units. Moving a second solid farther away can therefore change the mesh of the first solid. Empty-space pruning alone cannot correct this.

## Proposed contract

Keep the current depth mode as the default. Add an opt-in adaptive mode with two controls expressed in **shape-local length units**.

- **Target surface error** controls how far the approximation may deviate at the probes used to accept a coarse patch. This is a sampled estimate. It carries no global Hausdorff-distance guarantee.
- **Feature search spacing** limits the longest cell edge in regions where the available samples leave surface topology unresolved. Smaller values search farther for thin shells, narrow channels or enclosed components. A finite spacing cannot guarantee discovery of every feature of an arbitrary implicit expression.

Keep maximum depth and a visited-cell budget as hard limits. Report achieved sampling statistics and whether any region remained unresolved when a limit was reached. Treat budget exhaustion as an incomplete build. In Unity, retain the previous complete mesh and expose the outcome so callers can change the settings. Publish each completed update atomically. Preserve cancellation and the existing one-request-in-flight behavior.

## Algorithm and implementation stages

1. **Measure the existing path.** Instrument visited cells, interval rejections, finest-depth leaf evaluations, attempted and accepted collapses, output triangles, peak allocation and elapsed worker time. Use the same expression, bounds, parameter snapshot and normal mode in both modes. Count topology vertices before render-time normal splitting. Establish a fixed-depth reference that actually resolves the smallest intended feature.

2. **Prototype discovery in Rust.** Reuse interval pruning. When the interval includes zero and corner signs agree, subdivide until a crossing appears, the interval excludes zero, or a limit is reached. Record ambiguous cells at the search limit even when their corner mask emits no geometry. Use the transformed cell dimensions to decide spacing. Do not use the field magnitude as a physical distance because callers can supply bounds, distorted fields and arbitrary expressions.

3. **Accept easy surface patches early.** At a candidate mixed-sign cell, trial-subdivide one level. Reuse child samples during further descent. Compare the child connectivity and edge crossings with the proposed parent using the existing manifold-collapse checks. Treat a child whose interval still permits a hidden surface as unresolved until it satisfies the discovery policy. For example, a flat patch in one child must not hide a tiny disconnected sphere in another. Matching corners and midpoint probes alone can still miss multiple crossings on one edge. Treat this as a measurable sampling limit.

   For a topology-compatible patch, fit the parent vertex and require it to be finite and inside the cell. Estimate geometric error by comparing child intersection points to the candidate coarse surface in local units. Include face-interior probes to test curvature between edge samples. Start with distances to the reconstructed patch, checking its coverage at cell boundaries. Keep the existing QEF as a placement tool. Its raw residual depends on the coordinate system and number of constraints. Calibrate the independent distance estimate against analytic shapes before using it for early acceptance. Refine when the candidate fails. Retain child data wherever finer neighbors require matching face crossings.

4. **Verify stitching before optimizing.** Reuse the current dual traversal. Early terminal cells must supply the same corner mask, vertex-to-edge mapping and intersection data as a collapsed leaf. Independently accepted neighbors can disagree about a shared face. Cache shared dyadic samples and reconcile face subdivisions and crossing connectivity before mesh emission. If either side requires a finer face, refine the coarse neighbor and recheck affected faces until compatible or a limit is reached. Test deliberately large depth differences. Add balancing only if the traversal or measured transition quality requires it. Land the single-threaded prototype first. The parallel builder preassigns subtrees, so independently enforce compatible decisions across task boundaries and a deterministic global budget before enabling threaded use.

5. **Expose the verified mode.** Add an explicit sampling-policy entry point in Rust alongside the existing settings path. Introduce a versioned native options/result interface with validated finite positive tolerances, bounded depth and overflow-checked allocation sizes. Keep existing callers on their current entry point. Thread the policy and diagnostics through Unity's MeshSettings, MeshStream and typed mesh component. Preserve serialized Detail behavior on existing scenes. Test ABI compatibility and package matching native binaries before publishing an opt-in Unity example. A single connected model with a filleted cut and thin bridge should demonstrate both controls.

## Evaluation and release decision

| Fixture | What to measure |
| --- | --- |
| Flat block with a small fillet and a drilled hole | Surface deviation along the fillet, hole continuity, evaluations on the flat faces and total build time |
| Two separated spheres, then the same pair joined by a thin bridge | Component count, bridge continuity and local radius error as separation increases at fixed tolerances |
| Thin shell, tiny off-center sphere and narrow slot | Inner and outer surfaces, missed components and unresolved-cell counts as spacing and subcell offset vary |
| Sharp CSG intersection crossing coarse/fine boundaries | Manifold edge incidence, duplicate faces, cracks and edge displacement across the transition |
| Elongated bounds and animated cutter | Error on each local axis, frame-to-frame changes in topology, worker latency, cancellation and budget recovery |

Sweep translations through one reference cell and repeat with rotated features. Compare both directions of surface deviation using analytic or independently dense surface samples. Checking only distances from emitted vertices would miss an entirely absent component. For closed fixtures require two incident triangles per undirected edge, expected connected components and fixture-specific hole or shell checks. Test field rescaling with a positive multiplier, which preserves the zero set. Check single- and multi-threaded runs and repeat at smaller tolerances.

First ship criterion: match the resolved fixed-depth reference on fixture topology and measured geometric accuracy across the alignment sweep, and reduce field evaluations and median worker time on the mixed-scale fixtures. Report peak memory, tail latency and any regressions on simple shapes alongside the gains. A smaller triangle count alone cannot demonstrate cheaper sampling. If conservative discovery or face reconciliation consumes the savings, keep the prototype experimental and use the counters to identify the cost before exposing a public quality control.
