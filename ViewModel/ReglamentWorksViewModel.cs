using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Data;
using VityazReports.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using VityazReports.Models.ReglamentWorks;
//using VityazReports.Models.ChangeCost;
using System.ComponentModel;
using VityazReports.Models;
using System.Windows.Threading;

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
                DetailFlyoutVisible = false;
                #region данным кодом мы можем получить историю изменений  по галочкам
                //if (SelectedReglamentWork == null)
                //    return;
                ////TODO: Перенести в get
                //Test = null;
                //App.Current.Dispatcher.Invoke((System.Action)delegate {
                //    ReglamentWorksDetailCollection.Clear();
                //});
                //NewGuardObjectHistory before = null;
                //NewGuardObjectHistory after = null;
                //DateTime start = DateTime.Parse(DateStart.ToShortDateString()).AddHours(-5);
                //DateTime end = DateTime.Parse(DateEnd.ToShortDateString()).AddHours(-5);
                //List<NewGuardObjectHistory> history = context.NewGuardObjectHistory.Where(x => x.ModifiedOn >= start && x.ModifiedOn <= end && x.NewGuardObjectId == SelectedReglamentWork.ObjectID).AsNoTracking().ToList();
                //if (history.Where(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null).Count() > 0) {
                //    var r = history.GroupBy(a => new { a.NewGuardObjectId, a.ModifiedBy, a.ModifiedOn.Date }).ToList();
                //    foreach (var item in r) {
                //        before = null;
                //        after = null;
                //        foreach (var i in item) {
                //            if (i.HistoryState == "Старый")
                //                before = i;
                //            else
                //                after = i;
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
                //    }
                //    if (ReglamentWorksDetailCollection.Count <= 0) {
                //        before = null;
                //        after = null;
                //        foreach (var item in history.Where(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null)) {
                //            if (item.HistoryState == "Старый")
                //                before = item;
                //            else
                //                after = item;
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
                //        if (ReglamentWorksDetailCollection.Count <= 0) {
                //            NewGuardObjectHistory first = history.FirstOrDefault(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null);
                //            if (first == null)
                //                return;
                //            if (first.NewRrOnOff != false)
                //                App.Current.Dispatcher.Invoke((System.Action)delegate {
                //                    ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                //                        UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                //                        DateChanged = first.ModifiedOn,
                //                        FieldChanged = "Ежем. рег. работы",
                //                        BeforeChanged = (!first.NewRrOnOff.Value).ToString(),
                //                        AfterChanged = first.NewRrOnOff.Value.ToString()
                //                    });
                //                });
                //            if (first.NewRrOs != false)
                //                App.Current.Dispatcher.Invoke((System.Action)delegate {
                //                    ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                //                        UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                //                        DateChanged = first.ModifiedOn,
                //                        FieldChanged = "Ежем. рег. работы (ОС)",
                //                        BeforeChanged = (!first.NewRrOs.Value).ToString(),
                //                        AfterChanged = first.NewRrOs.Value.ToString()
                //                    });
                //                });
                //            if (first.NewRrPs != false)
                //                App.Current.Dispatcher.Invoke((System.Action)delegate {
                //                    ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                //                        UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                //                        DateChanged = first.ModifiedOn,
                //                        FieldChanged = "Ежем. рег. работы (ПС)",
                //                        BeforeChanged = (!first.NewRrPs.Value).ToString(),
                //                        AfterChanged = first.NewRrPs.Value.ToString()
                //                    });
                //                });
                //            if (first.NewRrSkud != false)
                //                App.Current.Dispatcher.Invoke((System.Action)delegate {
                //                    ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                //                        UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                //                        DateChanged = first.ModifiedOn,
                //                        FieldChanged = "Ежем. рег. работы (СКУД)",
                //                        BeforeChanged = (!first.NewRrSkud.Value).ToString(),
                //                        AfterChanged = first.NewRrSkud.Value.ToString()
                //                    });
                //                });
                //            if (first.NewRrVideo != false)
                //                App.Current.Dispatcher.Invoke((System.Action)delegate {
                //                    ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                //                        UserChanged = context.SystemUserBase.FirstOrDefault(x => x.SystemUserId == first.ModifiedBy).FullName,
                //                        DateChanged = first.ModifiedOn,
                //                        FieldChanged = "Ежем. рег. работы (Видео)",
                //                        BeforeChanged = (!first.NewRrVideo.Value).ToString(),
                //                        AfterChanged = first.NewRrVideo.Value.ToString()
                //                    });
                //                });
                //        }
                //    }
                //    if (ReglamentWorksDetailCollection.Count > 0)
                //        DetailFlyoutVisible = true;
                //}
                #endregion

                //TODO: Добавить отбор по дате

                Dispatcher.CurrentDispatcher.Invoke((Action)delegate {
                    if (SelectedReglamentWork == null)
                        return;
                    ReglamentWorksDetailCollection.Clear();
                    var so = (from soeb in context.NewServiceorderExtensionBase
                              join sob in context.NewServiceorderBase on soeb.NewServiceorderId equals sob.NewServiceorderId
                              join goeb in context.NewGuardObjectExtensionBase on soeb.NewNumber equals goeb.NewObjectNumber
                              join smeb in context.NewServicemanExtensionBase on soeb.NewServicemanServiceorder equals smeb.NewServicemanId
                              join apv in context.AttributePicklistValue on soeb.NewCategory equals apv.Value
                              join ll in context.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                              join a in context.Attribute on apv.AttributeId equals a.AttributeId
                              join e in context.Entity on a.EntityId equals e.EntityId
                              where sob.DeletionStateCode == 0
                                  && sob.Statecode == 0
                                  && sob.Statuscode == 1
                                  && (goeb.NewPriostDate == null || goeb.NewObjDeleteDate == null || goeb.NewRemoveDate == null)
                                  && soeb.NewNumber == SelectedReglamentWork.ObjectNumber
                                  && goeb.NewObjectNumber == SelectedReglamentWork.ObjectNumber
                                  && soeb.NewDate.Value.Date >= DateStart.Date
                                  && soeb.NewDate.Value.Date <= DateEnd.Date
                                  && a.PhysicalName.Equals("New_category")
                                  && e.Name.Equals("New_serviceorder")
                              select new { soeb, smeb, ll }).AsNoTracking().Distinct().OrderByDescending(x => x.soeb.NewDate).ToList();
                    if (so == null)
                        return;
                    if (so.Count <= 0)
                        return;
                    foreach (var item in so)
                        ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail(item.soeb.NewDate.Value.AddHours(5), item.ll.Label, item.smeb.NewName, item.soeb.NewName, item.soeb.NewTechConclusion));

                    if (ReglamentWorksDetailCollection.Count > 0)
                        DetailFlyoutVisible = true;
                });
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
                              //join soeb in context.NewServiceorderExtensionBase on goeb.NewObjectNumber equals soeb.NewNumber
                              //join sob in context.NewServiceorderBase on soeb.NewServiceorderId equals sob.NewServiceorderId
                              where gob.Statecode == 0 && gob.Statuscode == 1 && gob.DeletionStateCode == 0
                                 && goeb.NewRemoveDate == null && goeb.NewPriostDate == null && goeb.NewObjDeleteDate == null &&
                                 goeb.NewRrOnOff == true
                                 //&& sob.DeletionStateCode == 0
                                 //     && sob.Statecode == 0
                                 //     && sob.Statuscode == 1
                              select new {
                                  NewObjectNumber = goeb.NewObjectNumber,
                                  NewName = goeb.NewName,
                                  NewAddress = goeb.NewAddress,
                                  NewRrOnOff = goeb.NewRrOnOff,
                                  NewRrOs = goeb.NewRrOs,
                                  NewRrPs = goeb.NewRrPs,
                                  NewRrVideo = goeb.NewRrVideo,
                                  NewRrSkud = goeb.NewRrSkud,
                                  NewGuardObjectId = goeb.NewGuardObjectId,
                                  //Category = soeb.NewCategory
                              }).AsNoTracking().Distinct().ToList();
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
                                //IsOrderExist = item.Category == 11 ? true : false
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
