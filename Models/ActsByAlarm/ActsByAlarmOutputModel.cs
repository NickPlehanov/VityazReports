using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VityazReports.Models.ActsByAlarm {
    public class ActsByAlarmOutputModel {
        public Guid? ObjectID { get; set; }
        public int? ObjectNumber { get; set; }
        public string ObjectName { get; set; }
        public string ObjectAddress { get; set; }
        public string Curator { get; set; }

        //поля для отчёта по сработкам
        public bool? Act { get; set; }
        public bool? Police { get; set; }
        public bool? Owner { get; set; }
        public string DateSort { get; set; }
        public string HourSort { get; set; }

        [NotMapped]
        private DateTime? _Alarm;
        public DateTime? Alarm {
            get => _Alarm;
            set {
                if (value.HasValue)
                    _Alarm = value.Value.AddHours(5);
            }
        }
        [NotMapped]
        private DateTime? _Departure;
        public DateTime? Departure {//отправка
            get => _Departure;
            set {
                if (value.HasValue)
                    _Departure = value.Value.AddHours(5);
            }
        }
        [NotMapped]
        private DateTime? _Arrival;
        public DateTime? Arrival {//прибытие
            get => _Arrival;
            set {
                if (value.HasValue)
                    _Arrival = value.Value.AddHours(5);
            }
        }
        [NotMapped]
        private DateTime? _Cancel;
        public DateTime? Cancel {
            get => _Cancel;
            set {
                if (value.HasValue)
                    _Cancel = value.Value.AddHours(5);
            }
        }
        public string Result { get; set; }
        private bool? _Os { get; set; }
        public bool? Os {
            get => _Os;
            set {
                _Os = value.HasValue ? value : false;
            }
        }
        private bool? _Ps { get; set; }
        public bool? Ps {
            get => _Ps;
            set {
                _Ps = value.HasValue ? value : false;
            }
        }
        private bool? _Trs { get; set; }
        public bool? Trs {
            get => _Trs;
            set {
                _Trs = value.HasValue ? value : false;
            }
        }
        public int? Group { get; set; }
    }
}
