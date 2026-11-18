using System;

namespace FidgetExamples.Editor
{
    internal sealed class Entry
    {
        public readonly Type Type;
        public readonly string Title, Property;
        public readonly bool Integer;
        public Entry(Type type, string title, string property, bool integer)
        { Type = type; Title = title; Property = property; Integer = integer; }
    }
    internal sealed class Page
    {
        public readonly string Scene;
        public readonly Entry[] Entries;
        public Page(string scene, Entry[] entries) { Scene = scene; Entries = entries; }
    }
    internal static class SdfGalleryCatalog
    {
        internal static readonly Page[] Pages = {
            new Page("SdfSolids", new Entry[] {
                new Entry(typeof(RoundedSolids), "Rounded solids", "Radius", false),
                new Entry(typeof(ConeSolids), "Cones and frusta", "Radius", false),
                new Entry(typeof(CapsuleSolids), "Capsules and vesica", "Radius", false),
                new Entry(typeof(RingSolids), "Torus, arc and link", "TubeRadius", false),
                new Entry(typeof(PrismSolids), "Polygonal prisms", "Radius", false),
                new Entry(typeof(PolyhedronSolids), "Polyhedra", "Radius", false),
                new Entry(typeof(PowerPolyhedra), "Power-norm polyhedra", "Exponent", false),
                new Entry(typeof(SphericalSolids), "Cuts and spherical sectors", "CutHeight", false),
                new Entry(typeof(SurfaceSolids), "Thickened open surfaces", "Thickness", false),
                new Entry(typeof(DistanceSolids), "Cheap versus exact rounding", "Rounding", false),
                new Entry(typeof(BoundSolids), "Approximate fields", "Radius", false)
            }),
            new Page("SdfProfiles", new Entry[] {
                new Entry(typeof(RectangleProfiles), "Rectangle profiles", "Rounding", false),
                new Entry(typeof(TriangleProfiles), "Triangles and polygon", "Width", false),
                new Entry(typeof(PolygonProfiles), "Regular polygons", "Radius", false),
                new Entry(typeof(StarProfiles), "Stars and adjustable points", "Points", true),
                new Entry(typeof(CircularProfiles), "Pie, cut, arc and ring", "Aperture", false),
                new Entry(typeof(LensProfiles), "Lenses, moon and horseshoe", "Width", false),
                new Entry(typeof(CrossProfiles), "Cross profiles", "Rounding", false),
                new Entry(typeof(DecorativeProfiles), "Egg, heart, S and quadratic", "Scale", false),
                new Entry(typeof(CurveProfiles), "Ellipse, Bezier and conics", "Stroke", false),
                new Entry(typeof(ArchitecturalProfiles), "Trapezoid, skew, tunnel, stairs", "Skew", false),
                new Entry(typeof(WaveProfiles), "Wave, parabola and strokes", "Stroke", false)
            }),
            new Page("SdfOperations", new Entry[] {
                new Entry(typeof(SmoothJoins), "Smooth union, intersect, subtract", "BlendRadius", false),
                new Entry(typeof(RoundedJoins), "Rounded joins", "Radius", false),
                new Entry(typeof(ChamferedJoins), "Chamfered joins", "Radius", false),
                new Entry(typeof(ColumnJoins), "Column joins", "Count", true),
                new Entry(typeof(StairJoins), "Stair joins", "Count", true),
                new Entry(typeof(SurfaceFeatures), "Pipe, engrave, groove, tongue", "Width", false),
                new Entry(typeof(DeformedSolids), "Twist, bend, elongate, displace", "Strength", false),
                new Entry(typeof(LatticeRepetition), "Limited, mirrored and grid cells", "Radius", false),
                new Entry(typeof(PolarRepetition), "Radial cells and identifiers", "Count", true),
                new Entry(typeof(AxisRepetition), "Axes and finite intervals", "Radius", false),
                new Entry(typeof(FoldedSolids), "Symmetry and plane folding", "Offset", false),
                new Entry(typeof(ProfileConstruction), "Extrude, revolve, loft, elongate", "Rounding", false)
            })
        };
    }
}
