using Microsoft.EntityFrameworkCore;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models.ReglamentWorks;

namespace VityazReports.ViewModel {
    public class ReglamentWorksViewModel : BaseViewModel {
        private readonly MsCRMContext context;
        //private readonly Compare compare;
        //private readonly CommonMethods cm;

        NotificationManager notificationManager = new NotificationManager();

        public ReglamentWorksViewModel() {
            context = new MsCRMContext();
            FilterFlyoutVisible = true;
            //compare = new Compare();
            IsTabSecurity = true;
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
                if (IsTabSecurity)
                    GetDetailInfo.Execute(null);
                else
                    GetDetailFireAlarmInfo.Execute(null);
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
        private ObservableCollection<ReglamentWorksOutputModel> _ReglamentWorksFireAlarmList = new ObservableCollection<ReglamentWorksOutputModel>();
        public ObservableCollection<ReglamentWorksOutputModel> ReglamentWorksFireAlarmList {
            get => _ReglamentWorksFireAlarmList;
            set {
                _ReglamentWorksList = value;
                OnPropertyChanged(nameof(ReglamentWorksFireAlarmList));
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

        private RelayCommand _HelpCommand;
        public RelayCommand HelpCommand {
            get => _HelpCommand ??= new RelayCommand(async obj => {
                if (File.Exists(@"\\server-nass\Install\WORKPLACE\Инструкции\Регламентные работы.pdf"))
                    Process.Start(new ProcessStartInfo(@"\\server-nass\Install\WORKPLACE\Инструкции\Регламентные работы.pdf") { UseShellExecute = true });
                //else
                //    NotificationManager.Show(new NotificationContent {
                //        Title = "Ошибка",
                //        Message = "Файл инструкции не найден",
                //        Type = NotificationType.Error
                //    });
            });
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
                    else
                        notificationManager.Show(new NotificationContent() { Type = NotificationType.Information, Title = "Информация", Message = "За выбранный период заявок не было" });
                });
            });
        }
        private RelayCommand _GetDetailFireAlarmInfo;
        public RelayCommand GetDetailFireAlarmInfo {
            get => _GetDetailFireAlarmInfo ??= new RelayCommand(async obj => {
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


                Dispatcher.CurrentDispatcher.Invoke((Action)delegate {
                    if (SelectedReglamentWork == null)
                        return;
                    ReglamentWorksDetailCollection.Clear();
                    var so = (from teb in context.NewTest2ExtensionBase
                              join tb in context.NewTest2Base on teb.NewTest2Id equals tb.NewTest2Id
                              join goeb in context.NewGuardObjectExtensionBase on teb.NewNumber equals goeb.NewObjectNumber
                              join smeb in context.NewServicemanExtensionBase on teb.NewServicemanServiceorderPs equals smeb.NewServicemanId
                              join apv in context.AttributePicklistValue on teb.NewCategory equals apv.Value
                              join ll in context.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                              join a in context.Attribute on apv.AttributeId equals a.AttributeId
                              join e in context.Entity on a.EntityId equals e.EntityId
                              where tb.DeletionStateCode == 0
                                  && tb.Statecode == 0
                                  && tb.Statuscode == 1
                                  && (goeb.NewPriostDate == null || goeb.NewObjDeleteDate == null || goeb.NewRemoveDate == null)
                                  && teb.NewNumber == SelectedReglamentWork.ObjectNumber
                                  //&& teb.NewNumber == SelectedReglamentWork.ObjectNumber
                                  && teb.NewDate.Value.Date >= DateStart.Date
                                  && teb.NewDate.Value.Date <= DateEnd.Date
                                  && a.PhysicalName.Equals("New_category")
                                  && e.Name.Equals("New_test2")
                              select new { teb, smeb, ll }).AsNoTracking().Distinct().OrderByDescending(x => x.teb.NewDate).ToList();
                    if (so == null)
                        return;
                    if (so.Count <= 0)
                        return;
                    foreach (var item in so)
                        ReglamentWorksDetailCollection.Add(new ReglamentWorksDetail(item.teb.NewDate.Value.AddHours(5), item.ll.Label, item.smeb.NewName, item.teb.NewName, item.teb.NewTechconclusion));

                    if (ReglamentWorksDetailCollection.Count > 0)
                        DetailFlyoutVisible = true;
                    else
                        notificationManager.Show(new NotificationContent() { Type = NotificationType.Information, Title = "Информация", Message = "За выбранный период заявок не было" });
                });
            });
        }
        /// <summary>
        /// true - вкладка охраны
        /// false - вкладка ПС
        /// </summary>
        private bool _IsTabSecurity;
        public bool IsTabSecurity {
            get => _IsTabSecurity;
            set {
                _IsTabSecurity = value;
                OnPropertyChanged(nameof(IsTabSecurity));
            }
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
                bw.DoWork += (s, e1) => {
                    Loading = true;
                    FilterFlyoutVisible = false;

                    //получаем список охр. объектов с регламентными работами
                    var guardObjectsWithReglament = (from goeb in context.NewGuardObjectExtensionBase
                                                     join gob in context.NewGuardObjectBase on goeb.NewGuardObjectId equals gob.NewGuardObjectId
                                                     where gob.Statecode == 0
                                                         && gob.Statuscode == 1
                                                         && gob.DeletionStateCode == 0
                                                         && goeb.NewRemoveDate == null
                                                         && goeb.NewPriostDate == null
                                                         && goeb.NewObjDeleteDate == null
                                                         && goeb.NewRrOnOff == true
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
                                                     }
                                                     ).AsNoTracking().Distinct().ToList();
                    if (guardObjectsWithReglament == null)
                        return;
                    App.Current.Dispatcher.Invoke((System.Action)delegate {
                        ReglamentWorksList.Clear();
                        ReglamentWorksFireAlarmList.Clear();
                    });
                    foreach (var item in guardObjectsWithReglament) {
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
                                      && soeb.NewNumber == item.NewObjectNumber
                                      && goeb.NewObjectNumber == item.NewObjectNumber
                                      && a.PhysicalName.Equals("New_category")
                                      && e.Name.Equals("New_serviceorder")
                                      && goeb.NewGuardObjectId == item.NewGuardObjectId
                                      && soeb.NewCategory == 14
                                  select new { soeb, smeb, ll }).AsNoTracking().Distinct().OrderByDescending(x => x.soeb.NewDate).ToList();
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
                                ObjectID = item.NewGuardObjectId,
                                IsOrderExist = so.Where(x => x.soeb.NewDate.Value.Date >= DateStart.Date && x.soeb.NewDate.Value.Date <= DateEnd.Date).Count() > 0,
                                DaysAgoReglamentOrder = so.Count > 0 ?
                                so.OrderByDescending(x => x.soeb.NewDate).First().soeb.NewDate.Value.AddHours(5) < DateTime.Now ?
                                    (-1) * Math.Round((DateTime.Now - so.OrderByDescending(x => x.soeb.NewDate).First().soeb.NewDate.Value.AddHours(5)).TotalDays, 0) :
                                    Math.Round(((so.Where(x => x.soeb.NewDate >= DateTime.Now).OrderBy(x => x.soeb.NewDate).FirstOrDefault().soeb.NewDate.Value.AddHours(5) - DateTime.Now).TotalDays), 0) :
                                double.NaN
                            }); ;
                        });

                        //И сразу для ПС
                        var fa = (from teb in context.NewTest2ExtensionBase
                                  join tb in context.NewTest2Base on teb.NewTest2Id equals tb.NewTest2Id
                                  join goeb in context.NewGuardObjectExtensionBase on teb.NewNumber equals goeb.NewObjectNumber
                                  join smeb in context.NewServicemanExtensionBase on teb.NewServicemanServiceorderPs equals smeb.NewServicemanId
                                  join apv in context.AttributePicklistValue on teb.NewCategory equals apv.Value
                                  join ll in context.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                                  join a in context.Attribute on apv.AttributeId equals a.AttributeId
                                  join e in context.Entity on a.EntityId equals e.EntityId
                                  where tb.DeletionStateCode == 0
                                      && tb.Statecode == 0
                                      && tb.Statuscode == 1
                                      && (goeb.NewPriostDate == null || goeb.NewObjDeleteDate == null || goeb.NewRemoveDate == null)
                                      && teb.NewNumber == item.NewObjectNumber
                                      && goeb.NewObjectNumber == item.NewObjectNumber
                                      && a.PhysicalName.Equals("New_category")
                                      && e.Name.Equals("New_test2")
                                      && goeb.NewGuardObjectId == item.NewGuardObjectId
                                      && teb.NewCategory == 1
                                      && teb.NewDate != null
                                  select new { teb, smeb, ll }).AsNoTracking().Distinct().OrderByDescending(x => x.teb.NewDate).ToList();
                        App.Current.Dispatcher.Invoke((System.Action)delegate {
                            ReglamentWorksFireAlarmList.Add(new ReglamentWorksOutputModel() {
                                ObjectNumber = item.NewObjectNumber,
                                ObjectName = item.NewName,
                                ObjectAddress = item.NewAddress,
                                RrEveryMonth = item.NewRrOnOff,
                                RrOS = item.NewRrOs,
                                RrPS = item.NewRrPs,
                                RrVideo = item.NewRrVideo,
                                RrSkud = item.NewRrSkud,
                                ObjectID = item.NewGuardObjectId,
                                //IsOrderExist = fa.Where(x => x.teb.NewDate.Value.Date >= DateStart.Date && x.teb.NewDate.Value.Date <= DateEnd.Date).Count() > 0,                                
                                IsOrderExist = fa.Where(x => x.teb.NewDate.Value.Date >= DateStart.Date && x.teb.NewDate.Value.Date <= DateEnd.Date).Count() > 0,
                                DaysAgoReglamentOrder = fa.Count > 0 ?
                                fa.OrderByDescending(x => x.teb.NewDate).First().teb.NewDate.Value.AddHours(5) < DateTime.Now ?
                                    (-1) * Math.Round((DateTime.Now - fa.OrderByDescending(x => x.teb.NewDate).First().teb.NewDate.Value.AddHours(5)).TotalDays, 0) :
                                    Math.Round(((fa.Where(x => x.teb.NewDate >= DateTime.Now).OrderBy(x => x.teb.NewDate).FirstOrDefault().teb.NewDate.Value.AddHours(5) - DateTime.Now).TotalDays), 0) :
                                double.NaN
                            }); ;
                        });
                    }
                };
                bw.RunWorkerCompleted += (s, e1) => {
                    Loading = false;
                };
                bw.RunWorkerAsync();
            });
        }
    }
}
