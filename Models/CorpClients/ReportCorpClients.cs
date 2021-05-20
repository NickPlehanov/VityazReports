namespace VityazReports.Models.CorpClients {
    public class ReportCorpClients {
        public ReportCorpClients(string headOrganizationName, string subOrganizationName, string sumMonthlyPay, string countGuardObject) {
            HeadOrganizationName = headOrganizationName;
            SubOrganizationName = subOrganizationName;
            SumMonthlyPay = sumMonthlyPay;
            CountGuardObject = countGuardObject;
        }

        public string HeadOrganizationName { get; set; }
        public string SubOrganizationName { get; set; }
        public string SumMonthlyPay { get; set; }
        public string CountGuardObject { get; set; }
    }
}
