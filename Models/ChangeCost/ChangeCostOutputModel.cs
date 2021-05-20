using System;

namespace VityazReports.Models.ChangeCost {
    public class ChangeCostOutputModel {
        public ChangeCostOutputModel(string before, string after, string curator, DateTime? dateChanged, DateTime? dateStart, string whoChanged, string objectAddress, string objectName, int? objectNumber) {
            Before = before;
            After = after;
            Curator = curator;
            DateChanged = dateChanged;
            DateStart = dateStart;
            WhoChanged = whoChanged;
            ObjectAddress = objectAddress;
            ObjectName = objectName;
            ObjectNumber = objectNumber;
        }

        /// <summary>
        /// Хранит прошлое значение
        /// </summary>
        public string Before { get; set; }
        /// <summary>
        /// Хранит новое значение
        /// </summary>
        public string After { get; set; }
        /// <summary>
        /// Куратор по договору
        /// </summary>
        public string Curator { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime? DateChanged { get; set; }
        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime? DateStart { get; set; }
        /// <summary>
        /// Кто произвел изменение
        /// </summary>
        public string WhoChanged { get; set; }
        /// <summary>
        /// Адрес объекта
        /// </summary>
        public string ObjectAddress { get; set; }
        /// <summary>
        /// Наименование объекта
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// Номер объекта
        /// </summary>
        public int? ObjectNumber { get; set; }
    }
}
