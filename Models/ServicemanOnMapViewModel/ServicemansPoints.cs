using System;

namespace VityazReports.Models.ServicemanOnMapViewModel {
    public class ServicemansPoints {
        public ServicemansPoints(int objectNumber, DateTime income, DateTime outcome, int order, double distance) {
            ObjectNumber = objectNumber;
            Income = income;
            Outcome = outcome;
            Order = order;
            Distance = distance;
        }

        public int ObjectNumber { get; set; }
        public DateTime Income { get; set; }
        public DateTime Outcome { get; set; }
        public int Order { get; set; }
        public double Distance { get; set; }
    }
}
