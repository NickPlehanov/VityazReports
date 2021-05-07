using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Helpers;

namespace VityazReports.Models.CorpClients {
    public class AccountInfo {
        private readonly CommonMethods cm = new CommonMethods();
        public AccountInfo(Guid accountID, Guid? parentAccountID, string accountName, string parentAccountName, Guid guardObjectID, string guardObjectName, string guardObjectPay) {
            AccountID = accountID;
            ParentAccountID = parentAccountID;
            AccountName = accountName;
            ParentAccountName = parentAccountName;
            GuardObjectID = guardObjectID;
            GuardObjectName = guardObjectName;
            GuardObjectPay = guardObjectPay;
        }

        /// <summary>
        /// Идентификатор бизнес-партнера
        /// </summary>
        public Guid AccountID { get; set; }
        /// <summary>
        /// Идентификатор родительского бизнес-партнера
        /// </summary>
        public Guid? ParentAccountID { get; set; }
        /// <summary>
        /// Наименование бизнес-партнера
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Наименование родительского бизнес-партнера
        /// </summary>
        public string ParentAccountName { get; set; }
        /// <summary>
        /// Идентификатор охраняемого объекта
        /// </summary>
        public Guid GuardObjectID { get; set; }
        /// <summary>
        /// Наименование охраняемого объекта
        /// </summary>
        public string GuardObjectName { get; set; }
        /// <summary>
        /// Абонентская плата охраняемого объекта
        /// </summary>
        public string GuardObjectPay { get; set; }
        private int _Pay { get; set; }
        public int Pay {
            //get => cm.ParseDigit(GuardObjectPay);            
            get {
                if (string.IsNullOrEmpty(GuardObjectPay))
                    return 0;
                else
                    return cm.ParseDigit(GuardObjectPay);
            }
            set {
                if (string.IsNullOrEmpty(GuardObjectPay))
                    _Pay = 0;
                else
                    _Pay = cm.ParseDigit(GuardObjectPay);
            }
        }
    }
}
