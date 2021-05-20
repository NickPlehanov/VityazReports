using LiveCharts;
using System.Collections.Generic;

namespace VityazReports.Models.GuardObjectsOnMapGBR {
    public class SeriesCustomCollection {
        public SeriesCustomCollection(string id, SeriesCollection seriesCollectionBuild, List<MatrixTotals> cOL, double? latitude, double? longitude) {
            Id = id;
            SeriesCollectionBuild = seriesCollectionBuild;
            COL = cOL;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Id { get; set; }
        public SeriesCollection SeriesCollectionBuild { get; set; }
        public List<MatrixTotals> COL { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
