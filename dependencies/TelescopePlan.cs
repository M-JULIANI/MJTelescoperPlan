using Elements.Geometry;
using MJTelescoperPlan;
using Newtonsoft.Json;
using Elements.Geometry.Solids;


namespace Elements
{
    public class TelescopePlan : GeometricElement
    {
        private double _minModule = 5.0;
        public Profile Boundary { get; set; }

        [JsonProperty("Add Id")]
        public string AddId { get; set; }

        // public Func<double , double, double> MapFunction {get; set;}
        public double MaxTelescopeDistance { get; set; }
        public int RecurseLimit { get; set; }

        public double MaxHeight { get; set; }
        public double MinHeight { get; set; }

        public double TelescopingMultiplier { get; set; }
        public double PercentShrink {get; set;}

        // public TelescopePlan(TelescopePlanOverrideAddition add, Func<double, double, double> func, double max)
        // {
        //     this.Boundary = add.Value.Boundary;
        //     this.AddId = add.Id;
        //     this.MapFunction = func;
        //     this.MaxTelescopeDistance = max;
        // }


        public TelescopePlan(Profile boundary, double max, int recurseLimit, double minHeight, double maxHeight, double multiplier, double percentShrink)
        {
            this.Boundary = boundary;
            this.AddId = Guid.NewGuid().ToString();
            this.MaxTelescopeDistance = max;
            this.RecurseLimit = (int)recurseLimit;
            this.MinHeight = minHeight;
            this.MaxHeight = maxHeight;
            this.TelescopingMultiplier = multiplier;
            this.PercentShrink = percentShrink;
        }

        public static double ComputeHorizontalDistance(int index, int recurseLimit, double maxTelescope, double minModule)
        {
            double module = maxTelescope / (recurseLimit - 1);
            double calc = module * index;
            double percent = calc == 0 ? minModule : calc;
            return percent;
        }

        public static double ComputeVerticalDistance(double minHeight, double maxHeight, int index, int maxIndex, double exponent = 1.0)
        {
            double initialPercent = index / (1.0 * (maxIndex - 1));
            initialPercent = 1.0 - initialPercent;

            double hInitial = minHeight + ((maxHeight - minHeight) * initialPercent);
            double normalized = (hInitial - minHeight) / (maxHeight - minHeight);
            double mapped = Math.Pow(normalized, exponent);
            double renormed = (mapped * (maxHeight - minHeight)) + minHeight;
            double clamped = Math.Min(Math.Max(renormed, minHeight), maxHeight);
            return clamped;
        }

        // public bool Match(TelescopePlanIdentity identity)
        // {
        //     return identity.AddId ==this.AddId;
        // }

        // public TelescopePlan Update(TelescopePlanOverride edit)
        // {
        //     this.Boundary = edit.Value.Boundary;
        //     return this;
        // }

        public override void UpdateRepresentations()
        {
            if (this.Boundary == null || this.Boundary?.Segments() == null)
            {
                return;
            }
            var finalSegments = GenerateShapes();
            var reps = finalSegments.Select(x => x);
            this.Representation = new Representation(new List<SolidOperation>());
            reps.ToList().ForEach(r => this.Representation.SolidOperations.Add(r));
            var verticalDistance = ComputeVerticalDistance(this.MinHeight, this.MaxHeight, 0, this.RecurseLimit + 1, this.TelescopingMultiplier);
            this.Representation.SolidOperations.Add(new Extrude(this.Boundary, verticalDistance, Vector3.ZAxis, false));
            this.Material = new Material("Telescope")
            {
                Color = "#ADD8E6"
            };

        }

        private List<Extrude> GenerateShapes()
        {
            var segments = this.Boundary.Segments();
            var finalSegments = new List<Extrude>();
            segments.ForEach(s => RecurseTelescope(s, this.RecurseLimit, finalSegments));
            return finalSegments;
        }

        private void RecurseTelescope(Line segment, int limit, List<Extrude> finalSegments)
        {
            var counter = 0;

            Polygon polygon = null;
            var selectedSegment = segment;
            Vector3 segmentDirection = segment.Direction().Unitized();
            while (counter < limit)
            {
                var polygonAndHeight = BuildPolygon(selectedSegment, segmentDirection, counter);
                polygon = polygonAndHeight.Polygon;
                var height = polygonAndHeight.Height;
                finalSegments.Add(new Extrude(new Profile(polygon), height, Vector3.ZAxis, false));
                selectedSegment = polygon.Segments().ElementAt(2).Reversed();
                counter++;
            }
        }

        private (Polygon Polygon, double Height) BuildPolygon(Line segment, Vector3 segmentDirection, int index)
        {
            var cross = segmentDirection.Cross(Vector3.ZAxis).Unitized();
            Line line = new Line(segment.Start, segment.End);
            var distance = ComputeHorizontalDistance(index, this.RecurseLimit, this.MaxTelescopeDistance, this._minModule);
            var verticalDistance = ComputeVerticalDistance(this.MinHeight, this.MaxHeight, index, this.RecurseLimit, this.TelescopingMultiplier);
            var funcResult = distance == 0 ? _minModule : distance;
            var shrinkDistance = segment.Length() * (1 - this.PercentShrink);

            //shrink line
            var isSameDirection = line.Direction().Unitized().IsAlmostEqualTo(segmentDirection);
            if (isSameDirection)
            {
                line.Start = line.Start + (segmentDirection * shrinkDistance);
                line.End = line.End - (segmentDirection * shrinkDistance);
            }

            var xform = new Transform(cross * funcResult);
            Line offsetLine = line.TransformedLine(xform);

            var vecs = new Vector3[] { line.Start, line.End, offsetLine.End, offsetLine.Start, line.Start };
            return (new Polygon(vecs), verticalDistance);
        }

    }
}