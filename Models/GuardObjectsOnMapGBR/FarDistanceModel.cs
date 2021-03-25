using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.GuardObjectsOnMapGBR {
    public class FarDistanceModel {
        public FarDistanceModel(int objectNumber, string objectName, string objectAddress, double distance) {
            ObjectNumber = objectNumber;
            ObjectName = objectName;
            ObjectAddress = objectAddress;
            Distance = distance;
        }

        public int ObjectNumber { get; set; }
        public string ObjectName { get; set; }
        public string ObjectAddress { get; set; }
        public double Distance { get; set; }
    }
}
