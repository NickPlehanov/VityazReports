using System;

namespace VityazReports.Models.ServicemanOnMapViewModel {
    public class ServicemansPoints {
        public ServicemansPoints(int objectNumber, DateTime income, DateTime outcome, int order, double distance, string objectAddress) {
            ObjectNumber = objectNumber;
            Income = income;
            Outcome = outcome;
            Order = order;
            Distance = distance;
            ObjectAddress = objectAddress;
        }

        public ServicemansPoints(int objectNumber, DateTime income, DateTime outcome, int order, double distance, string objectAddress, double distanceA28ByIncome, double distanceA28ByOutcome) : 
            this(objectNumber, income, outcome, order, distance, objectAddress) {
            DistanceA28ByIncome = distanceA28ByIncome;
            DistanceA28ByOutcome = distanceA28ByOutcome;
        }

        public int ObjectNumber { get; set; }
        public string ObjectAddress { get; set; }
        public DateTime Income { get; set; }
        public DateTime Outcome { get; set; }
        public int Order { get; set; }
        public double Distance { get; set; }
        public double DistanceA28ByIncome { get; set; }
        public double DistanceA28ByOutcome { get; set; }
    }
}