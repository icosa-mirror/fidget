// Mercury hg_sdf 2021-07-28. See HgSdf.cs and ThirdPartyNotices.md.
using UnityEngine;

namespace Icosa.Fidget
{
    internal static partial class HgSdf
    {
        public static Expression fOpUnionChamfer(Expression a, Expression b, Expression r) => min(min(a,b),(a-r+b)*Mathf.Sqrt(0.5f));
        public static Expression fOpIntersectionChamfer(Expression a, Expression b, Expression r) => max(max(a,b),(a+r+b)*Mathf.Sqrt(0.5f));
        public static Expression fOpDifferenceChamfer(Expression a, Expression b, Expression r) => fOpIntersectionChamfer(a,-b,r);
        public static Expression fOpUnionRound(Expression a, Expression b, Expression r)
        {
            var u = max(new Expression2(r-a,r-b),new Expression2(0));
            return max(r,min(a,b))-length(u);
        }
        public static Expression fOpIntersectionRound(Expression a, Expression b, Expression r)
        {
            var u = max(new Expression2(r+a,r+b),new Expression2(0));
            return min(-r,max(a,b))+length(u);
        }
        public static Expression fOpDifferenceRound(Expression a, Expression b, Expression r) => fOpIntersectionRound(a,-b,r);
        public static Expression fOpUnionColumns(Expression a, Expression b, Expression r, Expression n)
        {
            var p = new Expression2(a,b);
            var columnradius = r*Mathf.Sqrt(2)/((n-1)*2+Mathf.Sqrt(2));
            pR45(ref p);
            p.x -= Mathf.Sqrt(2)/2*r;
            p.x += columnradius*Mathf.Sqrt(2);
            p.y += mod(n,2).EqualTo(1).Select(columnradius,0);
            pMod1(ref p.y,columnradius*2);
            var result = min(min(min(length(p)-columnradius,p.x),a),b);
            return a.LessThan(r).And(b.LessThan(r)).Select(result,min(a,b));
        }
        public static Expression fOpDifferenceColumns(Expression a, Expression b, Expression r, Expression n)
        {
            a = -a;
            var m = min(a,b);
            var p = new Expression2(a,b);
            var columnradius = r*Mathf.Sqrt(2)/((n-1)*2+Mathf.Sqrt(2));
            pR45(ref p);
            p.y += columnradius;
            p.x -= Mathf.Sqrt(2)/2*r;
            p.x += -columnradius*Mathf.Sqrt(2)/2;
            p.y += mod(n,2).EqualTo(1).Select(columnradius,0);
            pMod1(ref p.y,columnradius*2);
            var result = -min(min(max(-length(p)+columnradius,p.x),a),b);
            return a.LessThan(r).And(b.LessThan(r)).Select(result,-m);
        }
        public static Expression fOpIntersectionColumns(Expression a, Expression b, Expression r, Expression n) => fOpDifferenceColumns(a,-b,r,n);
        public static Expression fOpUnionStairs(Expression a, Expression b, Expression r, Expression n)
        {
            var s = r/n;
            var u = b-r;
            return min(min(a,b),0.5f*(u+a+abs(mod(u-a+s,2*s)-s)));
        }
        public static Expression fOpIntersectionStairs(Expression a, Expression b, Expression r, Expression n) => -fOpUnionStairs(-a,-b,r,n);
        public static Expression fOpDifferenceStairs(Expression a, Expression b, Expression r, Expression n) => -fOpUnionStairs(-a,b,r,n);
        public static Expression fOpUnionSoft(Expression a, Expression b, Expression r)
        {
            var e = max(r-abs(a-b),0);
            return min(a,b)-e*e*0.25f/r;
        }
        public static Expression fOpPipe(Expression a, Expression b, Expression r) => length(new Expression2(a,b))-r;
        public static Expression fOpEngrave(Expression a, Expression b, Expression r) => max(a,(a+r-abs(b))*Mathf.Sqrt(0.5f));
        public static Expression fOpGroove(Expression a, Expression b, Expression ra, Expression rb) => max(a,min(a+ra,rb-abs(b)));
        public static Expression fOpTongue(Expression a, Expression b, Expression ra, Expression rb) => min(a,max(a-ra,abs(b)-rb));
    }
}
