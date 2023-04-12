using Elements.Geometry;
using MJTelescoperPlan;
using Newtonsoft.Json;
using Elements.Geometry.Solids;


namespace Elements{
    public class TelescopePlan : GeometricElement
    {
        public Profile Boundary {get; set;}

        [JsonProperty("Add Id")]
        public string AddId {get; set;}

       // public Func<double , double, double> MapFunction {get; set;}
        public double MaxTelescopeDistance {get; set;}
        public int RecurseLimit {get; set;}

        // public TelescopePlan(TelescopePlanOverrideAddition add, Func<double, double, double> func, double max)
        // {
        //     this.Boundary = add.Value.Boundary;
        //     this.AddId = add.Id;
        //     this.MapFunction = func;
        //     this.MaxTelescopeDistance = max;
        // }


        public TelescopePlan(Profile boundary, double max, double recurseLimit)
        {
            this.Boundary = boundary;
            this.AddId = Guid.NewGuid().ToString();
            // this.MapFunction = func;
            this.MaxTelescopeDistance = max;
            this.RecurseLimit = (int)recurseLimit;
        }

    public double ComputeDistance(double x, double y)
        {
          return x / (y == 0 ? 1 : y);
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
            if(this.Boundary == null || this.Boundary?.Segments() == null)
            {
                return;
            }
            Console.WriteLine("segs: " + this.Boundary.Segments().Count);
            var finalSegments = GenerateShapes();
            Console.WriteLine("FINAL segs: " + finalSegments.Count);
            var reps = finalSegments.Select(x=> new Lamina(x));
            this.Representation = new Representation(new List<SolidOperation>());
            reps.ToList().ForEach(r=> this.Representation.SolidOperations.Add(r));
            this.Material = new Material("Telescope"){
                Color="#ADD8E6"
            };

        }

        private List<Profile> GenerateShapes()
        {
            var segments = this.Boundary.Segments();
            var finalSegments = new List<Profile>();
            segments.ForEach(s=> RecurseTelescope(s, this.RecurseLimit, finalSegments));
            return finalSegments;
        }

        private void RecurseTelescope(Line segment, int limit, List<Profile> finalSegments)
        {
            var counter = 0;

            Polygon polygon = null;
            var selectedSegment = segment;
            while(counter<limit)
            {
                polygon = BuildPolygon(selectedSegment, counter);
                selectedSegment = polygon.Segments()[0];
                finalSegments.Add(new Profile(polygon));
                counter++;
            }
        }

        private Polygon BuildPolygon(Line segment, int index)
        {
            var dir = segment.Direction().Unitized();
            var cross = dir.Cross(Vector3.ZAxis);
            
            Line line = new Line(segment.Start, segment.End);

            var funcResult = ComputeDistance(index, this.MaxTelescopeDistance);
            var offset =  funcResult == 0 ? (5 * index) + 5 : funcResult;
            Console.WriteLine("index is: " + index);
            Console.WriteLine("func result is: " + funcResult);
            Console.WriteLine("offset is: " + offset);
            var distance = 0.75;
            //shrink line
            line.Start = line.Start + (dir * distance);
            line.End = line.End - (dir * distance);
            //offset line
            var offsetStart = line.Start + (cross * offset);
            var offsetEnd = line.End + (cross * offset);
            Line offsetLine = new Line(offsetStart, offsetEnd);

            var vecs = new Vector3[]{line.Start, line.End, offsetLine.End, offsetLine.Start};
            return new Polygon(vecs);
        }

    }
}