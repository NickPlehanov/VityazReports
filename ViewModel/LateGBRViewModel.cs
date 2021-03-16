using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models.Lates;

namespace VityazReports.ViewModel {
    public class LateGBRViewModel : BaseViewModel {
        public LateGBRViewModel() {
            FilterFlyoutVisible = true;
            context = new MsCRMContext();
        }
        private readonly MsCRMContext context;
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

        private bool _Loading;
        public bool Loading {
            get => _Loading;
            set {
                _Loading = value;
                OnPropertyChanged(nameof(Loading));
            }
        }

        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterFlyoutVisible = !FilterFlyoutVisible;
            });
        }

        private ObservableCollection<LatesOutputModel> _LatesGBROutputList = new ObservableCollection<LatesOutputModel>();
        public ObservableCollection<LatesOutputModel> LatesGBROutputList {
            get => _LatesGBROutputList;
            set {
                _LatesGBROutputList = value;
                OnPropertyChanged(nameof(LatesGBROutputList));
            }
        }
        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, e) => {
                Loading = true;
                FilterFlyoutVisible = false;
                App.Current.Dispatcher.Invoke((System.Action)delegate {
                    LatesGBROutputList.Clear();
                });
                DateTime start1 = DateTime.Parse(DateStart.ToShortDateString()).AddHours(-5);
                DateTime end1 = DateTime.Parse(DateEnd.ToShortDateString()).AddHours(-5);
                var result = context.NewAlarmExtensionBase.Where(x => x.NewAlarmDt >= start1 && x.NewAlarmDt < end1).AsNoTracking().ToList();
                if (result != null)
                    if (result.Any()) {
                        foreach (var item1 in result) {
                            if (item1.NewArrival.HasValue && item1.NewDeparture.HasValue) {
                                if ((item1.NewArrival - item1.NewDeparture).Value.TotalMinutes >= 12) {
                                    //using (Vityaz_MSCRMContext context1 = new Vityaz_MSCRMContext()) {
                                    var andromeda = context.NewAndromedaExtensionBase.Where(x => x.NewAndromedaId == item1.NewAndromedaAlarm).AsNoTracking().ToList();
                                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                                        LatesGBROutputList.Add(new LatesOutputModel() {
                                            ObjectName = andromeda.FirstOrDefault(x => x.NewName != null).NewName,
                                            ObjectNumber = andromeda.FirstOrDefault().NewNumber,
                                            ObjectAddress = andromeda.FirstOrDefault().NewAddress,
                                            Os = item1.NewOnc,
                                            Ps = item1.NewPs,
                                            Trs = item1.NewTpc,
                                            Group = item1.NewGroup + 69,
                                            Alarm = item1.NewAlarmDt,
                                            Arrival = item1.NewArrival,
                                            Departure = item1.NewDeparture,
                                            Cancel = item1.NewCancel,
                                            Result = item1.NewName,
                                            Owner = item1.NewOwner,
                                            Police = item1.NewPolice,
                                            Act = item1.NewAct,
                                            Late = (item1.NewArrival - item1.NewDeparture).Value.ToString(),
                                            HourSort = item1.NewAlarmDt.Value.AddHours(5).Hour.ToString()
                                        });
                                    });
                                    //}
                                }
                            }
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
