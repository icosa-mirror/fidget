// Formula port of Mercury hg_sdf, version 2021-07-28.
// Original: https://mercury.sexy/hg_sdf/hg_sdf.glsl
// Copyright (c) 2011-2021 Mercury Demogroup. See ThirdPartyNotices.md.
using System;
using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>Mercury hg_sdf formulas as Fidget expressions. Angles are radians.</summary>
    internal static partial class HgSdf
    {
        public const float PI = Mathf.PI;
        public const float TAU = 2 * PI;
        public static readonly float PHI = (Mathf.Sqrt(5) + 1) / 2;
        public static Expression3 Point(ShapeBuilder builder) => new Expression3(builder.X, builder.Y, builder.Z);
        private static Expression min(Expression a, Expression b) => a.Min(b);
        private static Expression max(Expression a, Expression b) => a.Max(b);
        private static Expression abs(Expression a) => a.Abs();
        private static Expression sqrt(Expression a) => a.Sqrt();
        private static Expression cos(Expression a) => a.Cos();
        private static Expression sin(Expression a) => a.Sin();
        private static Expression floor(Expression a) => a.Floor();
        // GLSL mod follows the divisor's sign; Fidget modulo is Euclidean.
        private static Expression mod(Expression a, Expression b) => a - b * (a / b).Floor();
        private static Expression atan(Expression y, Expression x) => y.Atan2(x);
        private static Expression pow(Expression a, Expression b) => a.Pow(b);
        private static Expression clamp(Expression a, Expression lo, Expression hi) => min(max(a, lo), hi);
        public static Expression saturate(Expression a) => clamp(a, 0, 1);
        private static Expression step(Expression edge, Expression a) => a.GreaterThanOrEqual(edge);
        private static Expression mix(Expression a, Expression b, Expression t) => a * (1 - t) + b * t;
        public static Expression sgn(Expression a) => a.LessThan(0).Select(-1, 1);
        public static Expression2 sgn(Expression2 a) => new Expression2(sgn(a.x), sgn(a.y));
        public static Expression square(Expression a) => a.Square();
        public static Expression2 square(Expression2 a) => a * a;
        public static Expression3 square(Expression3 a) => a * a;
        private static Expression2 abs(Expression2 a) => new Expression2(abs(a.x), abs(a.y));
        private static Expression3 abs(Expression3 a) => new Expression3(abs(a.x), abs(a.y), abs(a.z));
        private static Expression2 floor(Expression2 a) => new Expression2(floor(a.x), floor(a.y));
        private static Expression3 floor(Expression3 a) => new Expression3(floor(a.x), floor(a.y), floor(a.z));
        private static Expression2 mod(Expression2 a, Expression2 b) => new Expression2(mod(a.x,b.x), mod(a.y,b.y));
        private static Expression3 mod(Expression3 a, Expression3 b) => new Expression3(mod(a.x,b.x), mod(a.y,b.y), mod(a.z,b.z));
        private static Expression2 min(Expression2 a, Expression2 b) => new Expression2(min(a.x,b.x), min(a.y,b.y));
        private static Expression3 min(Expression3 a, Expression3 b) => new Expression3(min(a.x,b.x), min(a.y,b.y), min(a.z,b.z));
        private static Expression2 max(Expression2 a, Expression2 b) => new Expression2(max(a.x,b.x), max(a.y,b.y));
        private static Expression3 max(Expression3 a, Expression3 b) => new Expression3(max(a.x,b.x), max(a.y,b.y), max(a.z,b.z));
        private static Expression dot(Expression2 a, Expression2 b) => a.x*b.x + a.y*b.y;
        private static Expression dot(Expression3 a, Expression3 b) => a.x*b.x + a.y*b.y + a.z*b.z;
        private static Expression length(Expression2 a) => sqrt(dot(a,a));
        private static Expression length(Expression3 a) => sqrt(dot(a,a));
        public static Expression lengthSqr(Expression3 a) => dot(a,a);
        private static Expression2 normalize(Expression2 a) => a / length(a);
        private static Expression3 normalize(Expression3 a) => a / length(a);
        private static Expression2 select(Expression condition, Expression2 a, Expression2 b) => new Expression2(condition.Select(a.x,b.x), condition.Select(a.y,b.y));
        private static Expression3 select(Expression condition, Expression3 a, Expression3 b) => new Expression3(condition.Select(a.x,b.x), condition.Select(a.y,b.y), condition.Select(a.z,b.z));
        public static Expression vmax(Expression2 a) => max(a.x,a.y);
        public static Expression vmax(Expression3 a) => max(max(a.x,a.y),a.z);
        public static Expression vmax(Expression4 a) => max(max(a.x,a.y),max(a.z,a.w));
        public static Expression vmin(Expression2 a) => min(a.x,a.y);
        public static Expression vmin(Expression3 a) => min(min(a.x,a.y),a.z);
        public static Expression vmin(Expression4 a) => min(min(a.x,a.y),min(a.z,a.w));

        public static Expression fSphere(Expression3 p, Expression r) => length(p) - r;
        public static Expression fPlane(Expression3 p, Expression3 n, Expression distanceFromOrigin) => dot(p,n) + distanceFromOrigin;
        public static Expression fBoxCheap(Expression3 p, Expression3 b) => vmax(abs(p)-b);
        public static Expression fBox(Expression3 p, Expression3 b)
        {
            var d = abs(p)-b;
            return length(max(d,new Expression3(0))) + vmax(min(d,new Expression3(0)));
        }
        public static Expression fBox2Cheap(Expression2 p, Expression2 b) => vmax(abs(p)-b);
        public static Expression fBox2(Expression2 p, Expression2 b)
        {
            var d = abs(p)-b;
            return length(max(d,new Expression2(0))) + vmax(min(d,new Expression2(0)));
        }
        public static Expression fCorner(Expression2 p) => length(max(p,new Expression2(0))) + vmax(min(p,new Expression2(0)));
        public static Expression fBlob(Expression3 p)
        {
            p = abs(p);
            p = select(p.x.LessThan(max(p.y,p.z)), p.yzx, p);
            p = select(p.x.LessThan(max(p.y,p.z)), p.yzx, p);
            var b = max(max(max(dot(p,normalize(new Expression3(1,1,1))),
                dot(p.xz,normalize(new Expression2(PHI+1,1)))),
                dot(p.yx,normalize(new Expression2(1,PHI)))),dot(p.xz,normalize(new Expression2(1,PHI))));
            var l = length(p);
            // Preserve the upstream singularity at p = 0.
            return l - 1.5f - 0.2f * (1.5f/2) * cos(min(sqrt(1.01f-b/l)*(PI/0.25f),PI));
        }
        public static Expression fCylinder(Expression3 p, Expression r, Expression height) => max(length(p.xz)-r,abs(p.y)-height);
        public static Expression fCapsule(Expression3 p, Expression r, Expression c) =>
            mix(length(p.xz)-r,length(new Expression3(p.x,abs(p.y)-c,p.z))-r,step(c,abs(p.y)));
        public static Expression fLineSegment(Expression3 p, Expression3 a, Expression3 b)
        {
            var ab = b-a;
            var t = saturate(dot(p-a,ab)/dot(ab,ab));
            return length(ab*t+a-p);
        }
        public static Expression fCapsule(Expression3 p, Expression3 a, Expression3 b, Expression r) => fLineSegment(p,a,b)-r;
        /// <summary>Upstream parameter order: small radius, then large radius.</summary>
        public static Expression fTorus(Expression3 p, Expression smallRadius, Expression largeRadius) => length(new Expression2(length(p.xz)-largeRadius,p.y))-smallRadius;
        /// <summary>Unsigned distance to a circle line in XZ (not the builder's filled XY circle).</summary>
        public static Expression fCircle(Expression3 p, Expression r) => length(new Expression2(p.y,length(p.xz)-r));
        public static Expression fDisc(Expression3 p, Expression r)
        {
            var l = length(p.xz)-r;
            return l.LessThan(0).Select(abs(p.y),length(new Expression2(p.y,l)));
        }
        public static Expression fHexagonCircumcircle(Expression3 p, Expression2 h)
        {
            var q = abs(p);
            return max(q.y-h.y,max(q.x*Mathf.Sqrt(3)*0.5f+q.z*0.5f,q.z)-h.x);
        }
        public static Expression fHexagonIncircle(Expression3 p, Expression2 h) => fHexagonCircumcircle(p,new Expression2(h.x*Mathf.Sqrt(3)*0.5f,h.y));
        /// <summary>Y-up cone with its base at Y = 0 and tip at Y = height.</summary>
        public static Expression fCone(Expression3 p, Expression radius, Expression height)
        {
            var q = new Expression2(length(p.xz),p.y);
            var tip = q-new Expression2(0,height);
            var mantleDir = normalize(new Expression2(height,radius));
            var mantle = dot(tip,mantleDir);
            var d = max(mantle,-q.y);
            var projected = dot(tip,new Expression2(mantleDir.y,-mantleDir.x));
            d = q.y.GreaterThan(height).And(projected.LessThan(0)).Select(max(d,length(tip)),d);
            return q.x.GreaterThan(radius).And(projected.GreaterThan(length(new Expression2(height,radius))))
                .Select(max(d,length(q-new Expression2(radius,0))),d);
        }

        private static readonly Vector3[] GDFVectors = MakeGdfVectors();
        private static Vector3[] MakeGdfVectors()
        {
            var v = new[] {new Vector3(1,0,0),new Vector3(0,1,0),new Vector3(0,0,1),
                new Vector3(1,1,1),new Vector3(-1,1,1),new Vector3(1,-1,1),new Vector3(1,1,-1),
                new Vector3(0,1,PHI+1),new Vector3(0,-1,PHI+1),new Vector3(PHI+1,0,1),new Vector3(-PHI-1,0,1),
                new Vector3(1,PHI+1,0),new Vector3(-1,PHI+1,0),new Vector3(0,PHI,1),new Vector3(0,-PHI,1),
                new Vector3(1,0,PHI),new Vector3(-1,0,PHI),new Vector3(PHI,1,0),new Vector3(-PHI,1,0)};
            for (int i=0;i<v.Length;i++) v[i] /= v[i].magnitude;
            return v;
        }
        private static void GdfRange(int begin, int end)
        {
            if (begin < 0 || end < begin || end >= GDFVectors.Length) throw new ArgumentOutOfRangeException(nameof(begin),"GDF indices must satisfy 0 <= begin <= end < 19.");
        }
        public static Expression fGDF(Expression3 p, Expression r, Expression e, int begin, int end)
        {
            GdfRange(begin,end);
            Expression d = 0;
            for(int i=begin;i<=end;i++) d += pow(abs(dot(p,GDFVectors[i])),e);
            return pow(d,1/e)-r;
        }
        public static Expression fGDF(Expression3 p, Expression r, int begin, int end)
        {
            GdfRange(begin,end);
            Expression d = 0;
            for(int i=begin;i<=end;i++) d = max(d,abs(dot(p,GDFVectors[i])));
            return d-r;
        }
        public static Expression fOctahedron(Expression3 p, Expression r, Expression e) => fGDF(p,r,e,3,6);
        public static Expression fDodecahedron(Expression3 p, Expression r, Expression e) => fGDF(p,r,e,13,18);
        public static Expression fIcosahedron(Expression3 p, Expression r, Expression e) => fGDF(p,r,e,3,12);
        public static Expression fTruncatedOctahedron(Expression3 p, Expression r, Expression e) => fGDF(p,r,e,0,6);
        public static Expression fTruncatedIcosahedron(Expression3 p, Expression r, Expression e) => fGDF(p,r,e,3,18);
        public static Expression fOctahedron(Expression3 p, Expression r) => fGDF(p,r,3,6);
        public static Expression fDodecahedron(Expression3 p, Expression r) => fGDF(p,r,13,18);
        public static Expression fIcosahedron(Expression3 p, Expression r) => fGDF(p,r,3,12);
        public static Expression fTruncatedOctahedron(Expression3 p, Expression r) => fGDF(p,r,0,6);
        public static Expression fTruncatedIcosahedron(Expression3 p, Expression r) => fGDF(p,r,3,18);
    }
}
