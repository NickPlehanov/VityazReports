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

        private RelayCommand _GetDetailInfo;
        public RelayCommand GetDetailInfo {
            get => _GetDetailInfo ??= new RelayCommand(async obj => {
                #region данным кодом мы можем получить историю изменений  по галочкам
                if (SelectedReglamentWork == null)
                    return;

                //TODO: Перенести в get
                Test=null;
                ReglamentWorksDetailCollection.Clear();
                NewGuardObjectHistory before = null;
                NewGuardObjectHistory after = null;
                DateTime start = DateTime.Parse(DateStart.ToShortDateString()).AddHours(-5);
                DateTime end = DateTime.Parse(DateEnd.ToShortDateString()).AddHours(-5);
                List<NewGuardObjectHistory> history = context.NewGuardObjectHistory.Where(x => x.ModifiedOn >= start && x.ModifiedOn <= end && x.NewGuardObjectId == SelectedReglamentWork.ObjectID).AsNoTracking().ToList();
                if (history.Where(x => x.NewRrOnOff != null || x.NewRrOs != null || x.NewRrPs != null || x.NewRrSkud != null || x.NewRrVideo != null).Count() > 0) {
                    var r = history.GroupBy(a => new { a.NewGuardObjectId, a.ModifiedBy, DateTime = DateTime.Parse(a.ModifiedOn.ToString()) }).ToList();
                    foreach (var item in r) {
                        before = null;
                        after = null;
                        foreach (var i in item)
                            if (i.HistoryState == "Старый")
                                before = i;
                            else
                                after = i;
                        //if ((after.NewRrOnOff != null || after.NewRrOs != null || after.NewRrPs != null || after.NewRrSkud != null || after.NewRrVideo != null) &&
                        //(before.NewRrOnOff != null || before.NewRrOs != null || before.NewRrPs != null || before.NewRrSkud != null || before.NewRrVideo != null)) {
                        List<Comparator> t = compare.CompareObject(before, after);
                        if (t != null)
                            foreach (var compr in t.Where(x => x.FieldName.Equals("NewRrOnOff") || x.FieldName.Equals("NewRrOs") || x.FieldName.Equals("NewRrPs") || x.FieldName.Equals("NewRrVideo") || x.FieldName.Equals("NewRrSkud"))) {
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
                                NewGuardObjectExtensionBase objectExtensionBase = context.NewGuardObjectExtensionBase.FirstOrDefault(x => x.NewGuardObjectId == after.NewGuardObjectId);
                            if (objectExtensionBase != null) {
                                //App.Current.Dispatcher.Invoke((System.Action)delegate {
                                ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail() {
                                    UserChanged = WhoChanged,
                                    DateChanged = WhenChanged,
                                    FieldChanged = compr.FieldName,
                                    BeforeChanged = compr.OldValue.ToString(),
                                    AfterChanged = compr.NewValue.ToString()
                                });
                                Test = "111111111";
                            }
                                //});
                            }
                        //}
                    }
                }
                #endregion
            });
        }
        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {
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
                    ReglamentWorksList.Clear();
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
            });
        }
    }
}
