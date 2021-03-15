using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.ReglamentWorks {
    public class ReglamentWorksDetail {
        public string UserChanged { get; set; }
        public DateTime? DateChanged { get; set; }
        public string FieldChanged { get; set; }
        public string BeforeChanged { get; set; }
        public string AfterChanged { get; set; }
    }
}
