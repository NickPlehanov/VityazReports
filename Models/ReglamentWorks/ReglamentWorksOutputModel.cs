using System;

namespace VityazReports.Models.ReglamentWorks {
    public class ReglamentWorksOutputModel {
        public Guid? ObjectID { get; set; }
        public int? ObjectNumber { get; set; }
        public string ObjectName { get; set; }
        public string ObjectAddress { get; set; }

        private bool? _RrEveryMonth { get; set; }
        public bool? RrEveryMonth {
            get => _RrEveryMonth;
            set {
                _RrEveryMonth = value.HasValue ? value : false;
            }
        }
        private bool? _RrOS { get; set; }
        public bool? RrOS {
            get => _RrOS;
            set {
                _RrOS = value.HasValue ? value : false;
            }
        }
        private bool? _RrPS { get; set; }
        public bool? RrPS {
            get => _RrPS;
            set {
                _RrPS = value.HasValue ? value : false;
            }
        }
        private bool? _RrVideo { get; set; }
        public bool? RrVideo {
            get => _RrVideo;
            set {
                _RrVideo = value.HasValue ? value : false;
            }
        }
        private bool? _RrSkud { get; set; }
        public bool? RrSkud {
            get => _RrSkud;
            set {
                _RrSkud = value.HasValue ? value : false;
            }
        }
        public bool IsOrderExist { get; set; }
        public double? DaysAgoReglamentOrder { get; set; }
    }
}
