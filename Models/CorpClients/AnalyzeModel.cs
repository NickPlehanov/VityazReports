using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.CorpClients {
    public class AnalyzeModel {
        public AnalyzeModel(DateTime dateRemove, int monthlyPay) {
            DateRemove = dateRemove;
            MonthlyPay = monthlyPay;
        }

        public DateTime DateRemove { get; set; }
        public int MonthlyPay { get; set; }
    }
}
