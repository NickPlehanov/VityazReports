using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.GuardObjectsOnMapGBR {
    public class MatrixTotals {
        public MatrixTotals(int duration, string objectInfo, string durationtext, int id) {
            Duration = duration;
            ObjectInfo = objectInfo;
            DurationText = durationtext;
            Id = id;
        }
        public MatrixTotals() {
        }

        public int Duration { get; set; }
        public string DurationText { get; set; }
        public string ObjectInfo { get; set; }
        public int Id { get; set; }
        //public string ObjectNumber { get; set; }
        //public string ObjectAddress { get; set; }
        //public string ObjectName { get; set; }
    }
}
