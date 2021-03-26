using System;
using System.Collections.Generic;

namespace VityazReports.Models.GuardsOnMap {
    public class RoutesTiming {
        public string NameGroup { get; set; }
        public double Duration { get; set; }
        public string DurationText {
            get {
                return TimeSpan.FromSeconds(Duration).Minutes.ToString() + ":" + TimeSpan.FromSeconds(Duration).Seconds.ToString();
            }
                }
        public double Distance { get; set; }
        public List<List<double>> Coordinates { get; set; }
    }
}
