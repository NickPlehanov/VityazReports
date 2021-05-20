namespace VityazReports.Models.ServiceOrders {
    public class ServiceOrdersModel {
        public ServiceOrdersModel(NewTest2ExtensionBase t2eb, NewServicemanExtensionBase smeb, string latitude, string longitude) {
            this.t2eb = t2eb;
            this.smeb = smeb;
            Latitude = latitude;
            Longitude = longitude;
        }

        public NewTest2ExtensionBase t2eb { get; set; }
        public NewServicemanExtensionBase smeb { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
