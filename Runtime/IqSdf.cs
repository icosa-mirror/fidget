// Distance-function formulas by Inigo Quilez, adapted to Fidget expressions.
// https://iquilezles.org/articles/distfunctions/
// https://iquilezles.org/articles/distfunctions2d/
using System;
using UnityEngine;

namespace Icosa.Fidget
{
    /// <summary>IQ distance functions. Names and parameter conventions follow the source articles.</summary>
    public static partial class IqSdf
    {
        public static Expression3 Point(ShapeBuilder shape) => new Expression3(shape.X,shape.Y,shape.Z);
        private static Expression min(Expression a,Expression b) => a.Min(b);
        private static Expression max(Expression a,Expression b) => a.Max(b);
        private static Expression abs(Expression a) => a.Abs();
        private static Expression sqrt(Expression a) => a.Sqrt();
        private static Expression sin(Expression a) => a.Sin();
        private static Expression cos(Expression a) => a.Cos();
        private static Expression atan(Expression y,Expression x) => y.Atan2(x);
        private static Expression acos(Expression a) => sqrt(1-a*a).Atan2(a);
        private static Expression pow(Expression a,Expression b) => a.Pow(b);
        private static Expression sign(Expression a) => a.Compare(0);
        private static Expression round(Expression a) => sign(a)*(abs(a)+0.5f).Floor();
        private static Expression mod(Expression a,Expression b) => a-b*(a/b).Floor();
        private static Expression clamp(Expression a,Expression lo,Expression hi) => min(max(a,lo),hi);
        private static Expression choose(Expression c,Expression a,Expression b) => c.Select(a,b);
        private static Expression2 choose(Expression c,Expression2 a,Expression2 b) => new Expression2(choose(c,a.x,b.x),choose(c,a.y,b.y));
        private static Expression3 choose(Expression c,Expression3 a,Expression3 b) => new Expression3(choose(c,a.x,b.x),choose(c,a.y,b.y),choose(c,a.z,b.z));
        private static Expression2 abs(Expression2 a) => new Expression2(abs(a.x),abs(a.y));
        private static Expression3 abs(Expression3 a) => new Expression3(abs(a.x),abs(a.y),abs(a.z));
        private static Expression2 min(Expression2 a,Expression2 b) => new Expression2(min(a.x,b.x),min(a.y,b.y));
        private static Expression3 min(Expression3 a,Expression3 b) => new Expression3(min(a.x,b.x),min(a.y,b.y),min(a.z,b.z));
        private static Expression2 max(Expression2 a,Expression2 b) => new Expression2(max(a.x,b.x),max(a.y,b.y));
        private static Expression3 max(Expression3 a,Expression3 b) => new Expression3(max(a.x,b.x),max(a.y,b.y),max(a.z,b.z));
        private static Expression2 max(Expression2 a,Expression b) => max(a,new Expression2(b));
        private static Expression3 max(Expression3 a,Expression b) => max(a,new Expression3(b));
        private static Expression2 clamp(Expression2 a,Expression lo,Expression hi) => new Expression2(clamp(a.x,lo,hi),clamp(a.y,lo,hi));
        private static Expression3 clamp(Expression3 a,Expression lo,Expression hi) => new Expression3(clamp(a.x,lo,hi),clamp(a.y,lo,hi),clamp(a.z,lo,hi));
        private static Expression3 clamp(Expression3 a,Expression3 lo,Expression3 hi) => min(max(a,lo),hi);
        private static Expression3 round(Expression3 a) => new Expression3(round(a.x),round(a.y),round(a.z));
        private static Expression dot(Expression2 a,Expression2 b) => a.x*b.x+a.y*b.y;
        private static Expression dot(Expression3 a,Expression3 b) => a.x*b.x+a.y*b.y+a.z*b.z;
        private static Expression dot2(Expression2 a) => dot(a,a);
        private static Expression dot2(Expression3 a) => dot(a,a);
        private static Expression length(Expression2 a) => sqrt(dot2(a));
        private static Expression length(Expression3 a) => sqrt(dot2(a));
        private static Expression3 cross(Expression3 a,Expression3 b) => new Expression3(a.y*b.z-a.z*b.y,a.z*b.x-a.x*b.z,a.x*b.y-a.y*b.x);

        public static Expression sdSphere(Expression3 p,Expression r) => length(p)-r;
        public static Expression sdBox(Expression3 p,Expression3 b)
        {
            var q=abs(p)-b;
            return boxDistance(q);
        }
        private static Expression boxDistance(Expression3 q) => length(max(q,0))+min(max(q.x,max(q.y,q.z)),0);
        public static Expression sdRoundBox(Expression3 p,Expression3 b,Expression r) => sdBox(p,b-r)-r;
        public static Expression sdBoxFrame(Expression3 p,Expression3 b,Expression e)
        {
            p=abs(p)-b;
            var q=abs(p+e)-e;
            return min(min(boxDistance(new Expression3(p.x,q.y,q.z)),
                           boxDistance(new Expression3(q.x,p.y,q.z))),
                           boxDistance(new Expression3(q.x,q.y,p.z)));
        }
        public static Expression sdTorus(Expression3 p,Expression2 t) => length(new Expression2(length(p.xz)-t.x,p.y))-t.y;
        public static Expression sdCappedTorus(Expression3 p,Expression2 sc,Expression ra,Expression rb)
        {
            p.x=abs(p.x);
            var k=choose((sc.y*p.x).GreaterThan(sc.x*p.y),dot(p.xy,sc),length(p.xy));
            return sqrt(dot(p,p)+ra*ra-2*ra*k)-rb;
        }
        public static Expression sdLink(Expression3 p,Expression le,Expression r1,Expression r2)
        {
            var q=new Expression3(p.x,max(abs(p.y)-le,0),p.z);
            return length(new Expression2(length(q.xy)-r1,q.z))-r2;
        }
        public static Expression sdCylinder(Expression3 p,Expression3 c) => length(p.xz-c.xy)-c.z;
        public static Expression sdCone(Expression3 p,Expression2 c,Expression h)
        {
            var q=h*new Expression2(c.x/c.y,-1);
            var w=new Expression2(length(p.xz),p.y);
            var a=w-q*clamp(dot(w,q)/dot(q,q),0,1);
            var b=w-q*new Expression2(clamp(w.x/q.x,0,1),1);
            var k=sign(q.y);
            var d=min(dot(a,a),dot(b,b));
            var s=max(k*(w.x*q.y-w.y*q.x),k*(w.y-q.y));
            return sqrt(d)*sign(s);
        }
        public static Expression sdCone(Expression3 p,Expression2 c)
        {
            var q=new Expression2(length(p.xz),-p.y);
            var d=length(q-c*max(dot(q,c),0));
            return d*choose((q.x*c.y-q.y*c.x).LessThan(0),-1,1);
        }
        public static Expression sdPlane(Expression3 p,Expression3 n,Expression h) => dot(p,n)+h;
        public static Expression sdHexPrism(Expression3 p,Expression2 h)
        {
            var k=new Expression3(-0.8660254f,0.5f,0.57735f);
            p=abs(p); p.xy-=2*min(dot(k.xy,p.xy),0)*k.xy;
            var d=new Expression2(length(p.xy-new Expression2(clamp(p.x,-k.z*h.x,k.z*h.x),h.x))*sign(p.y-h.x),p.z-h.y);
            return min(max(d.x,d.y),0)+length(max(d,0));
        }
        public static Expression sdCapsule(Expression3 p,Expression3 a,Expression3 b,Expression r)
        {
            var pa=p-a;var ba=b-a;
            var h=clamp(dot(pa,ba)/dot(ba,ba),0,1);
            return length(pa-ba*h)-r;
        }
        public static Expression sdVerticalCapsule(Expression3 p,Expression h,Expression r)
        {
            p.y-=clamp(p.y,0,h);
            return length(p)-r;
        }
        public static Expression sdCappedCylinder(Expression3 p,Expression r,Expression h)
        {
            var d=abs(new Expression2(length(p.xz),p.y))-new Expression2(r,h);
            return min(max(d.x,d.y),0)+length(max(d,0));
        }
        public static Expression sdCappedCylinder(Expression3 p,Expression3 a,Expression3 b,Expression r)
        {
            var ba=b-a;var pa=p-a;var baba=dot(ba,ba);var paba=dot(pa,ba);
            var x=length(pa*baba-ba*paba)-r*baba;
            var y=abs(paba-baba*0.5f)-baba*0.5f;
            var x2=x*x;var y2=y*y*baba;
            var d=choose(max(x,y).LessThan(0),-min(x2,y2),choose(x.GreaterThan(0),x2,0)+choose(y.GreaterThan(0),y2,0));
            return sign(d)*sqrt(abs(d))/baba;
        }
        public static Expression sdRoundedCylinder(Expression3 p,Expression ra,Expression rb,Expression h)
        {
            var d=new Expression2(length(p.xz)-ra+rb,abs(p.y)-h+rb);
            return min(max(d.x,d.y),0)+length(max(d,0))-rb;
        }
        public static Expression sdCappedCone(Expression3 p,Expression h,Expression r1,Expression r2)
        {
            var q=new Expression2(length(p.xz),p.y);
            var k1=new Expression2(r2,h);var k2=new Expression2(r2-r1,2*h);
            var ca=new Expression2(q.x-min(q.x,choose(q.y.LessThan(0),r1,r2)),abs(q.y)-h);
            var cb=q-k1+k2*clamp(dot(k1-q,k2)/dot2(k2),0,1);
            var s=choose(cb.x.LessThan(0).And(ca.y.LessThan(0)),-1,1);
            return s*sqrt(min(dot2(ca),dot2(cb)));
        }
        public static Expression sdCappedCone(Expression3 p,Expression3 a,Expression3 b,Expression ra,Expression rb)
        {
            var rba=rb-ra;var baba=dot(b-a,b-a);var papa=dot(p-a,p-a);var paba=dot(p-a,b-a)/baba;
            var x=sqrt(papa-paba*paba*baba);
            var cax=max(0,x-choose(paba.LessThan(0.5f),ra,rb));var cay=abs(paba-0.5f)-0.5f;
            var k=rba*rba+baba;var f=clamp((rba*(x-ra)+paba*baba)/k,0,1);
            var cbx=x-ra-f*rba;var cby=paba-f;
            var s=choose(cbx.LessThan(0).And(cay.LessThan(0)),-1,1);
            return s*sqrt(min(cax*cax+cay*cay*baba,cbx*cbx+cby*cby*baba));
        }
        public static Expression sdSolidAngle(Expression3 p,Expression2 c,Expression ra)
        {
            var q=new Expression2(length(p.xz),p.y);
            var l=length(q)-ra;var m=length(q-c*clamp(dot(q,c),0,ra));
            return max(l,m*sign(c.y*q.x-c.x*q.y));
        }
        public static Expression sdCutSphere(Expression3 p,Expression r,Expression h)
        {
            var w=sqrt(r*r-h*h);var q=new Expression2(length(p.xz),p.y);
            var s=max((h-r)*q.x*q.x+w*w*(h+r-2*q.y),h*q.x-w*q.y);
            return choose(s.LessThan(0),length(q)-r,choose(q.x.LessThan(w),h-q.y,length(q-new Expression2(w,h))));
        }
        public static Expression sdCutHollowSphere(Expression3 p,Expression r,Expression h,Expression t)
        {
            var w=sqrt(r*r-h*h);var q=new Expression2(length(p.xz),p.y);
            return choose((h*q.x).LessThan(w*q.y),length(q-new Expression2(w,h)),abs(length(q)-r))-t;
        }
        public static Expression sdDeathStar(Expression3 p2,Expression ra,Expression rb,Expression d)
        {
            var a=(ra*ra-rb*rb+d*d)/(2*d);var b=sqrt(max(ra*ra-a*a,0));
            var p=new Expression2(p2.x,length(p2.yz));
            return choose((p.x*b-p.y*a).GreaterThan(d*max(b-p.y,0)),length(p-new Expression2(a,b)),max(length(p)-ra,-(length(p-new Expression2(d,0))-rb)));
        }
        public static Expression sdRoundCone(Expression3 p,Expression r1,Expression r2,Expression h)
        {
            var b=(r1-r2)/h;var a=sqrt(1-b*b);var q=new Expression2(length(p.xz),p.y);
            var k=dot(q,new Expression2(-b,a));
            return choose(k.LessThan(0),length(q)-r1,choose(k.GreaterThan(a*h),length(q-new Expression2(0,h))-r2,dot(q,new Expression2(a,b))-r1));
        }
        public static Expression sdRoundCone(Expression3 p,Expression3 a,Expression3 b,Expression r1,Expression r2)
        {
            var ba=b-a;var l2=dot(ba,ba);var rr=r1-r2;var a2=l2-rr*rr;var il2=1/l2;
            var pa=p-a;var y=dot(pa,ba);var z=y-l2;var x2=dot2(pa*l2-ba*y);var y2=y*y*l2;var z2=z*z*l2;
            var k=sign(rr)*rr*rr*x2;
            return choose((sign(z)*a2*z2).GreaterThan(k),sqrt(x2+z2)*il2-r2,
                   choose((sign(y)*a2*y2).LessThan(k),sqrt(x2+y2)*il2-r1,(sqrt(x2*a2*il2)+y*rr)*il2-r1));
        }
        public static Expression sdVesicaSegment(Expression3 p,Expression3 a,Expression3 b,Expression w)
        {
            var c=(a+b)*0.5f;var l=length(b-a);var v=(b-a)/l;var y=dot(p-c,v);
            var q=new Expression2(length(p-c-y*v),abs(y));var r=0.5f*l;var d=0.5f*(r*r-w*w)/w;
            var h=choose((r*q.x).LessThan(d*(q.y-r)),new Expression3(0,r,0),new Expression3(-d,0,d+w));
            return length(q-h.xy)-h.z;
        }
        public static Expression sdRhombus(Expression3 p,Expression la,Expression lb,Expression h,Expression ra)
        {
            p=abs(p);var f=clamp((la*p.x-lb*p.z+lb*lb)/(la*la+lb*lb),0,1);
            var w=p.xz-new Expression2(la,lb)*new Expression2(f,1-f);
            var q=new Expression2(length(w)*sign(w.x)-ra,p.y-h);
            return min(max(q.x,q.y),0)+length(max(q,0));
        }
        public static Expression sdOctahedron(Expression3 p,Expression s)
        {
            p=abs(p);var m=p.x+p.y+p.z-s;
            var q=choose((3*p.x).LessThan(m),p,choose((3*p.y).LessThan(m),p.yzx,p.zxy));
            var k=clamp(0.5f*(q.z-q.y+s),0,s);
            var outside=length(new Expression3(q.x,q.y-s+k,q.z-k));
            var corner=(3*p.x).LessThan(m).Or((3*p.y).LessThan(m)).Or((3*p.z).LessThan(m));
            return choose(corner,outside,m*0.57735027f);
        }
        public static Expression sdOctahedronBound(Expression3 p,Expression s)
        {
            p=abs(p);return (p.x+p.y+p.z-s)*0.57735027f;
        }
        public static Expression sdPyramid(Expression3 p,Expression h)
        {
            var m2=h*h+0.25f;p.xz=abs(p.xz);p.xz=choose(p.z.GreaterThan(p.x),p.zx,p.xz);p.xz-=new Expression2(0.5f);
            var q=new Expression3(p.z,h*p.y-0.5f*p.x,h*p.x+0.5f*p.y);
            var s=max(-q.x,0);var t=clamp((q.y-0.5f*p.z)/(m2+0.25f),0,1);
            var a=m2*(q.x+s)*(q.x+s)+q.y*q.y;
            var b=m2*(q.x+0.5f*t)*(q.x+0.5f*t)+(q.y-m2*t)*(q.y-m2*t);
            var d2=choose(min(q.y,-q.x*m2-q.y*0.5f).GreaterThan(0),0,min(a,b));
            return sqrt((d2+q.z*q.z)/m2)*sign(max(q.z,-p.y));
        }
        private static Expression edgeDistance2(Expression3 edge,Expression3 delta) => dot2(edge*clamp(dot(edge,delta)/dot2(edge),0,1)-delta);
        public static Expression udTriangle(Expression3 p,Expression3 a,Expression3 b,Expression3 c)
        {
            var ba=b-a;var pa=p-a;var cb=c-b;var pb=p-b;var ac=a-c;var pc=p-c;var nor=cross(ba,ac);
            var sides=sign(dot(cross(ba,nor),pa))+sign(dot(cross(cb,nor),pb))+sign(dot(cross(ac,nor),pc));
            return sqrt(choose(sides.LessThan(2),min(min(edgeDistance2(ba,pa),edgeDistance2(cb,pb)),edgeDistance2(ac,pc)),dot(nor,pa)*dot(nor,pa)/dot2(nor)));
        }
        public static Expression udQuad(Expression3 p,Expression3 a,Expression3 b,Expression3 c,Expression3 d)
        {
            var ba=b-a;var pa=p-a;var cb=c-b;var pb=p-b;var dc=d-c;var pc=p-c;var ad=a-d;var pd=p-d;var nor=cross(ba,ad);
            var sides=sign(dot(cross(ba,nor),pa))+sign(dot(cross(cb,nor),pb))+sign(dot(cross(dc,nor),pc))+sign(dot(cross(ad,nor),pd));
            return sqrt(choose(sides.LessThan(3),min(min(min(edgeDistance2(ba,pa),edgeDistance2(cb,pb)),edgeDistance2(dc,pc)),edgeDistance2(ad,pd)),dot(nor,pa)*dot(nor,pa)/dot2(nor)));
        }
        public static Expression sdEllipsoid(Expression3 p,Expression3 r)
        {
            var k0=length(p/r);var k1=length(p/(r*r));return k0*(k0-1)/k1;
        }
        public static Expression sdTriPrism(Expression3 p,Expression2 h)
        {
            var q=abs(p);return max(q.z-h.y,max(q.x*0.866025f+p.y*0.5f,-p.y)-h.x*0.5f);
        }
    }
}
