using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.ServicemanOnMapViewModel {
    public class ServicemanCoords {
        public ServicemanCoords(string incomeLatitude, string incomeLongitude, string outgoneLatitude, string outgoneLongitude, int? number, string name, DateTime? income, DateTime? outgone) {
            IncomeLatitude = incomeLatitude;
            IncomeLongitude = incomeLongitude;
            OutgoneLatitude = outgoneLatitude;
            OutgoneLongitude = outgoneLongitude;
            Number = number;
            Name = name;
            Income = income;
            Outgone = outgone;
        }

        public string IncomeLatitude { get; set; }
        public string IncomeLongitude { get; set; }
        public string OutgoneLatitude { get; set; }
        public string OutgoneLongitude { get; set; }
        public int? Number { get; set; }
        public string Name { get; set; }
        public DateTime? Income { get; set; }
        public DateTime? Outgone { get; set; }
    }
}
