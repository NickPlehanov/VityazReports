using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Data;
using VityazReports.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VityazReports.Models.CorpClients;
using System.Collections.ObjectModel;

namespace VityazReports.ViewModel {
    public class CorpClientsViewModel : BaseViewModel {
        /// <summary>
        /// Конструктор ViewModel-и
        /// </summary>
        public CorpClientsViewModel() {
            HeadOrganizationList = new ObservableCollection<AccountModel>();
            SubOrganizationList = new ObservableCollection<AccountModel>();
            GuardObjectsByAccountsList = new ObservableCollection<AccountInfo>();

            GetAllHeadOrganization.Execute(null);
            GetAllSubOrganization.Execute(null);
            GetGuardObjectByAccount.Execute(null);
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
        /// Получаем из Црм, все организации, которые являются головными
        /// Пока что не учитываем, расторжена или нет. В последующем следует учитывать, является ли организация расторженой, а у неё есть организация без расторжения или охраняемый объект без расторжения
        /// TODO
        /// </summary>
        private RelayCommand _GetAllHeadOrganization;
        public RelayCommand GetAllHeadOrganization {
            get => _GetAllHeadOrganization ??= new RelayCommand(async obj => {
                MsCrmContext = GetMsCRMContext();
                List<AccountModel> head_org_list = (from ab1 in MsCrmContext.AccountBase
                                                    join aeb1 in MsCrmContext.AccountExtensionBase on ab1.AccountId equals aeb1.AccountId
                                                    where ab1.ParentAccountId == null
                                                        && aeb1.NewEndDate == null
                                                        && ab1.DeletionStateCode == 0
                                                        && ab1.StateCode == 0
                                                        && ab1.StatusCode == 1
                                                    select new AccountModel(ab1.AccountId, null, ab1.Name, null, aeb1.NewEndDate)
                                         ).AsNoTracking().Distinct().ToList();
                if (head_org_list.Count() <= 0)
                    //TODO: сообщение пользователю
                    return;
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
                if (HeadOrganizationList.Count() <= 0)
                    //TODO: сообщение пользователю
                    return;
                MsCrmContext = GetMsCRMContext();
                List<AccountModel> sub_org_list = new List<AccountModel>();
                foreach (AccountModel item in HeadOrganizationList) {
                    var el = (from ab1 in MsCrmContext.AccountBase
                              join aeb1 in MsCrmContext.AccountExtensionBase on ab1.AccountId equals aeb1.AccountId
                              where ab1.ParentAccountId != null
                                  && aeb1.NewEndDate == null
                                  && ab1.DeletionStateCode == 0
                                  && ab1.StateCode == 0
                                  && ab1.StatusCode == 1
                                  && ab1.ParentAccountId == item.AccountId
                              select new AccountModel(ab1.AccountId, item.AccountId, ab1.Name, item.AccountName, aeb1.NewEndDate)
                                         ).AsNoTracking().Distinct().ToList();
                    if (el.Count() <= 0)
                        continue;
                    sub_org_list.AddRange(el);
                }
                if (sub_org_list.Count() <= 0)
                    //TODO:сообщение пользователю
                    return;
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
                                where gob.DeletionStateCode == 0
                                  && gob.Statecode == 0
                                  && gob.Statuscode == 1
                                  && goeb.NewAccount == item.AccountId
                                  && goeb.NewRemoveDate==null
                                  && goeb.NewPriostDate==null
                                select (goeb)
                                );
                    if (orgs.Count() <= 0)
                        continue;
                    foreach (var _org in orgs)
                        GuardObjectsByAccountsList.Add(new AccountInfo(item.AccountId, item.ParentAccountId, item.AccountName, item.ParentAccountName, _org.NewGuardObjectId, _org.NewName, _org.NewMonthlypay));
                }
                //получаем охраняемые объекты у дочерних бизнес-партнеров
                foreach (AccountModel item in SubOrganizationList) {
                    var orgs = (from goeb in MsCrmContext.NewGuardObjectExtensionBase
                                join gob in MsCrmContext.NewGuardObjectBase on goeb.NewGuardObjectId equals gob.NewGuardObjectId
                                where gob.DeletionStateCode == 0
                                  && gob.Statecode == 0
                                  && gob.Statuscode == 1
                                  && goeb.NewAccount == item.AccountId
                                select (goeb)
                                );
                    if (orgs.Count() <= 0)
                        continue;
                    foreach (var _org in orgs)
                        GuardObjectsByAccountsList.Add(new AccountInfo(item.AccountId, item.ParentAccountId, item.AccountName, item.ParentAccountName, _org.NewGuardObjectId, _org.NewName, _org.NewMonthlypay));
                }
                CalculateTotals.Execute(null);
            });
        }

        private RelayCommand _CalculateTotals;
        public RelayCommand CalculateTotals {
            get => _CalculateTotals ??= new RelayCommand(async obj => {
                if (GuardObjectsByAccountsList.Count() <= 0)
                    //TODO: Уведомление пользователя
                    return;
                var groupped = GuardObjectsByAccountsList.GroupBy(x => x.ParentAccountID).ToList();
                foreach (var item in groupped) {
                    if (item.Key == null)
                        continue;
                    var g = GuardObjectsByAccountsList.Where(x => x.ParentAccountID == item.Key);
                    var g1 = GuardObjectsByAccountsList.FirstOrDefault(x => x.AccountID == item.Key);
                    if (g1 == null)
                        continue;
                    g1.Pay = g.Sum(x => x.Pay);
                }
            });
        }
    }
}
