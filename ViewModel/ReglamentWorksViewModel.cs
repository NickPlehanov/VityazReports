using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Data;
using VityazReports.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using VityazReports.Models.ReglamentWorks;
using VityazReports.Models.ChangeCost;
using System.ComponentModel;

namespace VityazReports.ViewModel {
    public class ReglamentWorksViewModel : BaseViewModel {
        private readonly MsCRMContext context;
        private readonly Compare compare;
        private readonly CommonMethods cm;

        public ReglamentWorksViewModel() {
            context = new MsCRMContext();
            FilterFlyoutVisible = true;
            compare = new Compare();
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

        private ReglamentWorksOutputModel _SelectedReglamentWork;
        public ReglamentWorksOutputModel SelectedReglamentWork {
            get => _SelectedReglamentWork;
            set {
                _SelectedReglamentWork = value;
                GetDetailInfo.Execute(null);
                OnPropertyChanged(nameof(SelectedReglamentWork));
            }
        }

        private ObservableCollection<ReglamentWorksOutputModel> _ReglamentWorksList = new ObservableCollection<ReglamentWorksOutputModel>();
        public ObservableCollection<ReglamentWorksOutputModel> ReglamentWorksList {
            get => _ReglamentWorksList;
            set {
                _ReglamentWorksList = value;
                OnPropertyChanged(nameof(ReglamentWorksList));
            }
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

        private ObservableCollection<ReglamentWorksDetail> _ReglamentWorksDetailCollection = new ObservableCollection<ReglamentWorksDetail>();
        public ObservableCollection<ReglamentWorksDetail> ReglamentWorksDetailCollection {
            get => _ReglamentWorksDetailCollection;
            set {
                _ReglamentWorksDetailCollection = value;
                OnPropertyChanged(nameof(ReglamentWorksDetailCollection));
            }
        }

        private string _Test;
        public string Test {
            get => _Test;
            set {
                _Test = value;
                OnPropertyChanged(nameof(Test));
            }
        }

        private bool _DetailFlyoutVisible;
        public bool DetailFlyoutVisible {
            get => _DetailFlyoutVisible;
            set {
                _DetailFlyoutVisible = value;
                OnPropertyChanged(nameof(DetailFlyoutVisible));
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

        private RelayCommand _GetDetailInfo;
        public RelayCommand GetDetailInfo {
            get => _GetDetailInfo ??= new RelayCommand(async obj => {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, e) => {
                    Loading = true;
                    DetailFlyoutVisible = false;
                    #region данным кодом мы можем получить историю изменений  по галочкам
                    if (SelectedReglamentWork == null)
                        return;
                    //TODO: Перенести в get
                    Test = null;
                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                        ReglamentWorksDetailCollection.Clear();
                    });
                    NewGuardObjectHistory before = null;
                    NewGuardObjectHistory after = null;
                    DateTime start = DateTime.Parse(DateStart.ToShortDateString()).AddHours(-5);
                    DateTime end = DateTime.Parse(DateEnd.ToShortDateString()).AddHours(-5);
                    List<NewGuardObjectHistory> history = context.NewGuardObjectHistory.Where(x => x.ModifiedOn >= start && x.ModifiedOn <= end && x.NewGuardObjectId == SelectedReglamentWork.ObjectID).AsNoTracking().ToList();
                    if (history.Where(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null).Count() > 0) {
                        var r = history.GroupBy(a => new { a.NewGuardObjectId, a.ModifiedBy, a.ModifiedOn.Date }).ToList();
                        foreach (var item in r) {
                            before = null;
                            after = null;
                            foreach (var i in item) {
                                if (i.HistoryState == "Старый")
                                    before = i;
                                else
                                    after = i;
                                if (before != null && after != null) {
                                    List<Comparator> comparators = compare.CompareObject(before, after);
                                    if (comparators.Any(x => x.FieldName.Equals("NewRrOnOff") || x.FieldName.Equals("NewRrOs") || x.FieldName.Equals("NewRrPs") || x.FieldName.Equals("NewRrVideo") || x.FieldName.Equals("NewRrSkud"))) {
                                        //var hj = 0;
                                        foreach (var compr in comparators) {
                                            string FieldChanged = GetRealFieldName(compr.FieldName);
                                            if (!string.IsNullOrEmpty(FieldChanged)) {
                                                App.Current.Dispatcher.Invoke((System.Action)delegate {
                                                    ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                                        UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == after.ModifiedBy).FullName,
                                                        DateChanged = after.ModifiedOn,
                                                        //FieldChanged = compr.FieldName,
                                                        FieldChanged = FieldChanged,
                                                        BeforeChanged = compr.OldValue.ToString(),
                                                        AfterChanged = compr.NewValue.ToString()
                                                    });
                                                });
                                            }
                                        }
                                    }
                                    before = null;
                                    after = null;
                                }
                            }
                        }
                        if (ReglamentWorksDetailCollection.Count <= 0) {
                            before = null;
                            after = null;
                            foreach (var item in history.Where(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null)) {
                                if (item.HistoryState == "Старый")
                                    before = item;
                                else
                                    after = item;
                                if (before != null && after != null) {
                                    List<Comparator> comparators = compare.CompareObject(before, after);
                                    if (comparators.Any(x => x.FieldName.Equals("NewRrOnOff") || x.FieldName.Equals("NewRrOs") || x.FieldName.Equals("NewRrPs") || x.FieldName.Equals("NewRrVideo") || x.FieldName.Equals("NewRrSkud"))) {
                                        //var hj = 0;
                                        foreach (var compr in comparators) {
                                            string FieldChanged = GetRealFieldName(compr.FieldName);
                                            if (!string.IsNullOrEmpty(FieldChanged)) {
                                                App.Current.Dispatcher.Invoke((System.Action)delegate {
                                                    ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                                        UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == after.ModifiedBy).FullName,
                                                        DateChanged = after.ModifiedOn,
                                                        FieldChanged = FieldChanged,
                                                        BeforeChanged = compr.OldValue.ToString(),
                                                        AfterChanged = compr.NewValue.ToString()
                                                    });
                                                });
                                            }
                                        }
                                    }
                                    before = null;
                                    after = null;
                                }
                            }
                            if (ReglamentWorksDetailCollection.Count <= 0) {
                                NewGuardObjectHistory first = history.FirstOrDefault(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null);
                                if (first == null)
                                    return;
                                if (first.NewRrOnOff != false)
                                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                                        ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                            UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                                            DateChanged = first.ModifiedOn,
                                            FieldChanged = "Ежем. рег. работы",
                                            BeforeChanged = (!first.NewRrOnOff.Value).ToString(),
                                            AfterChanged = first.NewRrOnOff.Value.ToString()
                                        });
                                    });
                                if (first.NewRrOs != false)
                                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                                        ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                            UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                                            DateChanged = first.ModifiedOn,
                                            FieldChanged = "Ежем. рег. работы (ОС)",
                                            BeforeChanged = (!first.NewRrOs.Value).ToString(),
                                            AfterChanged = first.NewRrOs.Value.ToString()
                                        });
                                    });
                                if (first.NewRrPs != false)
                                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                                        ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                            UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                                            DateChanged = first.ModifiedOn,
                                            FieldChanged = "Ежем. рег. работы (ПС)",
                                            BeforeChanged = (!first.NewRrPs.Value).ToString(),
                                            AfterChanged = first.NewRrPs.Value.ToString()
                                        });
                                    });
                                if (first.NewRrSkud != false)
                                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                                        ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                            UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                                            DateChanged = first.ModifiedOn,
                                            FieldChanged = "Ежем. рег. работы (СКУД)",
                                            BeforeChanged = (!first.NewRrSkud.Value).ToString(),
                                            AfterChanged = first.NewRrSkud.Value.ToString()
                                        });
                                    });
                                if (first.NewRrVideo != false)
                                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                                        ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                            UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                                            DateChanged = first.ModifiedOn,
                                            FieldChanged = "Ежем. рег. работы (Видео)",
                                            BeforeChanged = (!first.NewRrVideo.Value).ToString(),
                                            AfterChanged = first.NewRrVideo.Value.ToString()
                                        });
                                    });
                            }
                        }
                        //if (t != null)
                        //    foreach (var compr in t.Where(x => x.FieldName.Equals("NewRrOnOff") || x.FieldName.Equals("NewRrOs") || x.FieldName.Equals("NewRrPs") || x.FieldName.Equals("NewRrVideo") || x.FieldName.Equals("NewRrSkud"))) {
                        //        string WhoChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == after.ModifiedBy).FullName;
                        //        Guid? CuratorId = context.NewGuardObjectExtensionBase.FirstOrDefault(x => x.NewGuardObjectId == after.NewGuardObjectId).NewCurator;
                        //        string curatorName = null;
                        //        if (CuratorId.HasValue) {
                        //            Guid _id = Guid.Empty;
                        //            if (Guid.TryParse(CuratorId.Value.ToString(), out _id)) {
                        //                curatorName = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == CuratorId).FullName;
                        //            }
                        //        }
                        //        DateTime? WhenChanged = after.ModifiedOn;
                        //        NewGuardObjectExtensionBase objectExtensionBase = context.NewGuardObjectExtensionBase.FirstOrDefault(x => x.NewGuardObjectId == after.NewGuardObjectId);
                        //        if (objectExtensionBase != null) {
                        //            App.Current.Dispatcher.Invoke((System.Action)delegate {
                        //                ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                        //                    UserChanged = WhoChanged,
                        //                    DateChanged = WhenChanged,
                        //                    //FieldChanged = compr.FieldName,
                        //                    FieldChanged = GetRealFieldName(compr.FieldName),
                        //                    BeforeChanged = compr.OldValue.ToString(),
                        //                    AfterChanged = compr.NewValue.ToString()
                        //                });
                        //            });
                        //        }
                        //    }
                        //}
                        //}
                        //if (ReglamentWorksDetailCollection.Count <= 0) {
                        //    foreach (var item in r) {
                        //        before = null;
                        //        after = null;
                        //        foreach (var ite in item) {
                        //            if (ite.HistoryState == "Старый")
                        //                before = ite;
                        //            else
                        //                after = ite;
                        //            if (before != null && after != null) {
                        //                List<Comparator> comparators = compare.CompareObject(before, after);
                        //                if (comparators.Any(x => x.FieldName.Equals("NewRrOnOff") || x.FieldName.Equals("NewRrOs") || x.FieldName.Equals("NewRrPs") || x.FieldName.Equals("NewRrVideo") || x.FieldName.Equals("NewRrSkud"))) {
                        //                    //var hj = 0;
                        //                    foreach (var compr in comparators) {
                        //                        string FieldChanged = GetRealFieldName(compr.FieldName);
                        //                        if (!string.IsNullOrEmpty(FieldChanged)) {
                        //                            App.Current.Dispatcher.Invoke((System.Action)delegate {
                        //                                ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                        //                                    UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == after.ModifiedBy).FullName,
                        //                                    DateChanged = after.ModifiedOn,
                        //                                    //FieldChanged = compr.FieldName,
                        //                                    FieldChanged = FieldChanged,
                        //                                    BeforeChanged = compr.OldValue.ToString(),
                        //                                    AfterChanged = compr.NewValue.ToString()
                        //                                });
                        //                            });
                        //                        }
                        //                    }
                        //                }
                        //                before = null;
                        //                after = null;
                        //            }
                        //        }


                        //foreach (var item in it.Where(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null).Distinct()) {
                        //    if (item.NewRrOnOff != null)
                        //        App.Current.Dispatcher.Invoke((System.Action)delegate {
                        //            ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                        //                UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == item.ModifiedBy).FullName,
                        //                DateChanged = item.ModifiedOn,
                        //                FieldChanged = "Ежем. рег. работы",
                        //                BeforeChanged = (!item.NewRrOnOff.Value).ToString(),
                        //                AfterChanged = (item.NewRrOnOff.Value).ToString()
                        //            });
                        //        });
                        //    if (item.NewRrOs != null)
                        //        App.Current.Dispatcher.Invoke((System.Action)delegate {
                        //            ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                        //                UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == item.ModifiedBy).FullName,
                        //                DateChanged = item.ModifiedOn,
                        //                FieldChanged = "Ежем. рег. работы (ОС)",
                        //                BeforeChanged = (!item.NewRrOnOff.Value).ToString(),
                        //                AfterChanged = (item.NewRrOnOff.Value).ToString()
                        //            });
                        //        });
                        //    if (item.NewRrPs != null)
                        //        App.Current.Dispatcher.Invoke((System.Action)delegate {
                        //            ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                        //                UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == item.ModifiedBy).FullName,
                        //                DateChanged = item.ModifiedOn,
                        //                FieldChanged = "Ежем. рег. работы (ПС)",
                        //                BeforeChanged = (!item.NewRrOnOff.Value).ToString(),
                        //                AfterChanged = (item.NewRrOnOff.Value).ToString()
                        //            });
                        //        });
                        //    if (item.NewRrSkud != null)
                        //        App.Current.Dispatcher.Invoke((System.Action)delegate {
                        //            ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                        //                UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == item.ModifiedBy).FullName,
                        //                DateChanged = item.ModifiedOn,
                        //                FieldChanged = "Ежем. рег. работы (СКУД)",
                        //                BeforeChanged = (!item.NewRrOnOff.Value).ToString(),
                        //                AfterChanged = (item.NewRrOnOff.Value).ToString()
                        //            });
                        //        });
                        //    if (item.NewRrVideo != null)
                        //        App.Current.Dispatcher.Invoke((System.Action)delegate {
                        //            ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                        //                UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == item.ModifiedBy).FullName,
                        //                DateChanged = item.ModifiedOn,
                        //                FieldChanged = "Ежем. рег. работы (Видео)",
                        //                BeforeChanged = (!item.NewRrOnOff.Value).ToString(),
                        //                AfterChanged = (item.NewRrOnOff.Value).ToString()
                        //            });
                        //        });
                        //}
                        //}
                        if (ReglamentWorksDetailCollection.Count > 0)
                            DetailFlyoutVisible = true;
                    }
                    #endregion
                };
                bw.RunWorkerCompleted += (s, e) => {
                    Loading = false;
                };
                bw.RunWorkerAsync();
            });
        }

        private string GetRealFieldName(string field) {
            if (string.IsNullOrEmpty(field))
                return null;
            switch (field) {
                case "NewRrOnOff": return "Ежем. рег. работы";
                case "NewRrOs": return "Ежем. рег. работы (ОС)";
                case "NewRrPs": return "Ежем. рег. работы (ПС)";
                case "NewRrVideo": return "Ежем. рег. работы (Видео)";
                case "NewRrSkud": return "Ежем. рег. работы (СКУД)";
                default: return null;
            }
        }

        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, e) => {
                    Loading = true;
                    FilterFlyoutVisible = false;
                    var rr = (from goeb in context.NewGuardObjectExtensionBase
                              join gob in context.NewGuardObjectBase on goeb.NewGuardObjectId equals gob.NewGuardObjectId
                              where gob.Statecode == 0 && gob.Statuscode == 1 && gob.DeletionStateCode == 0
                                 && goeb.NewRemoveDate == null && goeb.NewPriostDate == null && goeb.NewObjDeleteDate == null &&
                                 goeb.NewRrOnOff == true /*&& ( goeb.NewRrOs == true || goeb.NewRrPs == true || goeb.NewRrVideo == true || goeb.NewRrSkud == true)*/
                              select new {
                                  NewObjectNumber = goeb.NewObjectNumber,
                                  NewName = goeb.NewName,
                                  NewAddress = goeb.NewAddress,
                                  NewRrOnOff = goeb.NewRrOnOff,
                                  NewRrOs = goeb.NewRrOs,
                                  NewRrPs = goeb.NewRrPs,
                                  NewRrVideo = goeb.NewRrVideo,
                                  NewRrSkud = goeb.NewRrSkud,
                                  NewGuardObjectId = goeb.NewGuardObjectId
                              }).AsNoTracking().ToList();
                    if (rr != null)
                        App.Current.Dispatcher.Invoke((System.Action)delegate {
                            ReglamentWorksList.Clear();
                        });
                    foreach (var item in rr) {
                        App.Current.Dispatcher.Invoke((System.Action)delegate {
                            ReglamentWorksList.Add(new ReglamentWorksOutputModel() {
                                ObjectNumber = item.NewObjectNumber,
                                ObjectName = item.NewName,
                                ObjectAddress = item.NewAddress,
                                RrEveryMonth = item.NewRrOnOff,
                                RrOS = item.NewRrOs,
                                RrPS = item.NewRrPs,
                                RrVideo = item.NewRrVideo,
                                RrSkud = item.NewRrSkud,
                                ObjectID = item.NewGuardObjectId
                            });
                        });
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
