// Mercury hg_sdf 2021-07-28. See HgSdf.cs and ThirdPartyNotices.md.
using UnityEngine;

namespace Icosa.Fidget
{
    public static partial class HgSdf
    {
        public static void pR(ref Expression2 p, Expression a) => p = cos(a)*p + sin(a)*new Expression2(p.y,-p.x);
        public static void pR45(ref Expression2 p) => p = (p+new Expression2(p.y,-p.x))*Mathf.Sqrt(0.5f);
        public static Expression pMod1(ref Expression p, Expression size)
        {
            var halfsize = size*0.5f;
            var c = floor((p+halfsize)/size);
            p = mod(p+halfsize,size)-halfsize;
            return c;
        }
        public static Expression pModMirror1(ref Expression p, Expression size)
        {
            var c = pMod1(ref p,size);
            p *= mod(c,2)*2-1;
            return c;
        }
        public static Expression pModSingle1(ref Expression p, Expression size)
        {
            var halfsize = size*0.5f;
            var c = floor((p+halfsize)/size);
            p = p.GreaterThanOrEqual(0).Select(mod(p+halfsize,size)-halfsize,p);
            return c;
        }
        public static Expression pModInterval1(ref Expression p, Expression size, Expression start, Expression stop)
        {
            var c = pMod1(ref p,size);
            p += size * (c-stop).Max(0);
            c = min(c,stop);
            p += size * (c-start).Min(0);
            return max(c,start);
        }
        public static Expression pModPolar(ref Expression2 p, Expression repetitions)
        {
            var angle = TAU/repetitions;
            var a = atan(p.y,p.x)+angle/2;
            var r = length(p);
            var c = floor(a/angle);
            a = mod(a,angle)-angle/2;
            p = new Expression2(cos(a),sin(a))*r;
            return abs(c).GreaterThanOrEqual(repetitions/2).Select(abs(c),c);
        }
        public static Expression2 pMod2(ref Expression2 p, Expression2 size)
        {
            var c = floor((p+size*0.5f)/size);
            p = mod(p+size*0.5f,size)-size*0.5f;
            return c;
        }
        public static Expression2 pModMirror2(ref Expression2 p, Expression2 size)
        {
            var c = pMod2(ref p,size);
            p *= mod(c,new Expression2(2))*2-new Expression2(1);
            return c;
        }
        public static Expression2 pModGrid2(ref Expression2 p, Expression2 size)
        {
            var c = pModMirror2(ref p,size);
            p -= size/2;
            p = select(p.x.GreaterThan(p.y),p.yx,p);
            return floor(c/2);
        }
        public static Expression3 pMod3(ref Expression3 p, Expression3 size)
        {
            var c = floor((p+size*0.5f)/size);
            p = mod(p+size*0.5f,size)-size*0.5f;
            return c;
        }
        public static Expression pMirror(ref Expression p, Expression dist)
        {
            var s = sgn(p);
            p = abs(p)-dist;
            return s;
        }
        public static Expression2 pMirrorOctant(ref Expression2 p, Expression2 dist)
        {
            var s = sgn(p);
            pMirror(ref p.x,dist.x); pMirror(ref p.y,dist.y);
            p = select(p.y.GreaterThan(p.x),p.yx,p);
            return s;
        }
        public static Expression pReflect(ref Expression3 p, Expression3 planeNormal, Expression offset)
        {
            var t = dot(p,planeNormal)+offset;
            p = select(t.LessThan(0),p-(2*t)*planeNormal,p);
            return sgn(t);
        }
    }
}
