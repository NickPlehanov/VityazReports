using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.CorpClients {
    public class AnalyzeModel {
        public AnalyzeModel(DateTime dateRemove, int monthlyPay,int order) {
            DateRemove = dateRemove;
            MonthlyPay = monthlyPay;
            Order = order;
        }

        public DateTime DateRemove { get; set; }
        public int MonthlyPay { get; set; }
        public int Order { get; set; }
    }
}
