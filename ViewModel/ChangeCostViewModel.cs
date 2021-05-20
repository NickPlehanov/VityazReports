using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models.ChangeCost;

namespace VityazReports.ViewModel {
    public class ChangeCostViewModel : BaseViewModel {
        private readonly MsCRMContext context;
        private readonly Compare compare;
        private readonly CommonMethods cm;
        public ChangeCostViewModel() {
            FilterFlyoutVisible = true;
            context = new MsCRMContext();
            compare = new Compare();
            cm = new CommonMethods();
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

        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterFlyoutVisible = !FilterFlyoutVisible;
            });
        }


        private ObservableCollection<ChangeCostOutputModel> _ChangeCostOutputList = new ObservableCollection<ChangeCostOutputModel>();
        public ObservableCollection<ChangeCostOutputModel> ChangeCostOutputList {
            get => _ChangeCostOutputList;
            set {
                _ChangeCostOutputList = value;
                OnPropertyChanged(nameof(ChangeCostOutputList));
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
        private RelayCommand _ViewTotalCommand;
        public RelayCommand ViewTotalCommand {
            get => _ViewTotalCommand ??= new RelayCommand(obj => {
                //изм. абонентской платы
                if (obj == null) {
                    //TotalManagers = new ObservableCollection<TotalManagers>();
                    int CountRecords = ChangeCostOutputList.Count;
                    var ChangeByUser = ChangeCostOutputList.GroupBy(x => x.WhoChanged);
                    int PlusCounter = 0;
                    int MinusCounter = 0;
                    float PlusSum = 0;
                    float MinusSum = 0;
                    //Todo: доделать общую сумму приходов/ расходов / общую
                    //float AllSum = 0;
                    foreach (var item in ChangeByUser) {
                        PlusCounter = 0;
                        MinusCounter = 0;
                        PlusSum = 0;
                        MinusSum = 0;
                        foreach (var i in item) {
                            if ((cm.ParseDigit(i.After) - cm.ParseDigit(i.Before)) > 0) {
                                PlusCounter++;
                                PlusSum += (cm.ParseDigit(i.After) - cm.ParseDigit(i.Before));
                            }
                            else {
                                MinusCounter++;
                                MinusSum += (cm.ParseDigit(i.After) - cm.ParseDigit(i.Before));
                            }
                        }
                        //TotalManagers.Add(new TotalManagers() {
                        //    ManagerName = item.Key.ToString(),
                        //    AllCountChanges = item.Count(),
                        //    MajorCountChanges = PlusCounter,
                        //    MinorCountChanges = MinusCounter,
                        //    MajorSumChanges = PlusSum,
                        //    MinorSumChanges = MinusSum,
                        //    DeltaSum = (PlusSum - MinusSum * (-1))
                        //});
                        MessageBox.Show(item.Key.ToString() + Environment.NewLine
                            + " Всего изменений: " + item.Count().ToString() + Environment.NewLine
                            + "Положительных: " + PlusCounter.ToString() + " на сумму: " + PlusSum.ToString() + Environment.NewLine
                            + "Отрицательных: " + MinusCounter.ToString() + " на сумму: " + MinusSum.ToString() + Environment.NewLine
                            + "Изменение: " + (PlusSum - MinusSum * (-1)).ToString()
                            );
                    }
                }
                else {
                    //TotalManagers = new ObservableCollection<TotalManagers>();
                    int CountRecords = ChangeCostOutputList.Count;
                    var ChangeByUser = ChangeCostOutputList.Where(x => x.WhoChanged == obj.ToString()).ToList();
                    int PlusCounter = 0;
                    int MinusCounter = 0;
                    float PlusSum = 0;
                    float MinusSum = 0;
                    //Todo: доделать общую сумму приходов/расходов/общую
                    //float AllSum = 0;
                    foreach (var item in ChangeByUser) {
                        //PlusCounter = 0;
                        //MinusCounter = 0;
                        //PlusSum = 0;
                        //MinusSum = 0;
                        if ((cm.ParseDigit(item.After) - cm.ParseDigit(item.Before)) > 0) {
                            PlusCounter++;
                            PlusSum += (cm.ParseDigit(item.After) - cm.ParseDigit(item.Before));
                        }
                        else {
                            MinusCounter++;
                            MinusSum += (cm.ParseDigit(item.After) - cm.ParseDigit(item.Before));
                        }
                    }
                    MessageBox.Show(obj.ToString() + Environment.NewLine
                        + "Всего изменений: " + ChangeByUser.Count.ToString() + Environment.NewLine
                        + "Положительных: " + PlusCounter.ToString() + " на сумму: " + PlusSum.ToString() + Environment.NewLine
                        + "Отрицательных: " + MinusCounter.ToString() + " на сумму: " + MinusSum.ToString() + Environment.NewLine
                        + "Разница: " + (PlusSum - MinusSum * (-1)).ToString()
                        );
                }

            });
        }

        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, e) => {
                    FilterFlyoutVisible = false;
                    Loading = true;
                    Models.NewGuardObjectHistory before = null;
                    Models.NewGuardObjectHistory after = null;
                    DateTime start = DateTime.Parse(DateStart.ToShortDateString()).AddHours(-5);
                    DateTime end = DateTime.Parse(DateEnd.ToShortDateString()).AddHours(-5);
                    List<Models.NewGuardObjectHistory> history = context.NewGuardObjectHistory.Where(x => x.ModifiedOn >= start && x.ModifiedOn <= end).ToList();
                    var r = history.GroupBy(a => new { a.NewGuardObjectId, a.ModifiedBy, DateTime = DateTime.Parse(a.ModifiedOn.ToString()) }).ToList();
                    foreach (var item in r) {
                        before = null;
                        after = null;
                        foreach (var i in item)
                            if (i.HistoryState == "Старый")
                                before = i;
                            else
                                after = i;
                        List<Comparator> t = compare.CompareObject(before, after);
                        if (t != null)
                            foreach (var compr in t.Where(x => x.FieldName.Equals("NewMonthlypay"))) {
                                string WhoChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == after.ModifiedBy).FullName;
                                Guid? CuratorId = context.NewGuardObjectExtensionBase.FirstOrDefault(x => x.NewGuardObjectId == after.NewGuardObjectId).NewCurator;
                                string curatorName = null;
                                if (CuratorId.HasValue) {
                                    Guid _id = Guid.Empty;
                                    if (Guid.TryParse(CuratorId.Value.ToString(), out _id)) {
                                        curatorName = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == CuratorId).FullName;
                                    }
                                }
                                DateTime? WhenChanged = after.ModifiedOn;
                                Models.NewGuardObjectExtensionBase objectExtensionBase = context.NewGuardObjectExtensionBase.FirstOrDefault(x => x.NewGuardObjectId == after.NewGuardObjectId);
                                if (objectExtensionBase != null)
                                    App.Current.Dispatcher.Invoke(delegate {
                                        ChangeCostOutputList.Add(new ChangeCostOutputModel(
                                            compr.OldValue.ToString(),
                                            compr.NewValue.ToString(),
                                            curatorName,
                                            WhenChanged,
                                            //objectExtensionBase.NewDateStart.Value.AddHours(5).Date,
                                            objectExtensionBase.NewDateStart.HasValue ? objectExtensionBase.NewDateStart.Value.AddHours(5).Date : DateTime.MinValue,
                                            WhoChanged,
                                            objectExtensionBase.NewAddress,
                                            objectExtensionBase.NewName,
                                            objectExtensionBase.NewObjectNumber
                                            ));
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
