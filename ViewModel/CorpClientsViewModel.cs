using System;
using System.Collections.Generic;
using VityazReports.Data;
using VityazReports.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VityazReports.Models.CorpClients;
using System.Collections.ObjectModel;
using Notifications.Wpf;
using LiveCharts;
using LiveCharts.Wpf;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

namespace VityazReports.ViewModel {
    public class CorpClientsViewModel : BaseViewModel {
        NotificationManager notificationManager;
        /// <summary>
        /// Конструктор ViewModel-и
        /// </summary>
        public CorpClientsViewModel() {
            HeadOrganizationList = new ObservableCollection<AccountModel>();
            FilteredHeadOrganizationList = new ObservableCollection<AccountModel>();
            SubOrganizationList = new ObservableCollection<AccountModel>();
            GuardObjectsByAccountsList = new ObservableCollection<AccountInfo>();
            SelectedSubOrganization = new ObservableCollection<AccountModel>();
            SelectedGuardObjects = new ObservableCollection<AccountInfo>();
            Reports = new ObservableCollection<ReportCorpClients>();
            Analyze = new ObservableCollection<AnalyzeModel>();
            notificationManager = new NotificationManager();
            FilterFlyoutVisible = true;

            Loading = false;
        }
        /// <summary>
        /// Свойство хранения контекста Црм
        /// </summary>
        private MsCRMContext _MsCrmContext;
        public MsCRMContext MsCrmContext {
            get => _MsCrmContext;
            set {
                _MsCrmContext = value;
                OnPropertyChanged(nameof(MsCrmContext));
            }
        }
        /// <summary>
        /// Коллекция для хранения списка головных организаций
        /// </summary>
        private ObservableCollection<AccountModel> _HeadOrganizationList;
        public ObservableCollection<AccountModel> HeadOrganizationList {
            get => _HeadOrganizationList;
            set {
                _HeadOrganizationList = value;
                OnPropertyChanged(nameof(HeadOrganizationList));
            }
        }

        private ObservableCollection<AccountModel> _FilteredHeadOrganizationList;
        public ObservableCollection<AccountModel> FilteredHeadOrganizationList {
            get => _FilteredHeadOrganizationList;
            set {
                _FilteredHeadOrganizationList = value;
                OnPropertyChanged(nameof(FilteredHeadOrganizationList));
            }
        }
        /// <summary>
        /// Коллекция для хранения списка дочерних организаций
        /// </summary>
        private ObservableCollection<AccountModel> _SubOrganizationList;
        public ObservableCollection<AccountModel> SubOrganizationList {
            get => _SubOrganizationList;
            set {
                _SubOrganizationList = value;
                OnPropertyChanged(nameof(SubOrganizationList));
            }
        }

        private ObservableCollection<AccountInfo> _GuardObjectsByAccountsList;
        public ObservableCollection<AccountInfo> GuardObjectsByAccountsList {
            get => _GuardObjectsByAccountsList;
            set {
                _GuardObjectsByAccountsList = value;
                OnPropertyChanged(nameof(GuardObjectsByAccountsList));
            }
        }
        /// <summary>
        /// Получение контекста Црм
        /// </summary>
        /// <returns></returns>
        private MsCRMContext GetMsCRMContext() {
            if (MsCrmContext == null)
                return new MsCRMContext();
            else
                return MsCrmContext;
        }
        /// <summary>
        /// Выбранный элемент типа Головная организация в сетке на форме
        /// </summary>
        private AccountModel _SelectedHeadOrganization;
        public AccountModel SelectedHeadOrganization {
            get => _SelectedHeadOrganization;
            set {
                _SelectedHeadOrganization = value;
                if (_SelectedHeadOrganization != null) {
                    GetSelectedSubOrganization.Execute(null);
                    notificationManager.Show(new NotificationContent {
                        Title = "Информация",
                        Message = string.Format("Выбрана орг., информация доступна на вкладках: Дочерние организации и Охр. объекты"),
                        Type = NotificationType.Information
                    });
                    AnalyzeCommand.Execute(null);
                }
                OnPropertyChanged(nameof(SelectedHeadOrganization));
            }
        }
        /// <summary>
        /// Храним список подчиненных организаций, относительно выбранной головной организации
        /// </summary>
        private ObservableCollection<AccountModel> _SelectedSubOrganization;
        public ObservableCollection<AccountModel> SelectedSubOrganization {
            get => _SelectedSubOrganization;
            set {
                _SelectedSubOrganization = value;
                OnPropertyChanged(nameof(SelectedSubOrganization));
            }
        }
        /// <summary>
        /// Храним список охраняемых объектов по головной и подиченным организациям
        /// </summary>
        private ObservableCollection<AccountInfo> _SelectedGuardObjects;
        public ObservableCollection<AccountInfo> SelectedGuardObjects {
            get => _SelectedGuardObjects;
            set {
                _SelectedGuardObjects = value;
                OnPropertyChanged(nameof(SelectedGuardObjects));
            }
        }
        /// <summary>
        /// Видимость индикиатора загрузки
        /// </summary>
        private bool _Loading;
        public bool Loading {
            get => _Loading;
            set {
                _Loading = value;
                OnPropertyChanged(nameof(Loading));
            }
        }
        /// <summary>
        /// Поле показа/закрытия окна фильтра
        /// </summary>
        private bool _FilterFlyoutVisible;
        public bool FilterFlyoutVisible {
            get => _FilterFlyoutVisible;
            set {
                _FilterFlyoutVisible = value;
                OnPropertyChanged(nameof(FilterFlyoutVisible));
            }
        }
        /// <summary>
        /// Поисковая строка в поле фильтра
        /// </summary>
        private string _SearchText;
        public string SearchText {
            get => _SearchText;
            set {
                _SearchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        private string _FilterValue;
        public string FilterValue {
            get => _FilterValue;
            set {
                _FilterValue = value;
                OnPropertyChanged(nameof(FilterValue));
            }
        }
        private bool _ChartFlyoutVisible;
        public bool ChartFlyoutVisible {
            get => _ChartFlyoutVisible;
            set {
                _ChartFlyoutVisible = value;
                OnPropertyChanged(nameof(ChartFlyoutVisible));
            }
        }
        private bool _SelectedOrgVisible;
        public bool SelectedOrgVisible {
            get => _SelectedOrgVisible;
            set {
                _SelectedOrgVisible = value;
                OnPropertyChanged(nameof(SelectedOrgVisible));
            }
        }

        private RelayCommand _ApplyFilter;
        public RelayCommand ApplyFilter {
            get => _ApplyFilter ??= new RelayCommand(async obj => {
                if (obj == null) {
                    if (FilteredHeadOrganizationList.Count() > 0) {
                        HeadOrganizationList = FilteredHeadOrganizationList;
                    }
                    return;
                }
                if (string.IsNullOrEmpty(obj.ToString())) {
                    if (FilteredHeadOrganizationList.Count() > 0) {
                        HeadOrganizationList = FilteredHeadOrganizationList;
                        return;
                    }
                    if (FilteredHeadOrganizationList.Count() == HeadOrganizationList.Count())
                        return;
                }
                FilteredHeadOrganizationList = HeadOrganizationList;
                if (!string.IsNullOrEmpty(FilterValue))
                    HeadOrganizationList = new ObservableCollection<AccountModel>(HeadOrganizationList.Where(x => x.AccountName.ToLower().Contains(FilterValue.ToLower())).ToList());
            });
        }
        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterFlyoutVisible = !FilterFlyoutVisible;
            });
        }
        private RelayCommand _ShowAllCommand;
        public RelayCommand ShowAllCommand {
            get => _ShowAllCommand ??= new RelayCommand(async obj => {
                App.Current.Dispatcher.Invoke((Action)delegate {
                    HeadOrganizationList.Clear();
                    SubOrganizationList.Clear();
                    GuardObjectsByAccountsList.Clear();
                    SelectedGuardObjects.Clear();
                    SelectedHeadOrganization = null;
                    SelectedSubOrganization.Clear();
                });

                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, e) => {

                    FilterFlyoutVisible = false;
                    Loading = true;

                    GetAllHeadOrganization.Execute(null);
                    GetAllSubOrganization.Execute(null);
                    GetGuardObjectByAccount.Execute(null);
                    AnalyzeCommand.Execute(null);

                };
                bw.RunWorkerCompleted += (s, e) => {
                    Loading = false;
                };
                bw.RunWorkerAsync();
            });
        }
        private RelayCommand _GetSelectedSubOrganization;
        public RelayCommand GetSelectedSubOrganization {
            get => _GetSelectedSubOrganization ??= new RelayCommand(async obj => {
                if (SelectedHeadOrganization == null)
                    return;
                App.Current.Dispatcher.Invoke((Action)delegate {
                    SelectedGuardObjects.Clear();
                    SelectedSubOrganization.Clear();
                });
                SelectedOrgVisible = true;
                var s_sub = SubOrganizationList.Where(x => x.ParentAccountId == SelectedHeadOrganization.AccountId).ToList();
                if (s_sub == null)
                    return;
                foreach (var s_item in s_sub) {
                    App.Current.Dispatcher.Invoke((Action)delegate {
                        SelectedSubOrganization.Add(s_item);
                    });
                    var s_go = GuardObjectsByAccountsList.Where(x => x.AccountID == s_item.AccountId).ToList();
                    if (s_go == null)
                        continue;
                    foreach (var s_go_item in s_go)
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            SelectedGuardObjects.Add(s_go_item);
                        });

                }
                var s_h_go = GuardObjectsByAccountsList.Where(x => x.AccountID == SelectedHeadOrganization.AccountId).ToList();
                if (s_h_go == null)
                    return;
                foreach (var s_h_go_item in s_h_go)
                    App.Current.Dispatcher.Invoke((Action)delegate {
                        SelectedGuardObjects.Add(s_h_go_item);
                    });

                //var s_go = GuardObjectsByAccountsList.Where(x => x.AccountID);
            });
        }
        /// <summary>
        /// Получаем из Црм, все организации, которые являются головными
        /// Пока что не учитываем, расторжена или нет. В последующем следует учитывать, является ли организация расторженой, а у неё есть организация без расторжения или охраняемый объект без расторжения
        /// TODO
        /// </summary>
        private RelayCommand _GetAllHeadOrganization;
        public RelayCommand GetAllHeadOrganization {
            get => _GetAllHeadOrganization ??= new RelayCommand(async obj => {
                //BackgroundWorker bw = new BackgroundWorker();
                //bw.DoWork += (s, e) => {
                //    Loading = true;

                MsCrmContext = GetMsCRMContext();
                List<AccountModel> head_org_list = new List<AccountModel>();
                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrWhiteSpace(SearchText))
                    head_org_list = (from ab1 in MsCrmContext.AccountBase
                                     join aeb1 in MsCrmContext.AccountExtensionBase on ab1.AccountId equals aeb1.AccountId
                                     join sub in MsCrmContext.SystemUserBase on ab1.OwningUser equals sub.SystemUserId
                                     where ab1.ParentAccountId == null
                                         //&& aeb1.NewEndDate == null
                                         && ab1.DeletionStateCode == 0
                                         && ab1.StateCode == 0
                                         && ab1.StatusCode == 1
                                     select new AccountModel(ab1.AccountId, null, ab1.Name, null, aeb1.NewEndDate.Value.AddHours(5), 0, 0, aeb1.NewFactAddrKladr, 0, sub.FullName)
                                             ).AsNoTracking().Distinct().ToList();
                else
                    head_org_list = (from ab1 in MsCrmContext.AccountBase
                                     join aeb1 in MsCrmContext.AccountExtensionBase on ab1.AccountId equals aeb1.AccountId
                                     join sub in MsCrmContext.SystemUserBase on ab1.OwningUser equals sub.SystemUserId
                                     where ab1.ParentAccountId == null
                                         //&& aeb1.NewEndDate == null
                                         && ab1.DeletionStateCode == 0
                                         && ab1.StateCode == 0
                                         && ab1.StatusCode == 1
                                         && ab1.Name.ToLower().Contains(SearchText.ToLower())
                                     select new AccountModel(ab1.AccountId, null, ab1.Name, null, aeb1.NewEndDate.Value.AddHours(5), 0, 0, aeb1.NewFactAddrKladr, 0, sub.FullName)
                                                 ).AsNoTracking().Distinct().ToList();
                //if (head_org_list.Count() <= 0) {
                //    //TODO: сообщение пользователю
                //    notificationManager.Show(new NotificationContent {
                //        Title = "Ошибка",
                //        Message = string.Format("Головных бизнес-партнеров не найдено"),
                //        Type = NotificationType.Error
                //    });
                //    //Loading = false;
                //    //return;
                //}
                foreach (AccountModel item in head_org_list)
                    App.Current.Dispatcher.Invoke((Action)delegate {
                        HeadOrganizationList.Add(item);
                    });
                //};
                //bw.RunWorkerCompleted += (s, e) => {
                //    Loading = false;
                //};
                //bw.RunWorkerAsync();
            });
        }
        /// <summary>
        /// Получаем из CRM и по списку HeadOrganizationList весь список дочерних организаций
        /// </summary>
        private RelayCommand _GetAllSubOrganization;
        public RelayCommand GetAllSubOrganization {
            get => _GetAllSubOrganization ??= new RelayCommand(async obj => {
                //BackgroundWorker bw = new BackgroundWorker();
                //bw.DoWork += (s, e) => {
                //    Loading = true;

                if (HeadOrganizationList.Count() <= 0) {
                    //TODO: сообщение пользователю
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = string.Format("Головных бизнес-партнеров не найдено"),
                        Type = NotificationType.Error
                    });
                    Loading = false;
                    return;
                }
                MsCrmContext = GetMsCRMContext();
                List<AccountModel> sub_org_list = new List<AccountModel>();
                foreach (AccountModel item in HeadOrganizationList) {
                    var el = (from ab1 in MsCrmContext.AccountBase
                              join aeb1 in MsCrmContext.AccountExtensionBase on ab1.AccountId equals aeb1.AccountId
                              join sub in MsCrmContext.SystemUserBase on ab1.OwningUser equals sub.SystemUserId
                              where
                              ab1.DeletionStateCode == 0
                                  && ab1.StateCode == 0
                                  && ab1.StatusCode == 1
                                  && ab1.ParentAccountId == item.AccountId
                              select new AccountModel(ab1.AccountId, item.AccountId, ab1.Name, item.AccountName, aeb1.NewEndDate.Value.AddHours(5), 0, 0, aeb1.NewFactAddrKladr, 0, sub.FullName)
                                         ).AsNoTracking().Distinct().ToList();
                    if (el.Count() <= 0)
                        continue;
                    sub_org_list.AddRange(el);
                    //SubOrganizationList.Add(new AccountModel(item.AccountId, null, item.AccountName, null, null, null, null, item.Address));
                }
                if (sub_org_list.Count() <= 0) {
                    //TODO: сообщение пользователю
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = string.Format("Дочерних бизнес-партнеров не найдено"),
                        Type = NotificationType.Error
                    });
                    Loading = false;
                    return;
                }
                foreach (AccountModel item in sub_org_list)
                    App.Current.Dispatcher.Invoke((Action)delegate {
                        SubOrganizationList.Add(item);
                        GetSubOrgBySubOrg.Execute(item);
                    });
                //};
                //bw.RunWorkerCompleted += (s, e) => {
                //    Loading = false;
                //};
                //bw.RunWorkerAsync();
            });
        }
        /// <summary>
        /// Команда получения дочерних организации, у дочерних организаций
        /// </summary>
        private RelayCommand _GetSubOrgBySubOrg;
        public RelayCommand GetSubOrgBySubOrg {
            get => _GetSubOrgBySubOrg ??= new RelayCommand(async obj => {
                if (obj == null)
                    return;
                if (!(obj is AccountModel suborg))
                    return;
                //var sub_sub = MsCrmContext.AccountBase.Where(x => x.ParentAccountId == suborg.AccountId).AsNoTracking().ToList();
                var sub_sub = (from ab1 in MsCrmContext.AccountBase
                               join aeb1 in MsCrmContext.AccountExtensionBase on ab1.AccountId equals aeb1.AccountId
                               join sub in MsCrmContext.SystemUserBase on ab1.OwningUser equals sub.SystemUserId
                               where
                               ab1.DeletionStateCode == 0
                                   && ab1.StateCode == 0
                                   && ab1.StatusCode == 1
                                   && ab1.ParentAccountId == suborg.AccountId
                               select new AccountModel(ab1.AccountId, suborg.ParentAccountId, ab1.Name, suborg.ParentAccountName, aeb1.NewEndDate.Value.AddHours(5), 0, 0, aeb1.NewFactAddrKladr, 0, sub.FullName)
                                         ).AsNoTracking().Distinct().ToList();
                if (sub_sub.Count() <= 0)
                    return;
                foreach (var item in sub_sub) {
                    SubOrganizationList.Add(item);
                    GetSubOrgBySubOrg.Execute(item);
                }
            });
        }

        private RelayCommand _GetGuardObjectsBySub;
        public RelayCommand GetGuardObjectsBySub {
            get => _GetGuardObjectsBySub ??= new RelayCommand(async obj => {

            });
        }
        /// <summary>
        /// Получаем список охраняемых объектов по бизнес-партнеру
        /// Сначало по головным, потом по дочерним
        /// </summary>
        private RelayCommand _GetGuardObjectByAccount;
        public RelayCommand GetGuardObjectByAccount {
            get => _GetGuardObjectByAccount ??= new RelayCommand(async obj => {
                //BackgroundWorker bw = new BackgroundWorker();
                //bw.DoWork += (s, e) => {
                //    Loading = true;

                MsCrmContext = GetMsCRMContext();
                //для начала получим охраняемые объекты у головных бизнес-партнеров
                foreach (AccountModel item in HeadOrganizationList) {
                    var orgs = (from goeb in MsCrmContext.NewGuardObjectExtensionBase
                                join gob in MsCrmContext.NewGuardObjectBase on goeb.NewGuardObjectId equals gob.NewGuardObjectId
                                join sub in MsCrmContext.SystemUserBase on gob.OwningUser equals sub.SystemUserId
                                where gob.DeletionStateCode == 0
                                  && gob.Statecode == 0
                                  && gob.Statuscode == 1
                                  && goeb.NewAccount == item.AccountId
                                  && goeb.NewRemoveDate == null
                                  && goeb.NewPriostDate == null
                                //select (goeb)
                                select new AccountInfo(item.AccountId, item.AccountId, item.AccountName, item.ParentAccountName,
                                goeb.NewGuardObjectId, goeb.NewName, goeb.NewAddress, goeb.NewMonthlypay, goeb.NewPriostDate, goeb.NewObjDeleteDate, sub.FullName)
                                ).AsNoTracking().Distinct().ToList();
                    //if (orgs.Count() <= 0)
                    //    continue;
                    //foreach (var _org in orgs) {
                    //    //if (item.ParentAccountId == null)
                    //    //    continue;
                    //    GuardObjectsByAccountsList.Add(new AccountInfo(item.AccountId, item.AccountId, item.AccountName, item.ParentAccountName, _org.NewGuardObjectId, _org.NewName, _org.NewAddress, _org.NewMonthlypay, _org.NewPriostDate, _org.NewObjDeleteDate,_org.full));
                    //}
                    foreach (var o in orgs)
                        GuardObjectsByAccountsList.Add(o);
                }
                //получаем охраняемые объекты у дочерних бизнес-партнеров
                foreach (AccountModel item in SubOrganizationList) {
                    var orgs = (from goeb in MsCrmContext.NewGuardObjectExtensionBase
                                join gob in MsCrmContext.NewGuardObjectBase on goeb.NewGuardObjectId equals gob.NewGuardObjectId
                                join sub in MsCrmContext.SystemUserBase on gob.OwningUser equals sub.SystemUserId
                                where gob.DeletionStateCode == 0
                                  && gob.Statecode == 0
                                  && gob.Statuscode == 1
                                  && goeb.NewAccount == item.AccountId
                                  && goeb.NewRemoveDate == null
                                  && goeb.NewPriostDate == null
                                //select (goeb)
                                select new AccountInfo(item.AccountId, item.AccountId, item.AccountName, item.ParentAccountName,
                                goeb.NewGuardObjectId, goeb.NewName, goeb.NewAddress, goeb.NewMonthlypay, goeb.NewPriostDate, goeb.NewObjDeleteDate, sub.FullName)
                                ).AsNoTracking().Distinct().ToList();
                    //if (orgs.Count() <= 0)
                    //    continue;
                    //foreach (var _org in orgs)
                    //    GuardObjectsByAccountsList.Add(new AccountInfo(item.AccountId, item.ParentAccountId, item.AccountName, item.ParentAccountName, _org.NewGuardObjectId, _org.NewName, 
                    //        _org.NewAddress, _org.NewMonthlypay, _org.NewPriostDate, _org.NewObjDeleteDate));
                    foreach (var o in orgs)
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            GuardObjectsByAccountsList.Add(o);
                        });
                }
                CalcTotals.Execute(null);
            });
        }

        private RelayCommand _OpenInCrmCommand;
        public RelayCommand OpenInCrmCommand {
            get => _OpenInCrmCommand ??= new RelayCommand(async obj => {
                bool k = false;
                if (obj == null)
                    return;
                if (string.IsNullOrEmpty(obj.ToString()))
                    return;
                Process.Start(new ProcessStartInfo("iexplore.exe", obj.ToString()) { UseShellExecute = true });
            });
        }
        private RelayCommand _CalcTotals;
        public RelayCommand CalcTotals {
            get => _CalcTotals ??= new RelayCommand(async obj => {
                if (HeadOrganizationList.Count() <= 0)
                    return;
                //if (SubOrganizationList.Count() <= 0)
                //    return;
                if (GuardObjectsByAccountsList.Count() <= 0)
                    return;
                foreach (var head in HeadOrganizationList) {
                    var subs = SubOrganizationList.Where(x => x.ParentAccountId == head.AccountId && x.AccountEndDate == null).ToList();
                    foreach (var sub_item in subs) {
                        var guard_objects = GuardObjectsByAccountsList.Where(x => (x.AccountID == sub_item.AccountId /*|| x.AccountID == head.AccountId*/) && x.DatePriost == null && x.DateRemove == null).ToList();
                        if (guard_objects.Count() <= 0)
                            continue;
                        sub_item.PaySumObjects = guard_objects.Sum(x => x.Pay);
                        sub_item.CountObjects = guard_objects.Count;
                    }
                    head.PaySumObjects += subs.Sum(x => x.PaySumObjects);
                    head.CountSubOrg = subs.Count();

                    var go = GuardObjectsByAccountsList.Where(x => x.AccountID == head.AccountId && x.DatePriost == null && x.DateRemove == null).ToList();
                    if (go.Count() <= 0)
                        continue;
                    head.PaySumObjects += go.Sum(x => x.Pay);                    
                    head.CountObjects = go.Count();                    
                }
                if (HeadOrganizationList.Count() == 1)
                    SelectedHeadOrganization = HeadOrganizationList[0];
                OnPropertyChanged(nameof(HeadOrganizationList));
            });
        }

        private RelayCommand _CalculateTotals;
        public RelayCommand CalculateTotals {
            get => _CalculateTotals ??= new RelayCommand(async obj => {
                if (GuardObjectsByAccountsList.Count() <= 0) {
                    //TODO: сообщение пользователю
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = string.Format("Список охраняемых объектов пустой"),
                        Type = NotificationType.Error
                    });
                    Loading = false;
                    return;
                }
                var groupped = GuardObjectsByAccountsList.Where(y => y.DatePriost == null && y.DateRemove == null).GroupBy(x => x.ParentAccountID).ToList();
                foreach (var item in groupped) {
                    if (item.Key == null)
                        continue;
                    var g2 = SubOrganizationList.Where(x => x.ParentAccountId == item.Key && x.AccountEndDate == null).ToList();
                    foreach (var it in g2) {
                        var g = GuardObjectsByAccountsList.Where(x => x.ParentAccountID == item.Key && x.DatePriost == null && x.DateRemove == null && x.AccountID == it.AccountId);
                        var g1 = HeadOrganizationList.FirstOrDefault(x => x.ParentAccountId == null && x.AccountId == item.Key && x.AccountEndDate == null);
                        //var g11 = SubOrganizationList.Where(x => x.ParentAccountId == item.Key);

                        if (g1 == null)
                            continue;
                        g1.PaySumObjects += g.Sum(x => x.Pay);
                        g1.CountObjects += g.Count();
                        g1.CountSubOrg = SubOrganizationList.Count(x => x.ParentAccountId == item.Key);

                        //var g2 = SubOrganizationList.Where(x => x.ParentAccountId == item.Key && x.AccountEndDate == null).ToList();
                        //foreach (var it in g2) {
                        //var _g2 = GuardObjectsByAccountsList.Where(x => x.AccountID == item.Key && x.DatePriost == null && x.DateRemove == null).ToList();
                        var _g22 = GuardObjectsByAccountsList.Where(x => x.AccountID == it.AccountId && x.DatePriost == null && x.DateRemove == null).ToList();
                        if (it == null)
                            continue;
                        it.PaySumObjects = _g22.Sum(x => x.Pay);
                        it.CountObjects = _g22.Count();
                        //}
                    }
                }
            });
        }

        private RelayCommand _CreateReport;
        public RelayCommand CreateReport {
            get => _CreateReport ??= new RelayCommand(async obj => {
                if (HeadOrganizationList.Count() <= 0)
                    return;
                if (SubOrganizationList.Count() <= 0)
                    return;
                Reports.Clear();
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = string.Format("Отчёт по корп.клиентам ({0})", DateTime.Now.Date.ToShortDateString());
                sfd.Filter = "CSV documents (.csv)|*.csv";
                DialogResult dr = sfd.ShowDialog();
                if (dr == DialogResult.OK) {
                    //using (StreamWriter sw = new StreamWriter(@"C:\Services\1.csv", false, System.Text.Encoding.UTF8)) {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8)) {
                        sw.WriteLine(string.Format("{0};{1};{2};{3};{4}", "Бизнес-партнёр", "Адрес", "Кол-во доч.орг.", "Аб. плата", "Ответственный"));
                        foreach (var head in HeadOrganizationList) {
                            sw.WriteLine(string.Format("{0};{1};{2};{3};{4}", head.AccountName, head.Address, head.CountSubOrg, head.CountObjects, head.Owner));
                        }
                    }
                    Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                }
            }, obj => HeadOrganizationList.Count() > 0);
        }

        private ObservableCollection<ReportCorpClients> _Reports;
        public ObservableCollection<ReportCorpClients> Reports {
            get => _Reports;
            set {
                _Reports = value;
                OnPropertyChanged(nameof(Reports));
            }
        }
        private ObservableCollection<AnalyzeModel> _Analyze;
        public ObservableCollection<AnalyzeModel> Analyze {
            get => _Analyze;
            set {
                _Analyze = value;
                OnPropertyChanged(nameof(Analyze));
            }
        }

        private SeriesCollection _ChartAnalytics;
        public SeriesCollection ChartAnalytics {
            get => _ChartAnalytics;
            set {
                _ChartAnalytics = value;
                OnPropertyChanged(nameof(ChartAnalytics));
            }
        }

        private RelayCommand _HelpCommand;
        public RelayCommand HelpCommand {
            get => _HelpCommand ??= new RelayCommand(async obj => {
                if (File.Exists(@"\\server-nass\Install\WORKPLACE\Инструкции\Корпоротивные клиенты.pdf"))
                    Process.Start(new ProcessStartInfo(@"\\server-nass\Install\WORKPLACE\Инструкции\Корпоротивные клиенты.pdf") { UseShellExecute = true });
                else
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Файл инструкции не найден",
                        Type = NotificationType.Error
                    });
            });
        }
        private RelayCommand _AnalyzeCommand;
        public RelayCommand AnalyzeCommand {
            get => _AnalyzeCommand ??= new RelayCommand(async obj => {
                App.Current.Dispatcher.Invoke((Action)delegate {
                    int order = 1;
                    if (SelectedHeadOrganization == null)
                        return;
                    //сортируем список охраняемых объектов по дате расторжения от ранней к старшей
                    //считаем сумму абонентских плат от даты из п.1.
                    Analyze.Clear();
                    ChartAnalytics = new SeriesCollection();
                    //if (GuardObjectsByAccountsList.Count() <= 0) {
                    if (SelectedGuardObjects.Count() <= 0) {
                        //TODO: сообщение пользователю
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = string.Format("Список охраняемых объектов пустой"),
                            Type = NotificationType.Error
                        });
                        Loading = false;
                        return;
                    }
                    //var sorted = GuardObjectsByAccountsList.Where(z => z.DateRemove != null).Select(y => y.DateRemove).OrderBy(x => x.Value).Distinct().ToList();
                    var sorted = SelectedGuardObjects.Where(z => z.DateRemove != null).Select(y => y.DateRemove).OrderBy(x => x.Value).Distinct().ToList();
                    sorted.Add(sorted.Min().Value.AddDays(-1));
                    sorted = sorted.OrderBy(x => x.Value).Distinct().ToList();
                    foreach (var s in sorted) {
                        int sum = 0;
                        //var subs = SubOrganizationList.Where(x => x.AccountEndDate == null);
                        var subs = SelectedSubOrganization.Where(x => x.AccountEndDate == null);
                        foreach (var item in subs) {
                            //sum += GuardObjectsByAccountsList.Where(y => y.DateRemove > s.Value || y.DateRemove == null && y.AccountID == item.AccountId).Sum(x => x.Pay);
                            sum += SelectedGuardObjects.Where(y => y.DateRemove > s.Value || y.DateRemove == null && y.AccountID == item.AccountId).Sum(x => x.Pay);
                        }
                        //if (subs.Count() <= 0) {
                        sum += SelectedGuardObjects.Where(y => y.DateRemove > s.Value || y.DateRemove == null && y.AccountID == SelectedHeadOrganization.AccountId).Sum(x => x.Pay);
                        //}
                        Analyze.Add(new AnalyzeModel(s.Value, sum, order));
                        order++;
                    }
                    ChartAnalytics = new SeriesCollection();
                    double[] ys1 = new double[Analyze.Count];
                    for (int i = 0; i < Analyze.Count; i++) {
                        ys1[i] = double.Parse(Analyze[i].MonthlyPay.ToString());
                    }
                    var s1 = new LineSeries() {
                        Title = "Размер аб. платы",
                        Values = new ChartValues<double>(ys1),

                    };
                    ChartAnalytics.Add(s1);
                    ChartFlyoutVisible = true;
                });
            });
        }
    }
}
