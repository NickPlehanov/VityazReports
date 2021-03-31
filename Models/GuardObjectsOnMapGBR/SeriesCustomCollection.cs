using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VityazReports.Models.GuardObjectsOnMapGBR {
    public class SeriesCustomCollection {
        public SeriesCustomCollection(string id, SeriesCollection seriesCollectionBuild, List<MatrixTotals> cOL) {
            Id = id;
            SeriesCollectionBuild = seriesCollectionBuild;
            COL = cOL;
        }

        public string Id { get; set; }
        public SeriesCollection SeriesCollectionBuild { get; set; }
        public List<MatrixTotals> COL { get; set; }
    }
}
