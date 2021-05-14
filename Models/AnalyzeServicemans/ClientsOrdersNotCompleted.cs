using System;
using System.Collections.Generic;
using System.Text;

namespace VityazReports.Models.AnalyzeServicemans {
    public class ClientsOrdersNotCompleted {
        public ClientsOrdersNotCompleted(int? objectNumber, string objectName, string objectAddress, int orderType, string orderTypeString, string reasonOrder, DateTime? income, DateTime? outgone, 
            double latitudeObject, double longitudeObject, double distanceIncome, 
            int result, string resultString, int reasonResult, string reasonResultString, string reasonComment, string conclusion) {
            ObjectNumber = objectNumber;
            ObjectName = objectName;
            ObjectAddress = objectAddress;
            OrderType = orderType;
            OrderTypeString = orderTypeString;
            ReasonOrder = reasonOrder;
            Income = income;
            Outgone = outgone;
            LatitudeObject = latitudeObject;
            LongitudeObject = longitudeObject;
            DistanceIncome = distanceIncome;
            Result = result;
            ResultString = resultString;
            ReasonResult = reasonResult;
            ReasonResultString = reasonResultString;
            ReasonComment = reasonComment;
            Conclusion = conclusion;
        }

        /// <summary>
        /// Номер объекта
        /// </summary>
        public int? ObjectNumber { get; set; }
        /// <summary>
        /// Название объекта
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// Адрес объекта
        /// </summary>
        public string ObjectAddress { get; set; }
        /// <summary>
        /// Код типа заявки
        /// VIP - 1
        /// Клиент - 2
        /// Сотрудник Витязя - 3
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// Текстовое описание типа заявки
        /// </summary>
        public string OrderTypeString { get; set; }
        /// <summary>
        /// Причина посещения (причина заявки технику)
        /// </summary>
        public string ReasonOrder { get; set; }
        /// <summary>
        /// Дата и время прихода
        /// </summary>
        public DateTime? Income { get; set; }
        /// <summary>
        /// Дата и время ухода
        /// </summary>
        public DateTime? Outgone { get; set; }
        /// <summary>
        /// Широта адреса объекта
        /// </summary>
        public double LatitudeObject { get; set; }
        /// <summary>
        /// Долгота адреса объекта
        /// </summary>
        public double LongitudeObject { get; set; }
        /// <summary>
        /// Расстояние между объектом и координатами отметки "пришёл"
        /// </summary>
        public double DistanceIncome { get; set; }
        /// <summary>
        /// Код результата выполнения заявки
        /// 1-выполнено
        /// 2-перенос
        /// 3-отмена
        /// </summary>
        public int Result { get; set; }
        /// <summary>
        /// Текстовое описание результата заявки
        /// </summary>
        public string ResultString { get; set; }
        /// <summary>
        /// Код причины отмены/переноса
        /// </summary>
        public int ReasonResult { get; set; }
        /// <summary>
        /// Текстовое описание причины отмены/переноса
        /// </summary>
        public string ReasonResultString { get; set; }
        /// <summary>
        /// Комментарий к приичне переноса/отмены
        /// </summary>
        public string ReasonComment { get; set; }
        /// <summary>
        /// Заключение техника по заявке
        /// </summary>
        public string Conclusion { get; set; }
    }
}
