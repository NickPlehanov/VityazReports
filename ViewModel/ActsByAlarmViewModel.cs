using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models.ActsByAlarm;

namespace VityazReports.ViewModel {
    public class ActsByAlarmViewModel : BaseViewModel {
        private readonly MsCRMContext context;
        public ActsByAlarmViewModel() {
            context = new MsCRMContext();
            FilterFlyoutVisible = true;
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

        private bool _Loading;
        public bool Loading {
            get => _Loading;
            set {
                _Loading = value;
                OnPropertyChanged(nameof(Loading));
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

        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterFlyoutVisible = !FilterFlyoutVisible;
            });
        }

        private ObservableCollection<ActsByAlarmOutputModel> _ActsByAlarmOutputList = new ObservableCollection<ActsByAlarmOutputModel>();
        public ObservableCollection<ActsByAlarmOutputModel> ActsByAlarmOutputList {
            get => _ActsByAlarmOutputList;
            set {
                _ActsByAlarmOutputList = value;
                OnPropertyChanged(nameof(ActsByAlarmOutputList));
            }
        }

        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, e) => {
                    Loading = true;
                    DateTime start = DateTime.Parse(DateStart.ToShortDateString()).AddHours(-5);
                    DateTime end = DateTime.Parse(DateEnd.ToShortDateString()).AddHours(-5);
                    var result = context.NewAlarmExtensionBase.Where(x => x.NewAlarmDt >= start && x.NewAlarmDt < end && x.NewAct == true).AsNoTracking().ToList();
                    if (result != null)
                        if (result.Any()) {
                            foreach (var item in result) {
                                var andromeda = context.NewAndromedaExtensionBase.Where(x => x.NewAndromedaId == item.NewAndromedaAlarm).AsNoTracking().ToList();
                                App.Current.Dispatcher.Invoke((Action)delegate {
                                    ActsByAlarmOutputList.Add(new ActsByAlarmOutputModel() {
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
                                        DateSort = item.NewAlarmDt.Value.AddHours(5).Date.ToShortDateString()
                                    });
                                });
                            }
                        }
                };
                bw.RunWorkerCompleted += (s, e) => {
                    Loading = false;
                };
                bw.RunWorkerAsync();
            });
        }
    }
}
