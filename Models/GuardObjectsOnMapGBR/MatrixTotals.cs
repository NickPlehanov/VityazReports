using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.GuardObjectsOnMapGBR {
    public class MatrixTotals {
        
        public MatrixTotals() {
        }

        public MatrixTotals(double duration, string durationText, string objectInfo, int id, List<List<double>> coordinates) {
            Duration = duration;
            DurationText = durationText;
            ObjectInfo = objectInfo;
            Id = id;
            this.coordinates = coordinates;
        }

        public double Duration { get; set; }
        public string DurationText { get; set; }
        public string ObjectInfo { get; set; }
        public int Id { get; set; }
        public List<List<double>> coordinates { get; set; }
        //public string ObjectNumber { get; set; }
        //public string ObjectAddress { get; set; }
        //public string ObjectName { get; set; }
    }
}
