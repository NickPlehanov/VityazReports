using System.Collections.Generic;

namespace VityazReports.Models.GuardObjectsOnMapGBR {
    public class Waypoint {
        public string hint { get; set; }
        public double distance { get; set; }
        public List<double> location { get; set; }
        public string name { get; set; }
    }

    public class Leg {
        public List<object> steps { get; set; }
        public double weight { get; set; }
        public double distance { get; set; }
        public string summary { get; set; }
        public double duration { get; set; }
    }

    public class Geometry {
        public List<List<double>> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Route {
        public List<Leg> legs { get; set; }
        public string weight_name { get; set; }
        public Geometry geometry { get; set; }
        public double weight { get; set; }
        public double distance { get; set; }
        public double duration { get; set; }
    }

    public class OSRM {
        public string code { get; set; }
        public List<Waypoint> waypoints { get; set; }
        public List<Route> routes { get; set; }
    }
}
