// Generated from hg_sdf.glsl; do not edit. Mercury MIT notice is retained there.
using System;
using HgReferenceTypes;
static partial class HgReference
{
const float PI = MathF.PI, TAU = 2*PI;
static readonly float PHI = MathF.Sqrt(5)*0.5f+0.5f;
public static float sgn(float x) {
	return (x<0)?-1:1;
}

public static Ref2 sgn(Ref2 v) {
	return new Ref2((v.x<0)?-1:1, (v.y<0)?-1:1);
}

public static float square(float x) {
	return x*x;
}

public static Ref2 square(Ref2 x) {
	return x*x;
}

public static Ref3 square(Ref3 x) {
	return x*x;
}

public static float lengthSqr(Ref3 x) {
	return dot(x, x);
}



public static float vmax(Ref2 v) {
	return max(v.x, v.y);
}

public static float vmax(Ref3 v) {
	return max(max(v.x, v.y), v.z);
}

public static float vmax(Ref4 v) {
	return max(max(v.x, v.y), max(v.z, v.w));
}

public static float vmin(Ref2 v) {
	return min(v.x, v.y);
}

public static float vmin(Ref3 v) {
	return min(min(v.x, v.y), v.z);
}

public static float vmin(Ref4 v) {
	return min(min(v.x, v.y), min(v.z, v.w));
}




















public static float fSphere(Ref3 p, float r) {
	return length(p) - r;
}


public static float fPlane(Ref3 p, Ref3 n, float distanceFromOrigin) {
	return dot(p, n) + distanceFromOrigin;
}


public static float fBoxCheap(Ref3 p, Ref3 b) {
	return vmax(abs(p) - b);
}


public static float fBox(Ref3 p, Ref3 b) {
	Ref3 d = abs(p) - b;
	return length(max(d, new Ref3(0))) + vmax(min(d, new Ref3(0)));
}


public static float fBox2Cheap(Ref2 p, Ref2 b) {
	return vmax(abs(p)-b);
}

public static float fBox2(Ref2 p, Ref2 b) {
	Ref2 d = abs(p) - b;
	return length(max(d, new Ref2(0))) + vmax(min(d, new Ref2(0)));
}



public static float fCorner(Ref2 p) {
	return length(max(p, new Ref2(0))) + vmax(min(p, new Ref2(0)));
}


public static float fBlob(Ref3 p) {
	p = abs(p);
	if (p.x < max(p.y, p.z)) p = p.yzx;
	if (p.x < max(p.y, p.z)) p = p.yzx;
	float b = max(max(max(
		dot(p, normalize(new Ref3(1, 1, 1))),
		dot(p.xz, normalize(new Ref2(PHI+1, 1)))),
		dot(p.yx, normalize(new Ref2(1, PHI)))),
		dot(p.xz, normalize(new Ref2(1, PHI))));
	float l = length(p);
	return l - 1.5f - 0.2f * (1.5f / 2)* cos(min(sqrt(1.01f - b / l)*(PI / 0.25f), PI));
}


public static float fCylinder(Ref3 p, float r, float height) {
	float d = length(p.xz) - r;
	d = max(d, abs(p.y) - height);
	return d;
}


public static float fCapsule(Ref3 p, float r, float c) {
	return mix(length(p.xz) - r, length(new Ref3(p.x, abs(p.y) - c, p.z)) - r, step(c, abs(p.y)));
}


public static float fLineSegment(Ref3 p, Ref3 a, Ref3 b) {
	Ref3 ab = b - a;
	float t = saturate(dot(p - a, ab) / dot(ab, ab));
	return length((ab*t + a) - p);
}


public static float fCapsule(Ref3 p, Ref3 a, Ref3 b, float r) {
	return fLineSegment(p, a, b) - r;
}


public static float fTorus(Ref3 p, float smallRadius, float largeRadius) {
	return length(new Ref2(length(p.xz) - largeRadius, p.y)) - smallRadius;
}


public static float fCircle(Ref3 p, float r) {
	float l = length(p.xz) - r;
	return length(new Ref2(p.y, l));
}



public static float fDisc(Ref3 p, float r) {
	float l = length(p.xz) - r;
	return l < 0 ? abs(p.y) : length(new Ref2(p.y, l));
}


public static float fHexagonCircumcircle(Ref3 p, Ref2 h) {
	Ref3 q = abs(p);
	return max(q.y - h.y, max(q.x*sqrt(3)*0.5f + q.z*0.5f, q.z) - h.x);


}


public static float fHexagonIncircle(Ref3 p, Ref2 h) {
	return fHexagonCircumcircle(p, new Ref2(h.x*sqrt(3)*0.5f, h.y));
}


public static float fCone(Ref3 p, float radius, float height) {
	Ref2 q = new Ref2(length(p.xz), p.y);
	Ref2 tip = q - new Ref2(0, height);
	Ref2 mantleDir = normalize(new Ref2(height, radius));
	float mantle = dot(tip, mantleDir);
	float d = max(mantle, -q.y);
	float projected = dot(tip, new Ref2(mantleDir.y, -mantleDir.x));


	if ((q.y > height) && (projected < 0)) {
		d = max(d, length(tip));
	}


	if ((q.x > radius) && (projected > length(new Ref2(height, radius)))) {
		d = max(d, length(q - new Ref2(radius, 0)));
	}
	return d;
}












static readonly Ref3[] GDFVectors = new Ref3[]{
	normalize(new Ref3(1, 0, 0)),
	normalize(new Ref3(0, 1, 0)),
	normalize(new Ref3(0, 0, 1)),

	normalize(new Ref3(1, 1, 1 )),
	normalize(new Ref3(-1, 1, 1)),
	normalize(new Ref3(1, -1, 1)),
	normalize(new Ref3(1, 1, -1)),

	normalize(new Ref3(0, 1, PHI+1)),
	normalize(new Ref3(0, -1, PHI+1)),
	normalize(new Ref3(PHI+1, 0, 1)),
	normalize(new Ref3(-PHI-1, 0, 1)),
	normalize(new Ref3(1, PHI+1, 0)),
	normalize(new Ref3(-1, PHI+1, 0)),

	normalize(new Ref3(0, PHI, 1)),
	normalize(new Ref3(0, -PHI, 1)),
	normalize(new Ref3(1, 0, PHI)),
	normalize(new Ref3(-1, 0, PHI)),
	normalize(new Ref3(PHI, 1, 0)),
	normalize(new Ref3(-PHI, 1, 0))
};



public static float fGDF(Ref3 p, float r, float e, int begin, int end) {
	float d = 0;
	for (int i = begin; i <= end; ++i)
		d += pow(abs(dot(p, GDFVectors[i])), e);
	return pow(d, 1/e) - r;
}


public static float fGDF(Ref3 p, float r, int begin, int end) {
	float d = 0;
	for (int i = begin; i <= end; ++i)
		d = max(d, abs(dot(p, GDFVectors[i])));
	return d - r;
}



public static float fOctahedron(Ref3 p, float r, float e) {
	return fGDF(p, r, e, 3, 6);
}

public static float fDodecahedron(Ref3 p, float r, float e) {
	return fGDF(p, r, e, 13, 18);
}

public static float fIcosahedron(Ref3 p, float r, float e) {
	return fGDF(p, r, e, 3, 12);
}

public static float fTruncatedOctahedron(Ref3 p, float r, float e) {
	return fGDF(p, r, e, 0, 6);
}

public static float fTruncatedIcosahedron(Ref3 p, float r, float e) {
	return fGDF(p, r, e, 3, 18);
}

public static float fOctahedron(Ref3 p, float r) {
	return fGDF(p, r, 3, 6);
}

public static float fDodecahedron(Ref3 p, float r) {
	return fGDF(p, r, 13, 18);
}

public static float fIcosahedron(Ref3 p, float r) {
	return fGDF(p, r, 3, 12);
}

public static float fTruncatedOctahedron(Ref3 p, float r) {
	return fGDF(p, r, 0, 6);
}

public static float fTruncatedIcosahedron(Ref3 p, float r) {
	return fGDF(p, r, 3, 18);
}


































public static void pR(ref Ref2 p, float a) {
	p = cos(a)*p + sin(a)*new Ref2(p.y, -p.x);
}


public static void pR45(ref Ref2 p) {
	p = (p + new Ref2(p.y, -p.x))*sqrt(0.5f);
}



public static float pMod1(ref float p, float size) {
	float halfsize = size*0.5f;
	float c = floor((p + halfsize)/size);
	p = mod(p + halfsize, size) - halfsize;
	return c;
}


public static float pModMirror1(ref float p, float size) {
	float halfsize = size*0.5f;
	float c = floor((p + halfsize)/size);
	p = mod(p + halfsize,size) - halfsize;
	p *= mod(c, 2.0f)*2 - 1;
	return c;
}


public static float pModSingle1(ref float p, float size) {
	float halfsize = size*0.5f;
	float c = floor((p + halfsize)/size);
	if (p >= 0)
		p = mod(p + halfsize, size) - halfsize;
	return c;
}


public static float pModInterval1(ref float p, float size, float start, float stop) {
	float halfsize = size*0.5f;
	float c = floor((p + halfsize)/size);
	p = mod(p+halfsize, size) - halfsize;
	if (c > stop) {
		p += size*(c - stop);
		c = stop;
	}
	if (c <start) {
		p += size*(c - start);
		c = start;
	}
	return c;
}




public static float pModPolar(ref Ref2 p, float repetitions) {
	float angle = 2*PI/repetitions;
	float a = atan(p.y, p.x) + angle/2.0f;
	float r = length(p);
	float c = floor(a/angle);
	a = mod(a,angle) - angle/2.0f;
	p = new Ref2(cos(a), sin(a))*r;


	if (abs(c) >= (repetitions/2)) c = abs(c);
	return c;
}


public static Ref2 pMod2(ref Ref2 p, Ref2 size) {
	Ref2 c = floor((p + size*0.5f)/size);
	p = mod(p + size*0.5f,size) - size*0.5f;
	return c;
}


public static Ref2 pModMirror2(ref Ref2 p, Ref2 size) {
	Ref2 halfsize = size*0.5f;
	Ref2 c = floor((p + halfsize)/size);
	p = mod(p + halfsize, size) - halfsize;
	p *= mod(c,new Ref2(2))*2 - new Ref2(1);
	return c;
}


public static Ref2 pModGrid2(ref Ref2 p, Ref2 size) {
	Ref2 c = floor((p + size*0.5f)/size);
	p = mod(p + size*0.5f, size) - size*0.5f;
	p *= mod(c,new Ref2(2))*2 - new Ref2(1);
	p -= size/2;
	if (p.x > p.y) p.xy = p.yx;
	return floor(c/2);
}


public static Ref3 pMod3(ref Ref3 p, Ref3 size) {
	Ref3 c = floor((p + size*0.5f)/size);
	p = mod(p + size*0.5f, size) - size*0.5f;
	return c;
}


public static float pMirror(ref float p, float dist) {
	float s = sgn(p);
	p = abs(p)-dist;
	return s;
}



public static Ref2 pMirrorOctant(ref Ref2 p, Ref2 dist) {
	Ref2 s = sgn(p);
	pMirror(ref p.x, dist.x);
	pMirror(ref p.y, dist.y);
	if (p.y > p.x)
		p.xy = p.yx;
	return s;
}


public static float pReflect(ref Ref3 p, Ref3 planeNormal, float offset) {
	float t = dot(p, planeNormal)+offset;
	if (t < 0) {
		p = p - (2*t)*planeNormal;
	}
	return sgn(t);
}



























































public static float fOpUnionChamfer(float a, float b, float r) {
	return min(min(a, b), (a - r + b)*sqrt(0.5f));
}




public static float fOpIntersectionChamfer(float a, float b, float r) {
	return max(max(a, b), (a + r + b)*sqrt(0.5f));
}


public static float fOpDifferenceChamfer(float a, float b, float r) {
	return fOpIntersectionChamfer(a, -b, r);
}


public static float fOpUnionRound(float a, float b, float r) {
	Ref2 u = max(new Ref2(r - a,r - b), new Ref2(0));
	return max(r, min (a, b)) - length(u);
}

public static float fOpIntersectionRound(float a, float b, float r) {
	Ref2 u = max(new Ref2(r + a,r + b), new Ref2(0));
	return min(-r, max (a, b)) + length(u);
}

public static float fOpDifferenceRound(float a, float b, float r) {
	return fOpIntersectionRound(a, -b, r);
}



public static float fOpUnionColumns(float a, float b, float r, float n) {
	if ((a < r) && (b < r)) {
		Ref2 p = new Ref2(a, b);
		float columnradius = r*sqrt(2)/((n-1)*2+sqrt(2));
		pR45(ref p);
		p.x -= sqrt(2)/2*r;
		p.x += columnradius*sqrt(2);
		if (mod(n,2) == 1) {
			p.y += columnradius;
		}



		pMod1(ref p.y, columnradius*2);
		float result = length(p) - columnradius;
		result = min(result, p.x);
		result = min(result, a);
		return min(result, b);
	} else {
		return min(a, b);
	}
}

public static float fOpDifferenceColumns(float a, float b, float r, float n) {
	a = -a;
	float m = min(a, b);

	if ((a < r) && (b < r)) {
		Ref2 p = new Ref2(a, b);
		float columnradius = r*sqrt(2)/n/2.0f;
		columnradius = r*sqrt(2)/((n-1)*2+sqrt(2));

		pR45(ref p);
		p.y += columnradius;
		p.x -= sqrt(2)/2*r;
		p.x += -columnradius*sqrt(2)/2;

		if (mod(n,2) == 1) {
			p.y += columnradius;
		}
		pMod1(ref p.y,columnradius*2);

		float result = -length(p) + columnradius;
		result = max(result, p.x);
		result = min(result, a);
		return -min(result, b);
	} else {
		return -m;
	}
}

public static float fOpIntersectionColumns(float a, float b, float r, float n) {
	return fOpDifferenceColumns(a,-b,r, n);
}



public static float fOpUnionStairs(float a, float b, float r, float n) {
	float s = r/n;
	float u = b-r;
	return min(min(a,b), 0.5f * (u + a + abs ((mod (u - a + s, 2 * s)) - s)));
}


public static float fOpIntersectionStairs(float a, float b, float r, float n) {
	return -fOpUnionStairs(-a, -b, r, n);
}

public static float fOpDifferenceStairs(float a, float b, float r, float n) {
	return -fOpUnionStairs(-a, b, r, n);
}





public static float fOpUnionSoft(float a, float b, float r) {
	float e = max(r - abs(a - b), 0);
	return min(a, b) - e*e*0.25f/r;
}




public static float fOpPipe(float a, float b, float r) {
	return length(new Ref2(a, b)) - r;
}


public static float fOpEngrave(float a, float b, float r) {
	return max(a, (a + r - abs(b))*sqrt(0.5f));
}


public static float fOpGroove(float a, float b, float ra, float rb) {
	return max(a, min(a + ra, rb - abs(b)));
}


public static float fOpTongue(float a, float b, float ra, float rb) {
	return min(a, max(a - ra, abs(b) - rb));
}
}
