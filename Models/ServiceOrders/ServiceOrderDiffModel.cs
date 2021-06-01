using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.ServiceOrders {
    public class ServiceOrderDiffModel {
        public ServiceOrderDiffModel(Guid objectID, int objectNumber, DateTime? oldDate, DateTime? newDate, double? oldDays, double? newDay) {
            ObjectID = objectID;
            ObjectNumber = objectNumber;
            OldDate = oldDate;
            NewDate = newDate;
            OldDays = oldDays;
            NewDay = newDay;
        }
        public ServiceOrderDiffModel() {

        }

        public Guid ObjectID { get; set; }
        public int ObjectNumber { get; set; }
        public DateTime? OldDate { get; set; }
        public DateTime? NewDate { get; set; }
        public double? OldDays { get; set; }
        public double? NewDay { get; set; }
    }
}
