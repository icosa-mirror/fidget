using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Icosa.Fidget;
using UnityEngine;
using HgReferenceTypes;

static partial class ShapeLibraryChecks
{
    static int hgCells;
    static void HgChecks()
    {
        var portSignatures = typeof(HgSdf).GetMethods(BindingFlags.Static|BindingFlags.Public).Where(m=>m.Name!="Point")
            .Select(m=>m.Name+":"+string.Join(",",m.GetParameters().Select(p=>ReferenceType(p.ParameterType).Name))).OrderBy(n=>n);
        var referenceSignatures = typeof(HgReference).GetMethods(BindingFlags.Static|BindingFlags.Public)
            .Select(m=>m.Name+":"+string.Join(",",m.GetParameters().Select(p=>p.ParameterType.Name))).OrderBy(n=>n);
        if(!portSignatures.SequenceEqual(referenceSignatures)) throw new Exception("Mercury API coverage differs from upstream");
        var random = new System.Random(42119);
        var points = new List<Vector3> { Vector3.zero, new Vector3(0,1.4f,0), new Vector3(0.7f,0,0),
            new Vector3(-0.65f,0.65f,-0.65f), new Vector3(0.65f,-0.65f,0.65f),
            new Vector3(-1.3f,1.3f,0), new Vector3(0.001f,0.002f,0.003f) };
        for (int i=0;i<120;i++) points.Add(new Vector3((float)random.NextDouble()*5-2.5f,(float)random.NextDouble()*5-2.5f,(float)random.NextDouble()*5-2.5f));
        int checks = 0, functions = 0;
        foreach (var method in typeof(HgSdf).GetMethods(BindingFlags.Static|BindingFlags.Public).Where(m=>m.Name!="Point"))
        {
            var parameters = method.GetParameters();
            var reference = typeof(HgReference).GetMethods(BindingFlags.Static|BindingFlags.Public).SingleOrDefault(m=>
                m.Name==method.Name && m.GetParameters().Select(p=>ReferenceType(p.ParameterType)).SequenceEqual(parameters.Select(p=>ReferenceType(p.ParameterType)))) ?? throw new Exception("No reference for " + method);
            foreach (int cells in new[] {4,5})
            foreach (float radius in new[] {0.3f,0.7f,1.1f})
            {
                hgCells = cells;
                var builder = new ShapeBuilder();
                var args = parameters.Select(p=>HgArgument(p,builder,Vector3.zero,radius,false)).ToArray();
                var result = method.Invoke(null,args);
                var outputs = new List<Expression>();
                if (method.ReturnType != typeof(void)) Components(result,outputs);
                for(int i=0;i<parameters.Length;i++) if(parameters[i].ParameterType.IsByRef) Components(args[i],outputs);
                // Every exported graph must also compile through the native ABI.
                foreach(var output in outputs) using(builder.Compile(output)) { }
                foreach(var point in points)
                {
                    var numericArgs = parameters.Select(p=>HgArgument(p,null,point,radius,true)).ToArray();
                    var numericResult = reference.Invoke(null,numericArgs);
                    var expected = new List<float>();
                    if(method.ReturnType != typeof(void)) Components(numericResult,expected);
                    for(int i=0;i<parameters.Length;i++) if(parameters[i].ParameterType.IsByRef) Components(numericArgs[i],expected);
                    for(int i=0;i<outputs.Count;i++)
                    {
                        float actual = Value(outputs[i],point,radius), want = expected[i];
                        bool same = float.IsNaN(want) ? float.IsNaN(actual) : float.IsInfinity(want) ? actual==want : float.IsFinite(actual) && MathF.Abs(actual-want)<0.00015f*(1+MathF.Abs(want));
                        if(!same) throw new Exception($"{method.Name} at {point}, radius {radius}, output {i}: {actual} != reference {want}");
                        checks++;
                    }
                }
            }
            functions++;
        }
        // Conditions must retain selected zero and suppress undefined unselected expressions.
        var s = new ShapeBuilder();
        Near(Value(s.X.LessThan(0).Select(0,s.Y.Ln()),new Vector3(-1,-1,0)),0);
        Near(Value(s.X.EqualTo(0).Select(7,s.Y.Ln()),Vector3.zero),7);
        Near(Value(s.X.Pow(2.5f),Vector3.zero),0);
        Near(Value(s.X.Pow(s.Parameter()),Vector3.zero,0),1);
        Reject(()=>HgSdf.fGDF(HgSdf.Point(s),1,-1,18));
        Console.WriteLine($"PASS: Mercury {functions} overloads; {checks} numeric comparisons against original GLSL, including domain cell IDs; all emitted graphs compiled natively");
    }
    static Type ReferenceType(Type t)
    {
        if(t.IsByRef) return ReferenceType(t.GetElementType()).MakeByRefType();
        if(t==typeof(Expression)) return typeof(float);
        if(t==typeof(Expression2)) return typeof(Ref2);
        if(t==typeof(Expression3)) return typeof(Ref3);
        if(t==typeof(Expression4)) return typeof(Ref4);
        return t;
    }
    static object HgArgument(ParameterInfo info, ShapeBuilder s, Vector3 point, float radius, bool numeric)
    {
        var type = info.ParameterType.IsByRef ? info.ParameterType.GetElementType() : info.ParameterType;
        string name = info.Name;
        if(type==typeof(int)) return name=="begin" ? 3 : 6;
        if(type==typeof(Expression))
        {
            if(name=="p" || name=="a" || name=="x") return numeric ? (object)point.x : s.X;
            if(name=="b") return numeric ? (object)point.y : s.Y;
            float scalar = name switch { "r" or "radius" => radius, "smallRadius" => 0.2f, "largeRadius" => 0.8f,
                "height" => 1.4f, "c" => 0.6f, "n" or "repetitions" => hgCells, "e" => 3.5f,
                "start" => -2, "stop" => 3, "size" => 1.3f, "dist" or "offset" or "distanceFromOrigin" => 0.2f,
                "ra" => 0.4f, "rb" => 0.3f, "a" => 0.35f, _ => 0.35f };
            if(numeric) return scalar;
            return name=="r" || name=="radius" ? (Expression)s.Parameter(scalar) : (Expression)scalar;
        }
        float x,y,z,w;
        if(name=="p" || name=="v" || name=="x") { x=point.x; y=point.y; z=point.z; w=point.x-point.y; }
        else if(name=="a") { x=-0.6f;y=-0.3f;z=0.2f;w=0.1f; }
        else if(name=="n" || name=="planeNormal") { var normal=new Vector3(0.3f,0.6f,0.7f).normalized; x=normal.x;y=normal.y;z=normal.z;w=0; }
        else if(name=="size") { x=1.3f;y=0.9f;z=1.1f;w=1.7f; }
        else if(name=="dist") { x=0.2f;y=0.3f;z=0.4f;w=0.1f; }
        else { x=0.7f;y=0.9f;z=0.6f;w=0.8f; }
        if(type==typeof(Expression2)) return numeric ? (object)new Ref2(x,y) : name=="p" || name=="v" || name=="x" ? new Expression2(s.X,s.Y) : new Expression2(x,y);
        if(type==typeof(Expression3)) return numeric ? (object)new Ref3(x,y,z) : name=="p" || name=="v" || name=="x" ? HgSdf.Point(s) : new Expression3(x,y,z);
        if(type==typeof(Expression4)) return numeric ? (object)new Ref4(x,y,z,w) : new Expression4(x,y,z,w);
        throw new Exception("Unknown reference argument " + info);
    }
    static void Components(object value,List<Expression> result)
    {
        if(value is Expression e) result.Add(e);
        else if(value is Expression2 a) { result.Add(a.x);result.Add(a.y); }
        else if(value is Expression3 b) { result.Add(b.x);result.Add(b.y);result.Add(b.z); }
        else if(value is Expression4 c) { result.Add(c.x);result.Add(c.y);result.Add(c.z);result.Add(c.w); }
        else throw new Exception("Unknown expression result");
    }
    static void Components(object value,List<float> result)
    {
        if(value is float f) result.Add(f);
        else if(value is Ref2 a) { result.Add(a.x);result.Add(a.y); }
        else if(value is Ref3 b) { result.Add(b.x);result.Add(b.y);result.Add(b.z); }
        else if(value is Ref4 c) { result.Add(c.x);result.Add(c.y);result.Add(c.z);result.Add(c.w); }
        else throw new Exception("Unknown numeric result");
    }
}
