using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Data;
using VityazReports.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VityazReports.Models.CorpClients;
using System.Collections.ObjectModel;
using Notifications.Wpf;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace VityazReports.ViewModel {
    public class CorpClientsViewModel : BaseViewModel {
        NotificationManager notificationManager;
        /// <summary>
        /// Конструктор ViewModel-и
        /// </summary>
        public CorpClientsViewModel() {
            Loading = true;
            HeadOrganizationList = new ObservableCollection<AccountModel>();
            SubOrganizationList = new ObservableCollection<AccountModel>();
            GuardObjectsByAccountsList = new ObservableCollection<AccountInfo>();
            SelectedSubOrganization = new ObservableCollection<AccountModel>();
            SelectedGuardObjects = new ObservableCollection<AccountInfo>();
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
        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterFlyoutVisible = !FilterFlyoutVisible;
            });
        }
        private RelayCommand _ShowAllCommand;
        public RelayCommand ShowAllCommand {
            get => _ShowAllCommand ??= new RelayCommand(async obj => {
                Loading = true;
                FilterFlyoutVisible = false;

                HeadOrganizationList.Clear();
                SubOrganizationList.Clear();
                GuardObjectsByAccountsList.Clear();
                SelectedGuardObjects.Clear();
                SelectedHeadOrganization = null;
                SelectedSubOrganization.Clear();

                GetAllHeadOrganization.Execute(null);
                GetAllSubOrganization.Execute(null);
                GetGuardObjectByAccount.Execute(null);
                AnalyzeCommand.Execute(null);
                Loading = false;
                return;
            });
        }
        private RelayCommand _GetSelectedSubOrganization;
        public RelayCommand GetSelectedSubOrganization {
            get => _GetSelectedSubOrganization ??= new RelayCommand(async obj => {
                if (SelectedHeadOrganization == null)
                    return;
                SelectedGuardObjects.Clear();
                SelectedSubOrganization.Clear();
                SelectedOrgVisible = true;
                var s_sub = SubOrganizationList.Where(x => x.ParentAccountId == SelectedHeadOrganization.AccountId).ToList();
                if (s_sub == null)
                    return;
                foreach (var s_item in s_sub) {
                    SelectedSubOrganization.Add(s_item);
                    var s_go = GuardObjectsByAccountsList.Where(x => x.AccountID == s_item.AccountId).ToList();
                    if (s_go == null)
                        continue;
                    foreach (var s_go_item in s_go)
                        SelectedGuardObjects.Add(s_go_item);

                }
                var s_h_go = GuardObjectsByAccountsList.Where(x => x.AccountID == SelectedHeadOrganization.AccountId).ToList();
                if (s_h_go == null)
                    return;
                foreach (var s_h_go_item in s_h_go)
                    SelectedGuardObjects.Add(s_h_go_item);

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
                                     select new AccountModel(ab1.AccountId, null, ab1.Name, null, aeb1.NewEndDate.Value.AddHours(5), null, null, aeb1.NewFactAddrKladr, 0, sub.FullName)
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
                if (head_org_list.Count() <= 0) {
                    //TODO: сообщение пользователю
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = string.Format("Головных бизнес-партнеров не найдено"),
                        Type = NotificationType.Error
                    });
                    Loading = false;
                    return;
                }
                foreach (AccountModel item in head_org_list)
                    HeadOrganizationList.Add(item);
            });
        }
        /// <summary>
        /// Получаем из CRM и по списку HeadOrganizationList весь список дочерних организаций
        /// </summary>
        private RelayCommand _GetAllSubOrganization;
        public RelayCommand GetAllSubOrganization {
            get => _GetAllSubOrganization ??= new RelayCommand(async obj => {
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
                              //aeb1.NewEndDate == null && 
                              ab1.DeletionStateCode == 0
                                  && ab1.StateCode == 0
                                  && ab1.StatusCode == 1
                                  && ab1.ParentAccountId == item.AccountId
                              //&& aeb1.NewEndDate == null
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
                foreach (AccountModel item in sub_org_list) {
                    SubOrganizationList.Add(item);
                }
            });
        }
        /// <summary>
        /// Получаем список охраняемых объектов по бизнес-партнеру
        /// Сначало по головным, потом по дочерним
        /// </summary>
        private RelayCommand _GetGuardObjectByAccount;
        public RelayCommand GetGuardObjectByAccount {
            get => _GetGuardObjectByAccount ??= new RelayCommand(async obj => {
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
                        GuardObjectsByAccountsList.Add(o);
                }
                CalculateTotals.Execute(null);
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
        private RelayCommand _AnalyzeCommand;
        public RelayCommand AnalyzeCommand {
            get => _AnalyzeCommand ??= new RelayCommand(async obj => {
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
                foreach (var s in sorted) {
                    int sum = 0;
                    //var subs = SubOrganizationList.Where(x => x.AccountEndDate == null);
                    var subs = SelectedSubOrganization.Where(x => x.AccountEndDate == null);
                    foreach (var item in subs) {
                        //sum += GuardObjectsByAccountsList.Where(y => y.DateRemove > s.Value || y.DateRemove == null && y.AccountID == item.AccountId).Sum(x => x.Pay);
                        sum += SelectedGuardObjects.Where(y => y.DateRemove > s.Value || y.DateRemove == null && y.AccountID == item.AccountId).Sum(x => x.Pay);

                    }
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
        }
    }
}
