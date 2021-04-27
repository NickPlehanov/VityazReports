using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.AnalyzeServicemans {
    public class Servicemans {
        public Servicemans(string name, int? category, Guid servicemanID) {
            Name = name;
            Category = category;
            ServicemanID = servicemanID;
        }

        public string Name { get; set; }
        public int? Category { get; set; }
        public Guid ServicemanID { get; set; }
    }
}
