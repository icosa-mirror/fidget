using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Icosa.Fidget;
using UnityEngine;

static partial class ShapeLibraryChecks
{
    static void IqChecks()
    {
        var s = new ShapeBuilder();
        var p = IqSdf.Point(s);
        var random = new System.Random(7132);
        var points = Enumerable.Range(0,60).Select(_ => new Vector3(
            (float)random.NextDouble()*4-2, (float)random.NextDouble()*4-2, (float)random.NextDouble()*4-2)).ToArray();
        int probes = 0, overloads = 0;
        foreach (var method in typeof(IqSdf).GetMethods(BindingFlags.Public|BindingFlags.Static).Where(m => m.Name != "Point" && m.Name != "opTx"))
        {
            var builder = new ShapeBuilder();
            var args = method.GetParameters().Select(a => IqArgument(a,builder)).ToArray();
            Expression expression;
            try { expression = (Expression)method.Invoke(null,args); }
            catch (TargetInvocationException e) { throw new Exception("Could not construct " + method, e.InnerException); }
            using (builder.Compile(expression)) { }
            foreach (var point in points)
            {
                float value = Value(expression,point);
                if (!float.IsFinite(value)) throw new Exception($"{method} is undefined at valid probe {point}: {value}");
                probes++;
            }
            overloads++;
        }
        // Independent distances for shared primitives, at random positions.
        var sphere = IqSdf.sdSphere(p,0.8f);
        var box = IqSdf.sdBox(p,new Expression3(1,0.7f,0.9f));
        var cylinder = IqSdf.sdCappedCylinder(p,0.6f,0.8f);
        var capsule = IqSdf.sdCapsule(p,new Expression3(0,-0.5f,0),new Expression3(0,0.5f,0),0.3f);
        var torus = IqSdf.sdTorus(p,new Expression2(0.8f,0.2f));
        var polygon = IqSdf.sdPolygon(new[]{new Expression2(-1,-0.7f),new Expression2(1,-0.7f),new Expression2(1,0.7f),new Expression2(-1,0.7f)},p.xy);
        var ellipse = IqSdf.sdEllipse(p.xy,new Expression2(1.3f,0.7f));
        var bezier = IqSdf.sdBezier(p.xy,new Expression2(-1,0),new Expression2(0,1),new Expression2(1,0));
        foreach (var q in points)
        {
            Near(Value(sphere,q),MathF.Sqrt(q.x*q.x+q.y*q.y+q.z*q.z)-0.8f);
            Near(Value(box,q),NumericBox(q,new Vector3(1,0.7f,0.9f)));
            float radial = MathF.Sqrt(q.x*q.x+q.z*q.z);
            float dx=radial-0.6f,dy=MathF.Abs(q.y)-0.8f;
            Near(Value(cylinder,q),MathF.Sqrt(MathF.Max(dx,0)*MathF.Max(dx,0)+MathF.Max(dy,0)*MathF.Max(dy,0))+MathF.Min(MathF.Max(dx,dy),0));
            float capY=q.y-Math.Clamp(q.y,-0.5f,0.5f);
            Near(Value(capsule,q),MathF.Sqrt(q.x*q.x+capY*capY+q.z*q.z)-0.3f);
            Near(Value(torus,q),MathF.Sqrt((radial-0.8f)*(radial-0.8f)+q.y*q.y)-0.2f);
            Near(Value(polygon,q),NumericBox(new Vector3(q.x,q.y,0),new Vector3(1,0.7f,100)));
            // Numerical minimization is independent of IQ's closed-form roots.
            float ellipseDistance = MinCurve(t => {
                double x=1.3*Math.Cos(t)-Math.Abs(q.x),y=0.7*Math.Sin(t)-Math.Abs(q.y);
                return x*x+y*y;
            },0,Math.PI/2);
            ellipseDistance *= q.x*q.x/(1.3f*1.3f)+q.y*q.y/(0.7f*0.7f)<1 ? -1 : 1;
            Approx(Value(ellipse,q),ellipseDistance,0.0005f,"ellipse closest point");
            float bezierDistance = MinCurve(t => {
                double x=2*t-1-q.x,y=2*t*(1-t)-q.y;
                return x*x+y*y;
            },0,1);
            Approx(Value(bezier,q),bezierDistance,0.0005f,"Bezier closest point");
        }
        // Surface/interior boundary regressions and important API conventions.
        Near(Value(IqSdf.sdBoxFrame(p,new Expression3(1),0.1f),new Vector3(0.9f,0.9f,0)),-0.1f);
        Near(Value(IqSdf.sdBoxFrame(p,new Expression3(1),0.1f),Vector3.zero),MathF.Sqrt(2)*0.8f);
        Near(Value(IqSdf.sdRoundBox(p,new Expression3(1),0.2f),Vector3.zero),-1);
        Near(Value(IqSdf.sdEllipse(p.xy,new Expression2(1.3f,0.7f)),Vector3.zero),-0.7f);
        Near(Value(IqSdf.sdEllipse(p.xy,new Expression2(1)),new Vector3(2,0,0)),1);
        Near(Value(IqSdf.sdBezier(p.xy,new Expression2(-1,0),new Expression2(0,0),new Expression2(1,0)),new Vector3(0,1,0)),1);
        Near(Value(IqSdf.opSubtraction(-1,0.4f),Vector3.zero),1);
        Near(Value(IqSdf.opSmoothUnion(s.X,s.Y,0.2f),Vector3.zero),-0.2f);
        Near(Value(IqSdf.opTwist(q => q.y,p,2),new Vector3(1,0.4f,0.8f)),0.4f);
        Near(Value(IqSdf.opScale(p,2,q => IqSdf.sdSphere(q,1)),new Vector3(3,0,0)),1);
        var nonSymmetric = IqSdf.opSymX(p,q => IqSdf.sdSphere(q-new Expression3(0.7f,0,0),0.2f));
        Near(Value(nonSymmetric,new Vector3(-0.7f,0,0)),-0.2f);
        Reject(() => IqSdf.sdPolygon(Array.Empty<Expression2>(),p.xy));
        Reject(() => IqSdf.sdStar(p.xy,1,2,2));
        // Native meshing and analytic normals exercise evaluation/interval/gradient
        // behavior, beyond the independent scalar graph interpreter above.
        NativeIqMesh(s,IqSdf.sdBoxFrame(p,new Expression3(0.7f),0.1f),"box frame");
        NativeIqMesh(s,IqSdf.sdRoundedCylinder(p,0.6f,0.15f,0.7f),"rounded cylinder");
        NativeIqMesh(s,IqSdf.sdCappedCone(p,0.7f,0.6f,0.25f),"frustum");
        NativeIqMesh(s,IqSdf.opExtrusion(p,q => IqSdf.sdStar(q,0.65f,5,3),0.2f),"extruded star");
        NativeIqMesh(s,IqSdf.opExtrusion(p,q => IqSdf.sdEllipse(q,new Expression2(0.7f,0.4f)),0.2f),"extruded ellipse");
        NativeIqMesh(s,IqSdf.opTwist(q => IqSdf.sdBox(q,new Expression3(0.5f,0.7f,0.4f)),p,0.7f),"twisted box");
        Console.WriteLine($"PASS: IQ {overloads} overloads compiled natively; {probes} finite probes; independent distance/closest-point checks; six native meshes with surface normals");
    }
    static void Approx(float a,float b,float tolerance,string name)
    {
        if(!float.IsFinite(a)||MathF.Abs(a-b)>tolerance) throw new Exception($"{name}: {a} != {b}");
        count++;
    }
    static float NumericBox(Vector3 p,Vector3 extents)
    {
        float x=MathF.Abs(p.x)-extents.x,y=MathF.Abs(p.y)-extents.y,z=MathF.Abs(p.z)-extents.z;
        return MathF.Sqrt(MathF.Max(x,0)*MathF.Max(x,0)+MathF.Max(y,0)*MathF.Max(y,0)+MathF.Max(z,0)*MathF.Max(z,0))+MathF.Min(MathF.Max(x,MathF.Max(y,z)),0);
    }
    static float MinCurve(Func<double,double> squaredDistance,double lo,double hi)
    {
        // Bracket the global minimum before refining: some profiles have several
        // stationary points, so a single unbracketed search would be misleading.
        const int samples=256;
        double step=(hi-lo)/samples,best=lo,bestDistance=squaredDistance(lo);
        for(int i=1;i<=samples;i++) { double t=lo+i*step,d=squaredDistance(t); if(d<bestDistance) {best=t;bestDistance=d;} }
        double a=Math.Max(lo,best-step),b=Math.Min(hi,best+step);
        for(int i=0;i<70;i++) { double l=a+(b-a)/3,r=b-(b-a)/3; if(squaredDistance(l)<squaredDistance(r)) b=r; else a=l; }
        return (float)Math.Sqrt(Math.Min(bestDistance,squaredDistance((a+b)/2)));
    }
    static object IqArgument(ParameterInfo parameter,ShapeBuilder s)
    {
        var type=parameter.ParameterType;string name=parameter.Name;string method=parameter.Member.Name;
        if(type==typeof(Expression2[])) return new[]{new Expression2(-0.8f,-0.5f),new Expression2(0.8f,-0.5f),new Expression2(0,0.8f)};
        if(type==typeof(int)) return 5;
        if(type==typeof(Expression))
        {
            if((name=="a"||name=="b")&&method.StartsWith("op")) return name=="a" ? s.X : s.Y;
            float value=name switch {
                "r"=>0.8f,"radius"=>0.2f,"r1" or "ra"=>0.65f,"r2" or "rb"=>0.25f,
                "h"=>method.Contains("Cut")?0.2f:method=="sdRoundedCross"?0.6f:1.1f,
                "he"=>method=="sdEgg"?1.2f:0.7f,"bu"=>0.6f,"th" or "thickness" or "t" or "e" or "chamfer"=>0.15f,
                "k"=>0.6f,"n"=>5,"m"=>3,"s" or "scale"=>1.3f,"spacing"=>1.5f,
                "la" or "le" or "wi" or "w"=>0.7f,"lb"=>0.5f,"sk"=>0.2f,"d"=>0.7f,"tb"=>0.4f,"o"=>0.4f,
                "field"=>0.3f,_=>0.3f};
            return (Expression)value;
        }
        if(type==typeof(Expression4)) return new Expression4(0.1f,0.2f,0.15f,0.25f);
        if(type==typeof(Expression2))
        {
            if(name=="p"||name=="pos") return new Expression2(s.X,s.Y);
            if(name=="ab") return new Expression2(1.3f,0.7f);
            if(name=="sc"||name=="c") return new Expression2(0.6f,0.8f);
            if(name=="n") return new Expression2(0.8f,0.6f);
            if(name=="a"||name=="p0"||name=="A") return new Expression2(-0.8f,-0.3f);
            if(name=="p2"||name=="C") return new Expression2(0.8f,-0.2f);
            if(name=="B") return new Expression2(0.1f,0.9f);
            if(name=="p1") return new Expression2(0,0.9f);
            if(name=="b"&&(method=="sdOrientedBox"||method=="sdSegment"||method=="sdOrientedVesica")) return new Expression2(0.8f,0.4f);
            if(name=="t") return new Expression2(0.8f,0.2f);
            return new Expression2(0.8f,0.6f);
        }
        if(type==typeof(Expression3))
        {
            if(name=="p"||name=="p2") return IqSdf.Point(s);
            if(name=="n") return new Expression3(0,1,0);
            if(name=="c" && method=="sdCylinder") return new Expression3(0,0,0.6f);
            if(method=="udTriangle"||method=="udQuad") return name switch {
                "a"=>new Expression3(-0.7f,-0.5f,0),"b"=>new Expression3(0.7f,-0.5f,0),
                "c"=>new Expression3(method=="udQuad"?0.7f:0,0.6f,0),_=>new Expression3(-0.7f,0.6f,0)};
            if(name=="a") return new Expression3(-0.4f,-0.6f,-0.2f);
            if(name=="b"&&(method.Contains("Cylinder")||method.Contains("Cone")||method=="sdCapsule"||method=="sdVesicaSegment")) return new Expression3(0.5f,0.7f,0.2f);
            return new Expression3(1,0.7f,0.9f);
        }
        if(type==typeof(Func<Expression2,Expression>)) return (Func<Expression2,Expression>)(q=>IqSdf.sdCircle(q,0.5f));
        if(type==typeof(Func<Expression3,Expression>)) return parameter.Name=="displacement" ?
            (Func<Expression3,Expression>)(q=>0.1f*q.x.Sin()*q.y.Sin()) : q=>IqSdf.sdSphere(q-new Expression3(0.1f,0,0),0.5f);
        throw new Exception("No IQ argument for " + parameter);
    }
    static unsafe void NativeIqMesh(ShapeBuilder s,Expression expression,string name)
    {
        using var shape=s.Compile(expression);
        var mesher=shape.CreateMesher();IntPtr mesh=IntPtr.Zero;
        try
        {
            var bounds=new Native.Bounds { Min=new Vector3(-1.5f,-1.5f,-1.5f),Max=new Vector3(1.5f,1.5f,1.5f) };
            fixed (float* parameters = shape.Defaults)
                Native.Check(Native.fidget_mesher_generate(mesher,parameters,(uint)shape.Defaults.Length,in bounds,5,out mesh),"IQ "+name);
            Native.Check(Native.fidget_mesh_prepare_surface_normals(mesher,mesh),"IQ normals "+name);
            Native.Check(Native.fidget_mesh_get_counts(mesh,out var counts),"IQ counts "+name);
            if(counts.Vertices==0||counts.Indices==0) throw new Exception("Empty IQ mesh: "+name);
        }
        finally { Native.fidget_mesh_free(mesh);Native.fidget_mesher_free(mesher); }
    }
}
