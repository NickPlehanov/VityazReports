using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models.UnmountDevices;

namespace VityazReports.ViewModel {
    public class UnmountDevicesViewModel : BaseViewModel {
        public UnmountDevicesViewModel() {
            FilterFlyoutVisible = true;
        }
        private readonly MsCRMContext context = new MsCRMContext();
        /// <summary>
        /// Управляет видимостью панели фильтра
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
        /// Фильтр. Дата начала
        /// </summary>
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
        /// <summary>
        /// Фильтр. Дата окончания
        /// </summary>
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
        /// <summary>
        /// Фильтр. Дата окончания
        /// </summary>
        private DateTime _DateRemove;
        public DateTime DateRemove {
            get {
                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
                if (_DateRemove == DateTime.MinValue)
                    return DateTime.Now;
                else
                    return DateTime.Parse(_DateRemove.ToShortDateString());
            }
            set {
                _DateRemove = value;
                OnPropertyChanged("DateRemove");
            }
        }
        /// <summary>
        /// Управляет видимостью индикатора загрузки
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
        /// Коллекция, для отображения результатов работы запроса
        /// </summary>
        private ObservableCollection<UnmountDevices> _UnmountDevicesList = new ObservableCollection<UnmountDevices>();
        public ObservableCollection<UnmountDevices> UnmountDevicesList {
            get => _UnmountDevicesList;
            set {
                _UnmountDevicesList = value;
                OnPropertyChanged(nameof(UnmountDevicesList));
            }
        }
        /// <summary>
        /// Управляет видимостью панели фильтра
        /// </summary>
        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterFlyoutVisible = !FilterFlyoutVisible;
            });
        }
        /// <summary>
        /// Команда запроса данных на основании введенных значений фильтра
        /// </summary>
        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {
                Dispatcher.CurrentDispatcher.Invoke((Action)delegate {
                    var udl = (from gob in context.NewGuardObjectBase
                               join goeb in context.NewGuardObjectExtensionBase on gob.NewGuardObjectId equals goeb.NewGuardObjectId
                               join ab in context.AccountBase on goeb.NewAccount equals ab.AccountId
                               join reb in context.NewRentDeviceExtensionBase on goeb.NewGuardObjectId equals reb.NewGuardObjectRentDevice
                               join deb in context.NewDeviceExtensionBase on reb.NewDeviceRentDevice equals deb.NewDeviceId
                               join soeb in context.NewServiceorderExtensionBase on goeb.NewObjectNumber equals soeb.NewNumber
                               where deb.NewName != null
                                 && soeb.NewResult == 1
                                 && soeb.NewIncome.Value.Date >= DateStart.Date
                                 && soeb.NewOutgone.Value.Date <= DateEnd.Date
                                 && soeb.NewName.ToLower().Contains("демонт")
                                 && goeb.NewRemoveDate.Value.Date >= DateRemove.Date
                                 && (goeb.NewPriostDate != null || goeb.NewObjDeleteDate != null || goeb.NewRemoveDate != null)
                               select new { goeb, ab, reb, deb }
                               //select new UnmountDevices(
                               //    goeb.NewObjectNumber,
                               //    goeb.NewName,
                               //    goeb.NewNamePult,
                               //    goeb.NewAddress,
                               //    ab.Name,
                               //    deb.NewName,
                               //    reb.NewPriceBase,
                               //    reb.NewQty,
                               //    reb.NewIsReturn
                               //)
                          ).AsNoTracking().OrderBy(x => x.goeb.NewObjectNumber).ToList();
                    if (udl == null)
                        return;
                    if (udl.Count <= 0)
                        return;
                    UnmountDevicesList.Clear();
                    foreach (var item in udl)
                        UnmountDevicesList.Add(new UnmountDevices(
                                   item.goeb.NewObjectNumber,
                                   item.goeb.NewName,
                                   item.goeb.NewNamePult,
                                   item.goeb.NewAddress,
                                   item.ab.Name,
                                   item.deb.NewName,
                                   item.reb.NewPriceBase,
                                   item.reb.NewQty,
                                   item.reb.NewIsReturn
                               ));
                    //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(UnmountDevicesList);
                    //PropertyGroupDescription groupDescription = new PropertyGroupDescription("DeviceName");
                    //view.GroupDescriptions.Add(groupDescription);
                    FilterFlyoutVisible = false;
                });
            });
        }
    }
}
