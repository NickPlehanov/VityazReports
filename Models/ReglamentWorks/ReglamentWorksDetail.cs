using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.ReglamentWorks {
    public class ReglamentWorksDetail {
        public ReglamentWorksDetail(DateTime? dateTimeOrder, string category, string servicemanName, string reason,string techConclusion) {
            DateTimeOrder = dateTimeOrder;
            Category = category;
            ServicemanName = servicemanName;
            Reason = reason;
            TechConclusion = techConclusion;
        }

        //public string UserChanged { get; set; }
        //public DateTime? DateChanged { get; set; }
        //public string FieldChanged { get; set; }
        //public string BeforeChanged { get; set; }
        //public string AfterChanged { get; set; }

        public DateTime? DateTimeOrder { get; set; }
        public string Category { get; set; }
        public string ServicemanName { get; set; }
        public string Reason { get; set; }
        public string TechConclusion { get; set; }
    }
}
