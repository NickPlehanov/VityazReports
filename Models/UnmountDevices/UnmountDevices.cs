namespace VityazReports.Models.UnmountDevices {
    public class UnmountDevices {
        public UnmountDevices(int? objectNumber, string name, string nameForPult, string address, string accountName, string deviceName, decimal? devicePrice, int? deviceCount, bool? isReturn) {
            ObjectNumber = objectNumber;
            Name = name;
            NameForPult = nameForPult;
            Address = address;
            AccountName = accountName;
            DeviceName = deviceName;
            DevicePrice = devicePrice;
            DeviceCount = deviceCount;
            IsReturn = isReturn;
        }

        /// <summary>
        /// Номер объекта
        /// </summary>
        public int? ObjectNumber { get; set; }
        /// <summary>
        /// Наименование объекта
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Наименование объекта для пульта
        /// </summary>
        public string NameForPult { get; set; }
        /// <summary>
        /// Адрес в формате кладр
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Наименование юридического лица (бизнес-партнер)
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Наименование оборудования
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// Стоимость оборудования
        /// </summary>
        public decimal? DevicePrice { get; set; }
        /// <summary>
        /// Количество оборудования отданного в аренду
        /// </summary>
        public int? DeviceCount { get; set; }
        /// <summary>
        /// Возвращено ли оборудование
        /// </summary>
        public bool? IsReturn { get; set; }
    }
}
