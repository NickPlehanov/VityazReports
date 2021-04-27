using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.AnalyzeServicemans {
    public class ServiceorderInfo {
        public ServiceorderInfo(string incomeLatitude, string incomeLongitude, string outgoneLatitude, string outgoneLongitude, int? number, int? category, int? categoryName, string objectName, string name, string conclusion, int result, string resultName, DateTime? income, DateTime? outgone, string address) {
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
        public int? CategoryName { get; set; }
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
        public int Result { get; set; }
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
    }
}
