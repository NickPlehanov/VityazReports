using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.CorpClients {
    public class AccountModel {
        public AccountModel(Guid accountId, Guid? parentAccountId, string accountName, string parentAccountName, DateTime? accountEndDate, int? countObjects, double? paySumObjects, string address,int? countSubOrg,string owner) {
            AccountId = accountId;
            ParentAccountId = parentAccountId;
            AccountName = accountName;
            ParentAccountName = parentAccountName;
            AccountEndDate = accountEndDate;
            CountObjects = countObjects;
            PaySumObjects = paySumObjects;
            Address = address;
            CountSubOrg = countSubOrg;
            Owner = owner;
        }

        //public AccountModel(Guid accountId, Guid? parentAccountId, string accountName,string parentAccountName, DateTime? accountEndDate) {
        //    AccountId = accountId;
        //    ParentAccountId = parentAccountId;
        //    AccountName = accountName;
        //    ParentAccountName = parentAccountName;
        //    AccountEndDate = accountEndDate;
        //}

        public Guid AccountId { get; set; }
        public Guid? ParentAccountId { get; set; }
        public string AccountName { get; set; }
        public string ParentAccountName { get; set; }
        public DateTime? AccountEndDate { get; set; }
        public int? CountObjects { get; set; }
        public double? PaySumObjects { get; set; }
        public string Address { get; set; }
        public int? CountSubOrg { get; set; }
        public string Owner { get; set; }
        private string _Link { get; set; }
        public string Link {
            get {
                return string.Format(@"http://crm/vtz2/sfa/accts/edit.aspx?id=%7b{0}%7d#", AccountId);
            }
            set {
                _Link = string.Format(@"http://crm/vtz2/sfa/accts/edit.aspx?id=%7b{0}%7d#", AccountId);
            }
        }
    }
}
