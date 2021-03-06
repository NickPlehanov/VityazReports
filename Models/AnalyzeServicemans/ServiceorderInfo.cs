using System;

namespace VityazReports.Models.AnalyzeServicemans {
    public class ServiceorderInfo {
        public ServiceorderInfo(string incomeLatitude, string incomeLongitude, string outgoneLatitude, string outgoneLongitude, int? number, int? category, string categoryName,
            string objectName, string name, string conclusion, int? result, string resultName, DateTime? income, DateTime? outgone, string address, int? whoInit, string whoInitString, string time, string techName
            , string andrlat, string andrlon) {
            IncomeLatitude = incomeLatitude;
            IncomeLongitude = incomeLongitude;
            OutgoneLatitude = outgoneLatitude;
            OutgoneLongitude = outgoneLongitude;
            Number = number;
            Category = category;
            CategoryName = categoryName;
            ObjectName = objectName;
            Name = name;
            Conclusion = conclusion;
            Result = result;
            ResultName = resultName;
            Income = income;
            Outgone = outgone;
            Address = address;
            WhoInit = whoInit;
            WhoInitString = whoInitString;
            Time = time;
            TechName = techName;
            Andr_lat = andrlat;
            Andr_lon = andrlon;
        }
        public ServiceorderInfo(int? numberObject, string addressObject, string nameObject, string reason, string time, DateTime income, DateTime outgone, string conclusion, int? timefrom, int? timeto) {
            Number = numberObject;
            Address = addressObject;
            ObjectName = nameObject;
            Name = reason;
            Time = time;
            Income = income;
            Outgone = outgone;
            Conclusion = conclusion;
            TimeFrom = timefrom;
            TimeTo = timeto;
        }

        /// <summary>
        /// Широта координаты прихода
        /// </summary>
        public string IncomeLatitude { get; set; }
        /// <summary>
        /// Долгота координаты прихода
        /// </summary>
        public string IncomeLongitude { get; set; }
        /// <summary>
        /// Широта координаты ухода
        /// </summary>
        public string OutgoneLatitude { get; set; }
        /// <summary>
        /// Долгота координаты ухода
        /// </summary>
        public string OutgoneLongitude { get; set; }
        /// <summary>
        /// Номер объекта
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// Код категории заявки технику
        /// </summary>
        public int? Category { get; set; }
        /// <summary>
        /// Наименование категории заявки технику
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Наименование объекта
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// Причина посещения заявки технику
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Заключение по заявке технику
        /// </summary>
        public string Conclusion { get; set; }
        /// <summary>
        /// Результат заявки технику
        /// </summary>
        public int? Result { get; set; }
        /// <summary>
        /// результат заявки технику
        /// </summary>
        public string ResultName { get; set; }
        /// <summary>
        /// Время прихода
        /// </summary>
        public DateTime? Income { get; set; }
        /// <summary>
        /// Время ухода
        /// </summary>
        public DateTime? Outgone { get; set; }
        /// <summary>
        /// Адрес охраняемого объекта
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Кто дал заявку
        /// </summary>
        public int? WhoInit { get; set; }
        /// <summary>
        /// Кто дал заявку
        /// </summary>
        public string WhoInitString { get; set; }
        /// <summary>
        /// Время заявки
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// Техник
        /// </summary>
        public string TechName { get; set; }
        public string Andr_lat { get; set; }
        public string Andr_lon { get; set; }
        /// <summary>
        /// Время (от)
        /// </summary>
        public int? TimeFrom { get; set; }
        /// <summary>
        /// Время (до)
        /// </summary>
        public int? TimeTo { get; set; }

        private DateTime? _IncomeReal { get; set; }
        public DateTime? IncomeReal {
            get {
                if (Income.HasValue)
                    return Income.Value.AddHours(5);
                else
                    return null;
            }
        }
        public DateTime? OutgoneReal {
            get {
                if (Outgone.HasValue)
                    return Outgone.Value.AddHours(5);
                else
                    return null;
            }
        }
    }
}
