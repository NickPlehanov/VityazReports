using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Helpers;

namespace VityazReports.ViewModel {
    public class ActsByAlarmViewModel : BaseViewModel {
        public ActsByAlarmViewModel() {

        }
        private DateTime _DateStart;
        public DateTime DateStart {
            get {
                if (_DateStart == DateTime.MinValue)
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                else
                    return _DateStart;
            }
            set {
                _DateStart = value;
                OnPropertyChanged("DateStart");
            }
        }

        private DateTime _DateEnd;
        public DateTime DateEnd {
            get {
                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
                if (_DateEnd == DateTime.MinValue)
                    return DateTime.Now;
                else
                    return DateTime.Parse(_DateEnd.ToShortDateString());
            }
            set {
                _DateEnd = value;
                OnPropertyChanged("DateEnd");
            }
        }
        private bool _FilterFlyoutVisible;
        public bool FilterFlyoutVisible {
            get => _FilterFlyoutVisible;
            set {
                _FilterFlyoutVisible = value;
                OnPropertyChanged(nameof(FilterFlyoutVisible));
            }
        }

        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {
                DateTime start = DateTime.Parse(DateStart.ToShortDateString()).AddHours(-5);
                DateTime end = DateTime.Parse(DateEnd.ToShortDateString()).AddHours(-5);
                var result = context.NewAlarmExtensionBase.Where(x => x.NewAlarmDt >= start && x.NewAlarmDt < end && x.NewAct == true);
                if (result != null)
                    if (result.Any()) {
                        foreach (var item in result) {
                            using (Vityaz_MSCRMContext context1 = new Vityaz_MSCRMContext()) {
                                var andromeda = context1.NewAndromedaExtensionBase.Where(x => x.NewAndromedaId == item.NewAndromedaAlarm).ToList();
                                App.Current.Dispatcher.Invoke((System.Action)delegate {
                                    Reports.Add(new Report() {
                                        ObjectName = andromeda.FirstOrDefault(x => x.NewName != null).NewName,
                                        ObjectNumber = andromeda.FirstOrDefault().NewNumber,
                                        ObjectAddress = andromeda.FirstOrDefault().NewAddress,
                                        Os = item.NewOnc,
                                        Ps = item.NewPs,
                                        Trs = item.NewTpc,
                                        Group = item.NewGroup + 69,
                                        Alarm = item.NewAlarmDt,
                                        Arrival = item.NewArrival,
                                        Departure = item.NewDeparture,
                                        Cancel = item.NewCancel,
                                        Result = item.NewName,
                                        Owner = item.NewOwner,
                                        Police = item.NewPolice,
                                        Act = item.NewAct,
                                        DateSort = item.NewAlarmDt.Value.Date.ToShortDateString()
                                    });
                                });
                            }
                        }
                    }
            });
        }
    }
}
