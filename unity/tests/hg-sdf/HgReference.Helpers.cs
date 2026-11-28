using System;
using HgReferenceTypes;

static partial class HgReference
{
    static float min(float a,float b) => MathF.Min(a,b);
    static float max(float a,float b) => MathF.Max(a,b);
    static float abs(float a) => MathF.Abs(a);
    static float sqrt(float a) => MathF.Sqrt(a);
    static float cos(float a) => MathF.Cos(a);
    static float sin(float a) => MathF.Sin(a);
    static float floor(float a) => MathF.Floor(a);
    static float mod(float a,float b) => a-b*MathF.Floor(a/b);
    static float atan(float y,float x) => MathF.Atan2(y,x);
    static float pow(float a,float b) => MathF.Pow(a,b);
    public static float saturate(float a) => min(max(a,0),1);
    static float step(float edge,float a) => a < edge ? 0 : 1;
    static float mix(float a,float b,float t) => a*(1-t)+b*t;
    static Ref2 abs(Ref2 a) => new Ref2(abs(a.x),abs(a.y));
    static Ref3 abs(Ref3 a) => new Ref3(abs(a.x),abs(a.y),abs(a.z));
    static Ref2 floor(Ref2 a) => new Ref2(floor(a.x),floor(a.y));
    static Ref3 floor(Ref3 a) => new Ref3(floor(a.x),floor(a.y),floor(a.z));
    static Ref2 mod(Ref2 a,Ref2 b) => new Ref2(mod(a.x,b.x),mod(a.y,b.y));
    static Ref3 mod(Ref3 a,Ref3 b) => new Ref3(mod(a.x,b.x),mod(a.y,b.y),mod(a.z,b.z));
    static Ref2 min(Ref2 a,Ref2 b) => new Ref2(min(a.x,b.x),min(a.y,b.y));
    static Ref3 min(Ref3 a,Ref3 b) => new Ref3(min(a.x,b.x),min(a.y,b.y),min(a.z,b.z));
    static Ref2 max(Ref2 a,Ref2 b) => new Ref2(max(a.x,b.x),max(a.y,b.y));
    static Ref3 max(Ref3 a,Ref3 b) => new Ref3(max(a.x,b.x),max(a.y,b.y),max(a.z,b.z));
    static float dot(Ref2 a,Ref2 b) => a.x*b.x+a.y*b.y;
    static float dot(Ref3 a,Ref3 b) => a.x*b.x+a.y*b.y+a.z*b.z;
    static float length(Ref2 a) => sqrt(dot(a,a));
    static float length(Ref3 a) => sqrt(dot(a,a));
    static Ref2 normalize(Ref2 a) => a/length(a);
    static Ref3 normalize(Ref3 a) => a/length(a);
}
