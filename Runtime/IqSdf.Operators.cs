// Formulas by Inigo Quilez: https://iquilezles.org/articles/distfunctions/
using System;
using UnityEngine;

namespace Icosa.Fidget
{
    public static partial class IqSdf
    {
        public static Expression opRevolution(Expression3 p,Func<Expression2,Expression> primitive,Expression o) => primitive(new Expression2(length(p.xz)-o,p.y));
        public static Expression opExtrusion(Expression3 p,Func<Expression2,Expression> primitive,Expression h)
        {
            var w=new Expression2(primitive(p.xy),abs(p.z)-h);
            return min(max(w.x,w.y),0)+length(max(w,0));
        }
        public static Expression opElongate(Func<Expression3,Expression> primitive,Expression3 p,Expression3 h) => primitive(p-clamp(p,-h,h));
        public static Expression opElongateInterior(Func<Expression3,Expression> primitive,Expression3 p,Expression3 h)
        {
            var q=abs(p)-h;return primitive(max(q,0))+min(max(q.x,max(q.y,q.z)),0);
        }
        public static Expression opRound(Expression field,Expression radius) => field-radius;
        public static Expression opOnion(Expression field,Expression thickness) => abs(field)-thickness;
        public static Expression length2(Expression3 p) => length(p);
        public static Expression length6(Expression3 p)
        {
            p=p*p*p;p=p*p;return pow(p.x+p.y+p.z,1f/6);
        }
        public static Expression length8(Expression3 p)
        {
            p=p*p;p=p*p;p=p*p;return pow(p.x+p.y+p.z,1f/8);
        }
        public static Expression opUnion(Expression a,Expression b) => min(a,b);
        /// <summary>IQ operand order: subtract a from b.</summary>
        public static Expression opSubtraction(Expression a,Expression b) => max(-a,b);
        public static Expression opIntersection(Expression a,Expression b) => max(a,b);
        public static Expression opXor(Expression a,Expression b) => max(min(a,b),-max(a,b));
        public static Expression opSmoothUnion(Expression a,Expression b,Expression k)
        {
            k*=4;var h=max(k-abs(a-b),0);return min(a,b)-h*h*0.25f/k;
        }
        public static Expression opSmoothSubtraction(Expression a,Expression b,Expression k) => -opSmoothUnion(a,-b,k);
        public static Expression opSmoothIntersection(Expression a,Expression b,Expression k) => -opSmoothUnion(-a,-b,k);
        public static Expression opTx(Expression3 p,Matrix4x4 localToModel,Func<Expression3,Expression> primitive)
        {
            for(int i=0;i<16;i++) ShapeBuilder.Finite(localToModel[i],nameof(localToModel));
            if(localToModel.m30!=0 || localToModel.m31!=0 || localToModel.m32!=0 || localToModel.m33!=1 || Mathf.Abs(localToModel.determinant)<1e-12f)
                throw new ArgumentException("Transform must be affine and invertible.",nameof(localToModel));
            var m=localToModel.inverse;
            return primitive(new Expression3(m.m00*p.x+m.m01*p.y+m.m02*p.z+m.m03,m.m10*p.x+m.m11*p.y+m.m12*p.z+m.m13,m.m20*p.x+m.m21*p.y+m.m22*p.z+m.m23));
        }
        public static Expression opScale(Expression3 p,Expression scale,Func<Expression3,Expression> primitive) => primitive(p/scale)*scale;
        public static Expression opSymX(Expression3 p,Func<Expression3,Expression> primitive)
        {
            p.x=abs(p.x);return primitive(p);
        }
        public static Expression opSymXZ(Expression3 p,Func<Expression3,Expression> primitive)
        {
            p.xz=abs(p.xz);return primitive(p);
        }
        public static Expression opRepetition(Expression3 p,Expression3 spacing,Func<Expression3,Expression> primitive) => primitive(p-spacing*round(p/spacing));
        public static Expression opLimitedRepetition(Expression3 p,Expression spacing,Expression3 limit,Func<Expression3,Expression> primitive) => primitive(p-spacing*clamp(round(p/spacing),-limit,limit));
        public static Expression opDisplace(Func<Expression3,Expression> primitive,Expression3 p,Func<Expression3,Expression> displacement) => primitive(p)+displacement(p);
        public static Expression opTwist(Func<Expression3,Expression> primitive,Expression3 p,Expression k)
        {
            var c=cos(k*p.y);var s=sin(k*p.y);
            // Keep Y in the middle component when twisting the XZ plane.
            return primitive(new Expression3(c*p.x+s*p.z,p.y,-s*p.x+c*p.z));
        }
        public static Expression opCheapBend(Func<Expression3,Expression> primitive,Expression3 p,Expression k)
        {
            var c=cos(k*p.x);var s=sin(k*p.x);
            return primitive(new Expression3(c*p.x+s*p.y,-s*p.x+c*p.y,p.z));
        }
    }
}
