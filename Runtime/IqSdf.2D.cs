// Formulas by Inigo Quilez: https://iquilezles.org/articles/distfunctions2d/
using System;
using UnityEngine;

namespace Icosa.Fidget
{
    public static partial class IqSdf
    {
        public static Expression sdCircle(Expression2 p,Expression r) => length(p)-r;
        public static Expression sdRoundedBox(Expression2 p,Expression2 b,Expression4 r)
        {
            r.xy=choose(p.x.GreaterThan(0),r.xy,r.zw);
            r.x=choose(p.y.GreaterThan(0),r.x,r.y);
            var q=abs(p)-b+r.x;
            return min(max(q.x,q.y),0)+length(max(q,0))-r.x;
        }
        public static Expression sdChamferBox(Expression2 p,Expression2 b,Expression chamfer)
        {
            p=abs(p)-b;p=choose(p.y.GreaterThan(p.x),p.yx,p);p.y+=chamfer;
            var k=1-Mathf.Sqrt(2);
            return choose(p.y.LessThan(0).And((p.y+p.x*k).LessThan(0)),p.x,
                   choose(p.x.LessThan(p.y),(p.x+p.y)*Mathf.Sqrt(0.5f),length(p)));
        }
        public static Expression sdBox(Expression2 p,Expression2 b)
        {
            var d=abs(p)-b;return length(max(d,0))+min(max(d.x,d.y),0);
        }
        public static Expression sdOrientedBox(Expression2 p,Expression2 a,Expression2 b,Expression th)
        {
            var l=length(b-a);var d=(b-a)/l;var q=p-(a+b)*0.5f;
            q=new Expression2(d.x*q.x+d.y*q.y,-d.y*q.x+d.x*q.y);
            q=abs(q)-new Expression2(l,th)*0.5f;
            return length(max(q,0))+min(max(q.x,q.y),0);
        }
        public static Expression sdSegment(Expression2 p,Expression2 a,Expression2 b)
        {
            var pa=p-a;var ba=b-a;var h=clamp(dot(pa,ba)/dot(ba,ba),0,1);
            return length(pa-ba*h);
        }
        public static Expression sdRhombus(Expression2 p,Expression2 b)
        {
            b.y=-b.y;p=abs(p);var h=clamp((dot(b,p)+b.y*b.y)/dot(b,b),0,1);
            p-=b*new Expression2(h,h-1);return length(p)*sign(p.x);
        }
        public static Expression sdTrapezoid(Expression2 p,Expression r1,Expression r2,Expression he)
        {
            var k1=new Expression2(r2,he);var k2=new Expression2(r2-r1,2*he);p.x=abs(p.x);
            var ca=new Expression2(p.x-min(p.x,choose(p.y.LessThan(0),r1,r2)),abs(p.y)-he);
            var cb=p-k1+k2*clamp(dot(k1-p,k2)/dot2(k2),0,1);
            return choose(cb.x.LessThan(0).And(ca.y.LessThan(0)),-1,1)*sqrt(min(dot2(ca),dot2(cb)));
        }
        public static Expression sdParallelogram(Expression2 p,Expression wi,Expression he,Expression sk)
        {
            var e=new Expression2(sk,he);p=choose(p.y.LessThan(0),-p,p);
            var w=p-e;w.x-=clamp(w.x,-wi,wi);var d=new Expression2(dot(w,w),-w.y);
            var s=p.x*e.y-p.y*e.x;p=choose(s.LessThan(0),-p,p);
            var v=p-new Expression2(wi,0);v-=e*clamp(dot(v,e)/dot(e,e),-1,1);
            d=min(d,new Expression2(dot(v,v),wi*he-abs(s)));
            return sqrt(d.x)*sign(-d.y);
        }
        public static Expression sdEquilateralTriangle(Expression2 p,Expression r)
        {
            var k=Mathf.Sqrt(3);p.x=abs(p.x)-r;p.y+=r/k;
            p=choose((p.x+k*p.y).GreaterThan(0),new Expression2(p.x-k*p.y,-k*p.x-p.y)/2,p);
            p.x-=clamp(p.x,-2*r,0);return -length(p)*sign(p.y);
        }
        public static Expression sdTriangleIsosceles(Expression2 p,Expression2 q)
        {
            p.x=abs(p.x);var a=p-q*clamp(dot(p,q)/dot(q,q),0,1);
            var b=p-q*new Expression2(clamp(p.x/q.x,0,1),1);var s=-sign(q.y);
            var d=min(new Expression2(dot(a,a),s*(p.x*q.y-p.y*q.x)),new Expression2(dot(b,b),s*(p.y-q.y)));
            return -sqrt(d.x)*sign(d.y);
        }
        public static Expression sdTriangle(Expression2 p,Expression2 p0,Expression2 p1,Expression2 p2)
        {
            var e0=p1-p0;var e1=p2-p1;var e2=p0-p2;var v0=p-p0;var v1=p-p1;var v2=p-p2;
            var pq0=v0-e0*clamp(dot(v0,e0)/dot(e0,e0),0,1);
            var pq1=v1-e1*clamp(dot(v1,e1)/dot(e1,e1),0,1);
            var pq2=v2-e2*clamp(dot(v2,e2)/dot(e2,e2),0,1);
            var s=sign(e0.x*e2.y-e0.y*e2.x);
            var d=min(min(new Expression2(dot(pq0,pq0),s*(v0.x*e0.y-v0.y*e0.x)),
                          new Expression2(dot(pq1,pq1),s*(v1.x*e1.y-v1.y*e1.x))),
                          new Expression2(dot(pq2,pq2),s*(v2.x*e2.y-v2.y*e2.x)));
            return -sqrt(d.x)*sign(d.y);
        }
        public static Expression sdUnevenCapsule(Expression2 p,Expression r1,Expression r2,Expression h)
        {
            p.x=abs(p.x);var b=(r1-r2)/h;var a=sqrt(1-b*b);var k=dot(p,new Expression2(-b,a));
            return choose(k.LessThan(0),length(p)-r1,choose(k.GreaterThan(a*h),length(p-new Expression2(0,h))-r2,dot(p,new Expression2(a,b))-r1));
        }
        public static Expression sdPentagon(Expression2 p,Expression r)
        {
            var k=new Expression3(0.809016994f,0.587785252f,0.726542528f);p.x=abs(p.x);
            var a=new Expression2(-k.x,k.y);p-=2*min(dot(a,p),0)*a;
            a=new Expression2(k.x,k.y);p-=2*min(dot(a,p),0)*a;
            p-=new Expression2(clamp(p.x,-r*k.z,r*k.z),r);return length(p)*sign(p.y);
        }
        public static Expression sdHexagon(Expression2 p,Expression r)
        {
            var k=new Expression3(-0.866025404f,0.5f,0.577350269f);p=abs(p);
            p-=2*min(dot(k.xy,p),0)*k.xy;
            p-=new Expression2(clamp(p.x,-k.z*r,k.z*r),r);return length(p)*sign(p.y);
        }
        public static Expression sdOctogon(Expression2 p,Expression r)
        {
            var k=new Expression3(-0.9238795325f,0.3826834323f,0.4142135623f);p=abs(p);
            var a=new Expression2(k.x,k.y);p-=2*min(dot(a,p),0)*a;
            a=new Expression2(-k.x,k.y);p-=2*min(dot(a,p),0)*a;
            p-=new Expression2(clamp(p.x,-k.z*r,k.z*r),r);return length(p)*sign(p.y);
        }
        public static Expression sdHexagram(Expression2 p,Expression r)
        {
            var k=new Expression4(-0.5f,0.8660254038f,0.5773502692f,1.7320508076f);p=abs(p);
            p-=2*min(dot(k.xy,p),0)*k.xy;p-=2*min(dot(k.yx,p),0)*k.yx;
            p-=new Expression2(clamp(p.x,r*k.z,r*k.w),r);return length(p)*sign(p.y);
        }
        public static Expression sdPentagram(Expression2 p,Expression r)
        {
            var v1=new Expression2(0.809016994f,-0.587785252f);var v2=new Expression2(-0.809016994f,-0.587785252f);
            var v3=new Expression2(0.309016994f,-0.951056516f);
            p.x=abs(p.x);p-=2*max(dot(v1,p),0)*v1;p-=2*max(dot(v2,p),0)*v2;p.x=abs(p.x);p.y-=r;
            return length(p-v3*clamp(dot(p,v3),0,0.726542528f*r))*sign(p.y*v3.x-p.x*v3.y);
        }
        public static Expression sdStar(Expression2 p,Expression r,int n,Expression m)
        {
            if(n<3) throw new ArgumentOutOfRangeException(nameof(n));
            float an=3.141593f/n;var en=3.141593f/m;
            var acs=new Expression2(Mathf.Cos(an),Mathf.Sin(an));var ecs=new Expression2(cos(en),sin(en));
            var bn=mod(atan(p.x,p.y),2*an)-an;
            p=length(p)*new Expression2(cos(bn),abs(sin(bn)));p-=r*acs;
            p+=ecs*clamp(-dot(p,ecs),0,r*acs.y/ecs.y);return length(p)*sign(p.x);
        }
        public static Expression sdPie(Expression2 p,Expression2 c,Expression r)
        {
            p.x=abs(p.x);var l=length(p)-r;var m=length(p-c*clamp(dot(p,c),0,r));
            return max(l,m*sign(c.y*p.x-c.x*p.y));
        }
        public static Expression sdCutDisk(Expression2 p,Expression r,Expression h)
        {
            var w=sqrt(r*r-h*h);p.x=abs(p.x);
            var s=max((h-r)*p.x*p.x+w*w*(h+r-2*p.y),h*p.x-w*p.y);
            return choose(s.LessThan(0),length(p)-r,choose(p.x.LessThan(w),h-p.y,length(p-new Expression2(w,h))));
        }
        public static Expression sdArc(Expression2 p,Expression2 sc,Expression ra,Expression rb)
        {
            p.x=abs(p.x);return choose((sc.y*p.x).GreaterThan(sc.x*p.y),length(p-sc*ra),abs(length(p)-ra))-rb;
        }
        public static Expression sdRing(Expression2 p,Expression2 n,Expression r,Expression th)
        {
            p.x=abs(p.x);p=new Expression2(n.x*p.x-n.y*p.y,n.y*p.x+n.x*p.y);
            return max(abs(length(p)-r)-th*0.5f,length(new Expression2(p.x,max(0,abs(r-p.y)-th*0.5f)))*sign(p.x));
        }
        public static Expression sdHorseshoe(Expression2 p,Expression2 c,Expression r,Expression2 w)
        {
            p.x=abs(p.x);var l=length(p);p=new Expression2(-c.x*p.x+c.y*p.y,c.y*p.x+c.x*p.y);
            p=new Expression2(choose(p.y.GreaterThan(0).Or(p.x.GreaterThan(0)),p.x,l*sign(-c.x)),choose(p.x.GreaterThan(0),p.y,l));
            p=new Expression2(p.x,abs(p.y-r))-w;return length(max(p,0))+min(0,max(p.x,p.y));
        }
        public static Expression sdVesica(Expression2 p,Expression w,Expression h)
        {
            // The article's `vec3 d` is a scalar declaration typo.
            var d=0.5f*(w*w-h*h)/h;p=abs(p);
            var c=choose((w*p.y).LessThan(d*(p.x-w)),new Expression3(0,w,0),new Expression3(-d,0,d+h));
            return length(p-c.yx)-c.z;
        }
        public static Expression sdOrientedVesica(Expression2 p,Expression2 a,Expression2 b,Expression w)
        {
            var r=0.5f*length(b-a);var d=0.5f*(r*r-w*w)/w;var v=(b-a)/r;var c=(b+a)*0.5f;
            var q=p-c;q=0.5f*abs(new Expression2(v.y*q.x-v.x*q.y,v.x*q.x+v.y*q.y));
            var h=choose((r*q.x).LessThan(d*(q.y-r)),new Expression3(0,r,0),new Expression3(-d,0,d+w));
            return length(q-h.xy)-h.z;
        }
        public static Expression sdMoon(Expression2 p,Expression d,Expression ra,Expression rb)
        {
            p.y=abs(p.y);var a=(ra*ra-rb*rb+d*d)/(2*d);var b=sqrt(max(ra*ra-a*a,0));
            return choose((d*(p.x*b-p.y*a)).GreaterThan(d*d*max(b-p.y,0)),length(p-new Expression2(a,b)),max(length(p)-ra,-(length(p-new Expression2(d,0))-rb)));
        }
        public static Expression sdRoundedCross(Expression2 p,Expression h)
        {
            var k=0.5f*(h+1/h);p=abs(p);
            return choose(p.x.LessThan(1).And(p.y.LessThan(p.x*(k-h)+h)),k-sqrt(dot2(p-new Expression2(1,k))),sqrt(min(dot2(p-new Expression2(0,h)),dot2(p-new Expression2(1,0)))));
        }
        public static Expression sdEgg(Expression2 p,Expression he,Expression ra,Expression rb,Expression bu)
        {
            var r=0.5f*(he+ra+rb)/bu;var da=r-ra;var db=r-rb;
            var y=(db*db-da*da-he*he)/(2*he);var x=sqrt(da*da-y*y);
            p.x=abs(p.x);var k=p.y*x-p.x*y;
            return choose(k.GreaterThan(0).And(k.LessThan(he*(p.x+x))),length(p+new Expression2(x,y))-r,min(length(p)-ra,length(new Expression2(p.x,p.y-he))-rb));
        }
        public static Expression sdHeart(Expression2 p)
        {
            p.x=abs(p.x);
            return choose((p.y+p.x).GreaterThan(1),sqrt(dot2(p-new Expression2(0.25f,0.75f)))-Mathf.Sqrt(2)/4,
                sqrt(min(dot2(p-new Expression2(0,1)),dot2(p-0.5f*max(p.x+p.y,0))))*sign(p.x-p.y));
        }
        public static Expression sdCross(Expression2 p,Expression2 b,Expression r)
        {
            p=abs(p);p=choose(p.y.GreaterThan(p.x),p.yx,p);var q=p-b;var k=max(q.y,q.x);
            var w=choose(k.GreaterThan(0),q,new Expression2(b.y-p.x,-k));return sign(k)*length(max(w,0))+r;
        }
        public static Expression sdRoundedX(Expression2 p,Expression w,Expression r)
        {
            p=abs(p);return length(p-min(p.x+p.y,w)*0.5f)-r;
        }
        public static Expression sdPolygon(Expression2[] v,Expression2 p)
        {
            if(v==null || v.Length<3) throw new ArgumentException("A polygon needs at least three vertices.",nameof(v));
            Expression d=dot(p-v[0],p-v[0]),s=1;
            for(int i=0,j=v.Length-1;i<v.Length;j=i,i++)
            {
                var e=v[j]-v[i];var w=p-v[i];var b=w-e*clamp(dot(w,e)/dot(e,e),0,1);d=min(d,dot(b,b));
                var c0=p.y.GreaterThanOrEqual(v[i].y);var c1=p.y.LessThan(v[j].y);var c2=(e.x*w.y).GreaterThan(e.y*w.x);
                var crossing=c0.And(c1).And(c2).Or(c0.Not().And(c1.Not()).And(c2.Not()));
                s*=choose(crossing,-1,1);
            }
            return s*sqrt(d);
        }
        public static Expression sdEllipse(Expression2 p,Expression2 ab)
        {
            p=abs(p);var swap=p.x.GreaterThan(p.y);p=choose(swap,p.yx,p);ab=choose(swap,ab.yx,ab);
            var circular=ab.x.EqualTo(ab.y);var l=choose(circular,1,ab.y*ab.y-ab.x*ab.x);
            var m=ab.x*p.x/l;var m2=m*m;var n=ab.y*p.y/l;var n2=n*n;
            var c=(m2+n2-1)/3;var c3=c*c*c;var q=c3+m2*n2*2;var d=c3+m2*n2;var g=m+m*n2;
            var h0=acos(q/c3)/3;var s0=cos(h0);var t0=sin(h0)*Mathf.Sqrt(3);
            var rx0=sqrt(-c*(s0+t0+2)+m2);var ry0=sqrt(-c*(s0-t0+2)+m2);
            var co0=(ry0+sign(l)*rx0+abs(g)/(rx0*ry0)-m)/2;
            var h1=2*m*n*sqrt(d);var s1=sign(q+h1)*pow(abs(q+h1),1f/3);var u=sign(q-h1)*pow(abs(q-h1),1f/3);
            var rx1=-s1-u-c*4+2*m2;var ry1=(s1-u)*Mathf.Sqrt(3);var rm=sqrt(rx1*rx1+ry1*ry1);
            var co1=(ry1/sqrt(rm-rx1)+2*g/rm-m)/2;
            var co=choose(d.LessThan(0),co0,co1);var r=ab*new Expression2(co,sqrt(1-co*co));
            var center=p.x.EqualTo(0).And(p.y.EqualTo(0));
            return choose(center,-min(ab.x,ab.y),choose(circular,length(p)-ab.x,length(r-p)*sign(p.y-r.y)));
        }
        private static Expression parabolaRoot(Expression p,Expression q)
        {
            var h=q*q-p*p*p;var r0=pow(q+sqrt(h),1f/3);var x0=r0+p/r0;
            var r1=sqrt(p);var x1=2*r1*cos(acos(q/(p*r1))/3);
            return choose(q.EqualTo(0),sqrt(max(3*p,0)),choose(h.GreaterThan(0),x0,x1));
        }
        public static Expression sdParabola(Expression2 pos,Expression k)
        {
            pos.x=abs(pos.x);var ik=1/k;var p=ik*(pos.y-0.5f*ik)/3;var q=0.25f*ik*ik*pos.x;
            var x=parabolaRoot(p,q);return length(pos-new Expression2(x,k*x*x))*sign(pos.x-x);
        }
        public static Expression sdParabola(Expression2 pos,Expression wi,Expression he)
        {
            pos.x=abs(pos.x);var ik=wi*wi/he;var p=ik*(he-pos.y-0.5f*ik)/3;var q=pos.x*ik*ik/4;
            var x=min(parabolaRoot(p,q),wi);return length(pos-new Expression2(x,he-x*x/ik))*sign(ik*(pos.y-he)+pos.x*pos.x);
        }
        public static Expression sdBezier(Expression2 pos,Expression2 A,Expression2 B,Expression2 C)
        {
            var a=B-A;var b=A-2*B+C;var c=a*2;var d=A-pos;var linear=dot(b,b).EqualTo(0);var kk=1/choose(linear,1,dot(b,b));
            var kx=kk*dot(a,b);var ky=kk*(2*dot(a,a)+dot(d,b))/3;var kz=kk*dot(d,a);
            var p=ky-kx*kx;var p3=p*p*p;var q=kx*(2*kx*kx-3*ky)+kz;var h=q*q+4*p3;
            var root=sqrt(h);var x=new Expression2(root,-root)-q;
            x/=2;var uv=new Expression2(sign(x.x)*pow(abs(x.x),1f/3),sign(x.y)*pow(abs(x.y),1f/3));
            var t=clamp(uv.x+uv.y-kx,0,1);var res0=dot2(d+(c+b*t)*t);
            var z=sqrt(-p);var v=acos(q/(p*z*2))/3;var m=cos(v);var n=sin(v)*1.732050808f;
            var ts=clamp(new Expression3(m+m,-n-m,n-m)*z-kx,0,1);
            var res1=min(dot2(d+(c+b*ts.x)*ts.x),dot2(d+(c+b*ts.y)*ts.y));
            return choose(linear,sdSegment(pos,A,C),sqrt(choose(h.GreaterThanOrEqual(0),res0,res1)));
        }
        public static Expression sdBlobbyCross(Expression2 pos,Expression he)
        {
            pos=abs(pos);pos=new Expression2(abs(pos.x-pos.y),1-pos.x-pos.y)/Mathf.Sqrt(2);
            var p=(he-pos.y-0.25f/he)/(6*he);var q=pos.x/(he*he*16);var h=q*q-p*p*p;
            var r0=sqrt(h);var x0=pow(q+r0,1f/3)-pow(abs(q-r0),1f/3)*sign(r0-q);
            var r1=sqrt(p);var x1=2*r1*cos(acos(q/(p*r1))/3);
            var x=min(choose(h.GreaterThan(0),x0,x1),Mathf.Sqrt(2)/2);
            var z=new Expression2(x,he*(1-2*x*x))-pos;return length(z)*sign(z.y);
        }
        public static Expression sdTunnel(Expression2 p,Expression2 wh)
        {
            p.x=abs(p.x);p.y=-p.y;var q=p-wh;var d1=dot2(new Expression2(max(q.x,0),q.y));
            q.x=choose(p.y.GreaterThan(0),q.x,length(p)-wh.x);var d2=dot2(new Expression2(q.x,max(q.y,0)));
            var d=sqrt(min(d1,d2));return choose(max(q.x,q.y).LessThan(0),-d,d);
        }
        public static Expression sdStairs(Expression2 p,Expression2 wh,Expression n)
        {
            var ba=wh*n;var d=min(dot2(p-new Expression2(clamp(p.x,0,ba.x),0)),dot2(p-new Expression2(ba.x,clamp(p.y,0,ba.y))));
            var s=sign(max(-p.y,p.x-ba.x));var dia=length(wh);
            p=new Expression2(wh.x*p.x+wh.y*p.y,-wh.y*p.x+wh.x*p.y)/dia;
            var id=clamp(round(p.x/dia),0,n-1);p.x-=id*dia;
            p=new Expression2(wh.x*p.x-wh.y*p.y,wh.y*p.x+wh.x*p.y)/dia;
            var hh=wh.y/2;p.y-=hh;s=choose(p.y.GreaterThan(hh*sign(p.x)),1,s);
            p=choose(id.LessThan(0.5f).Or(p.x.GreaterThan(0)),p,-p);
            d=min(d,dot2(p-new Expression2(0,clamp(p.y,-hh,hh))));
            d=min(d,dot2(p-new Expression2(clamp(p.x,0,wh.x),hh)));return sqrt(d)*s;
        }
        public static Expression sdQuadraticCircle(Expression2 p)
        {
            p=abs(p);p=choose(p.y.GreaterThan(p.x),p.yx,p);var a=p.x-p.y;var b=p.x+p.y;var c=(2*b-1)/3;var h=a*a+c*c*c;
            var root=sqrt(h);var t0=sign(root-a)*pow(abs(root-a),1f/3)-pow(root+a,1f/3);
            var z=sqrt(-c);var v=acos(a/(c*z))/3;var t1=-z*(cos(v)+sin(v)*1.732050808f);
            var t=choose(h.GreaterThanOrEqual(0),t0,t1)*0.5f;var w=new Expression2(-t,t)+0.75f-t*t-p;
            return length(w)*sign(a*a*0.5f+b-1.5f);
        }
        public static Expression sdHyberbola(Expression2 p,Expression k,Expression he)
        {
            p=abs(p);p=new Expression2(p.x-p.y,p.x+p.y)/Mathf.Sqrt(2);
            var x2=p.x*p.x/16;var y2=p.y*p.y/16;var r=k*(4*k-p.x*p.y)/12;var q=(x2-y2)*k*k;var h=q*q+r*r*r;
            var m0=sqrt(-r);var u0=m0*cos(acos(q/(r*m0))/3);
            var m1=pow(sqrt(h)-q,1f/3);var u1=(m1-r/m1)/2;var u=choose(h.LessThan(0),u0,u1);
            var w=sqrt(u+x2);var b=k*p.y-x2*p.x*2;
            var t=p.x/4-w+sqrt(2*x2-u+b/w/4);t=max(t,sqrt(he*he*0.5f+k)-he/Mathf.Sqrt(2));
            var d=length(p-new Expression2(t,k/t));return choose((p.x*p.y).LessThan(k),d,-d);
        }
        public static Expression sdfCoolS(Expression2 p)
        {
            var six=choose(p.y.LessThan(0),-p.x,p.x);p.x=abs(p.x);p.y=abs(p.y)-0.2f;
            var rex=p.x-min(round(p.x/0.4f),0.4f);var aby=abs(p.y-0.2f)-0.6f;
            var d=dot2(new Expression2(six,-p.y)-clamp(0.5f*(six-p.y),0,0.2f));
            d=min(d,dot2(new Expression2(p.x,-aby)-clamp(0.5f*(p.x-aby),0,0.4f)));
            d=min(d,dot2(new Expression2(rex,p.y-clamp(p.y,0,0.4f))));
            var s=2*p.x+aby+abs(aby+0.4f)-0.4f;return sqrt(d)*sign(s);
        }
        public static Expression sdCircleWave(Expression2 p,Expression tb,Expression ra)
        {
            tb=3.1415927f*5/6*max(tb,0.0001f);var co=ra*new Expression2(sin(tb),cos(tb));
            p.x=abs(mod(p.x,co.x*4)-co.x*2);var p1=p;var p2=new Expression2(abs(p.x-2*co.x),-p.y+2*co.y);
            var d1=choose((co.y*p1.x).GreaterThan(co.x*p1.y),length(p1-co),abs(length(p1)-ra));
            var d2=choose((co.y*p2.x).GreaterThan(co.x*p2.y),length(p2-co),abs(length(p2)-ra));return min(d1,d2);
        }
    }
}
