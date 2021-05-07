using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.CorpClients {
    public class AccountModel {
        public AccountModel(Guid accountId, Guid? parentAccountId, string accountName,string parentAccountName, DateTime? accountEndDate) {
            AccountId = accountId;
            ParentAccountId = parentAccountId;
            AccountName = accountName;
            ParentAccountName = parentAccountName;
            AccountEndDate = accountEndDate;
        }

        public Guid AccountId { get; set; }
        public Guid? ParentAccountId { get; set; }
        public string AccountName { get; set; }
        public string ParentAccountName { get; set; }
        public DateTime? AccountEndDate { get; set; }
    }
}
