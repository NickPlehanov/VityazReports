using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.EntityFrameworkCore;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models;
using VityazReports.Models.ServiceOrders;

namespace VityazReports.ViewModel {
    public class ServiceOrdersViewModel : BaseViewModel {
        private MsCRMContext msCRMContext = new MsCRMContext();
        NotificationManager notificationManager;
        public ServiceOrdersViewModel() {
            InitializeMapControl.Execute(null);

            FilterDate = NewServiceOrderDate = OrderDate = DateTime.Now;
            //FlyoutFilterOpened = true;

            SelectedMarkers = new ObservableCollection<GMapMarker>();
            ServicemanList = new ObservableCollection<NewServicemanExtensionBase>();
            ServiceOrdersList = new ObservableCollection<ServiceOrdersModel>();
            SelectedOrders = new ObservableCollection<ServiceOrdersModel>();
            GetServicemansCommand.Execute(null);

            notificationManager = new NotificationManager();

            //ShowServiceOrderOnMapCommand.Execute(null);

        }
        /// <summary>
        /// На какое число создаются заявки
        /// </summary>
        private DateTime _OrderDate;
        public DateTime OrderDate {
            get => _OrderDate;
            set {
                _OrderDate = value;
                OnPropertyChanged(nameof(OrderDate));
            }
        }
        /// <summary>
        /// Контрол карты
        /// </summary>
        private GMapControl _gmaps_contol;
        public GMapControl gmaps_contol {
            get => _gmaps_contol;
            set {
                _gmaps_contol = value;
                OnPropertyChanged(nameof(gmaps_contol));
            }
        }
        /// <summary>
        /// Определяет видимость плавающей области "фильтр"
        /// </summary>
        private bool _FlyoutFilterOpened;
        public bool FlyoutFilterOpened {
            get => _FlyoutFilterOpened;
            set {
                _FlyoutFilterOpened = value;
                OnPropertyChanged(nameof(FlyoutFilterOpened));
            }
        }

        private bool _FlyoutServicemanListOpened;
        public bool FlyoutServicemanListOpened {
            get => _FlyoutServicemanListOpened;
            set {
                _FlyoutServicemanListOpened = value;
                OnPropertyChanged(nameof(FlyoutServicemanListOpened));
            }
        }
        /// <summary>
        /// Храним значение выбранной даты для заявок технику в плавающей области "фильтр"
        /// </summary>
        private DateTime _FilterDate;
        public DateTime FilterDate {
            get => _FilterDate;
            set {
                _FilterDate = value;
                if (SelectedMarkers != null)
                    SelectedMarkers.Clear();
                SelectedServiceman = null;
                //ShowServiceOrderOnMapCommand.Execute(null);
                OnPropertyChanged(nameof(FilterDate));
            }
        }

        private bool _FlyoutNewServiceOrder;
        public bool FlyoutNewServiceOrder {
            get => _FlyoutNewServiceOrder;
            set {
                _FlyoutNewServiceOrder = value;
                OnPropertyChanged(nameof(FlyoutNewServiceOrder));
            }
        }
        private ObservableCollection<GMapMarker> _SelectedMarkers;
        public ObservableCollection<GMapMarker> SelectedMarkers {
            get => _SelectedMarkers;
            set {
                _SelectedMarkers = value;
                OnPropertyChanged(nameof(SelectedMarkers));
            }
        }

        private ObservableCollection<NewServicemanExtensionBase> _ServicemanList;
        public ObservableCollection<NewServicemanExtensionBase> ServicemanList {
            get => _ServicemanList;
            set {
                _ServicemanList = value;
                OnPropertyChanged(nameof(ServicemanList));
            }
        }
        /// <summary>
        /// Получем контекст данных ЦРМ
        /// </summary>
        /// <returns>Контекст данных</returns>
        private MsCRMContext GetMsCRMContext() {
            if (msCRMContext == null)
                return new MsCRMContext();
            else
                return msCRMContext;
        }

        private NewServicemanExtensionBase _SelectedServiceman;
        public NewServicemanExtensionBase SelectedServiceman {
            get => _SelectedServiceman;
            set {
                _SelectedServiceman = value;
                OnPropertyChanged(nameof(SelectedServiceman));
            }
        }

        private ObservableCollection<ServiceOrdersModel> _ServiceOrdersList;
        public ObservableCollection<ServiceOrdersModel> ServiceOrdersList {
            get => _ServiceOrdersList;
            set {
                _ServiceOrdersList = value;
                OnPropertyChanged(nameof(ServiceOrdersList));
            }
        }

        private ObservableCollection<ServiceOrdersModel> _SelectedOrders;
        public ObservableCollection<ServiceOrdersModel> SelectedOrders {
            get => _SelectedOrders;
            set {
                _SelectedOrders = value;
                OnPropertyChanged(nameof(SelectedOrders));
            }
        }

        private ServiceOrdersModel _SelectedServiceOrder;
        public ServiceOrdersModel SelectedServiceOrder {
            get => _SelectedServiceOrder;
            set {
                _SelectedServiceOrder = value;
                OnPropertyChanged(nameof(SelectedServiceOrder));
            }
        }

        private DateTime _NewServiceOrderDate;
        public DateTime NewServiceOrderDate {
            get => _NewServiceOrderDate;
            set {
                _NewServiceOrderDate = value;
                OnPropertyChanged(nameof(NewServiceOrderDate));
            }
        }

        private string _NewServiceOrderObjectNumber;
        public string NewServiceOrderObjectNumber {
            get => _NewServiceOrderObjectNumber;
            set {
                _NewServiceOrderObjectNumber = value;
                OnPropertyChanged(nameof(NewServiceOrderObjectNumber));
            }
        }

        private string _NewServiceOrderDescription;
        public string NewServiceOrderDescription {
            get => _NewServiceOrderDescription;
            set {
                _NewServiceOrderDescription = value;
                OnPropertyChanged(nameof(NewServiceOrderDescription));
            }
        }

        private NewServicemanExtensionBase _NewServiceOrderSelectedServiceman;
        public NewServicemanExtensionBase NewServiceOrderSelectedServiceman {
            get => _NewServiceOrderSelectedServiceman;
            set {
                _NewServiceOrderSelectedServiceman = value;
                OnPropertyChanged(nameof(NewServiceOrderSelectedServiceman));
            }
        }


        private bool _IsShowGuardObjectsWithDownTimeReglaments;
        public bool IsShowGuardObjectsWithDownTimeReglaments {
            get => _IsShowGuardObjectsWithDownTimeReglaments;
            set {
                _IsShowGuardObjectsWithDownTimeReglaments = value;
                OnPropertyChanged(nameof(IsShowGuardObjectsWithDownTimeReglaments));
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
        private ObservableCollection<ServiceOrderDiffModel> _DiffModel = new ObservableCollection<ServiceOrderDiffModel>();
        public ObservableCollection<ServiceOrderDiffModel> DiffModel {
            get => _DiffModel;
            set {
                _DiffModel = value;
                OnPropertyChanged(nameof(DiffModel));
            }
        }
        /// <summary>
        /// Получаем все объекты, у которых есть регламент на ПС и связываем с сущностью андромеды, в зависимости от цвета маркера становится ясно, на сколько всё плохо.
        /// </summary>
        private RelayCommand _ShowGuardObjectsWithDownTimeReglamentsCommand;
        public RelayCommand ShowGuardObjectsWithDownTimeReglamentsCommand {
            get => _ShowGuardObjectsWithDownTimeReglamentsCommand ??= new RelayCommand(async obj => {
                //if (obj == null)
                gmaps_contol.Markers.Clear();
                //else {
                //var andr = msCRMContext.NewAndromedaExtensionBase.Include(x => x.NewAndromeda).FirstOrDefault(x => x.NewNumber.Equals(obj));
                //if (andr != null) {
                //    var m = gmaps_contol.Markers.FirstOrDefault(x => x.Tag.ToString() == andr.NewAndromedaId.ToString() && andr.NewAndromeda.Statecode == 0 && andr.NewAndromeda.Statuscode == 1 && andr.NewAndromeda.DeletionStateCode == 0);
                //    if (m != null)
                //gmaps_contol.Markers.Remove(m);
                //}
                //}

                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, e) => {
                    Loading = true;
                    var datas = (from gob in msCRMContext.NewGuardObjectBase
                                 join goeb in msCRMContext.NewGuardObjectExtensionBase on gob.NewGuardObjectId equals goeb.NewGuardObjectId
                                 join andr in msCRMContext.NewAndromedaExtensionBase on goeb.NewObjectNumber equals andr.NewNumber
                                 join a in msCRMContext.NewAndromedaBase on andr.NewAndromedaId equals a.NewAndromedaId
                                 //join t2eb in msCRMContext.NewTest2ExtensionBase on goeb.NewObjectNumber equals t2eb.NewNumber
                                 join t2eb in msCRMContext.NewTest2ExtensionBase on andr.NewAndromedaId equals t2eb.NewAndromedaServiceorder
                                 join t2b in msCRMContext.NewTest2Base on t2eb.NewTest2Id equals t2b.NewTest2Id
                                 where gob.DeletionStateCode == 0
                                     && gob.Statecode == 0
                                     && gob.Statuscode == 1
                                     && goeb.NewRrPs == true
                                     && goeb.NewPriostDate == null
                                     && goeb.NewObjDeleteDate == null
                                     && goeb.NewRemoveDate == null
                                     && t2b.DeletionStateCode == 0
                                     && t2b.Statecode == 0
                                     && t2b.Statuscode == 1
                                     && a.DeletionStateCode == 0
                                     && a.Statecode == 0
                                     && a.Statuscode == 1
                                 //&& t2eb.NewNumber.ToString().Contains(obj == null ? "" : obj.ToString())
                                 //&& goeb.NewObjectNumber==9520
                                 select new { goeb, t2eb, andr }).AsNoTracking().Distinct().ToList();
                    if (datas == null)
                        return;
                    var grp_result = datas.GroupBy(a => new { a.goeb.NewObjectNumber }).ToList();
                    foreach (var grp_item in grp_result) {
                        foreach (var item in grp_item.Where(x => x.t2eb.NewDate.HasValue).ToList()) {
                            if (item.t2eb.NewDate.Value.AddHours(5).Date <= DateTime.Now.Date) {
                                var diff = DiffModel.FirstOrDefault(x => x.ObjectID == item.andr.NewAndromedaId);
                                if (diff == null)
                                    Application.Current.Dispatcher.Invoke(() => {
                                        DiffModel.Add(new ServiceOrderDiffModel(item.andr.NewAndromedaId, item.goeb.NewObjectNumber.Value, item.t2eb.NewDate.Value.AddHours(5),
                                            null, Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays), null));
                                    });
                                else if (!diff.OldDays.HasValue) {
                                    diff.OldDate = item.t2eb.NewDate.Value.AddHours(5);
                                    diff.OldDays = Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays);
                                }
                                else if (Math.Abs(diff.OldDays.Value) > Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays) && item.t2eb.NewResult == 1) {
                                    diff.OldDate = item.t2eb.NewDate.Value.AddHours(5);
                                    diff.OldDays = Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays);
                                }
                            }
                            else {
                                var diff = DiffModel.FirstOrDefault(x => x.ObjectID == item.andr.NewAndromedaId);
                                if (diff == null)
                                    Application.Current.Dispatcher.Invoke(() => {
                                        DiffModel.Add(new ServiceOrderDiffModel(item.andr.NewAndromedaId, item.goeb.NewObjectNumber.Value, null,
                                            item.t2eb.NewDate, null, Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays)));
                                    });
                                else if (!diff.NewDay.HasValue) {
                                    diff.NewDate = item.t2eb.NewDate.Value.AddHours(5);
                                    diff.NewDay = Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays);
                                }
                                else if (Math.Abs(diff.NewDay.Value) > Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays)) {
                                    diff.NewDate = item.t2eb.NewDate.Value.AddHours(5);
                                    diff.NewDay = Math.Abs((DateTime.Now.Date - item.t2eb.NewDate).Value.TotalDays);
                                }
                            }

                        }
                        //DrawMarker.Execute(null);
                    }
                    App.Current.Dispatcher.Invoke((Action)delegate {
                        foreach (var diffmodel_item in DiffModel) {
                            NewAndromedaExtensionBase extensionBase = msCRMContext.NewAndromedaExtensionBase.FirstOrDefault(x => x.NewAndromedaId == diffmodel_item.ObjectID);
                            if (extensionBase == null)
                                continue;
                            GMapMarker mark = gmaps_contol.Markers.FirstOrDefault(x => x.Tag.ToString() == diffmodel_item.ObjectID.ToString());
                            if (mark == null) {
                                GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(extensionBase.NewLatitude), Convert.ToDouble(extensionBase.NewLonitude))) {
                                    Shape = new Ellipse {
                                        Width = 20,
                                        Height = 20,
                                        Stroke = diffmodel_item.OldDays < 30 ? Brushes.Green :
                                        diffmodel_item.OldDays < 50 ? Brushes.Blue :
                                        diffmodel_item.OldDays < 100 ? Brushes.Yellow :
                                        Brushes.Red,
                                        StrokeThickness = 7.5,
                                        ToolTip = string.Format("{0} ({1})" + Environment.NewLine + "{2}" 
                                        + Environment.NewLine+ "Пред. {3:N} ({4})"
                                        + Environment.NewLine + "След. {5:N} ({6})",
                                            extensionBase.NewNumber, extensionBase.NewName, extensionBase.NewAddress
                                            ,diffmodel_item.OldDays,diffmodel_item.OldDate,
                                            diffmodel_item.NewDay,diffmodel_item.NewDate
                                            ),
                                        AllowDrop = false
                                    }
                                };
                                marker.Tag = extensionBase.NewAndromedaId;
                                marker.Shape.MouseLeftButtonDown += Shape_MouseLeftButtonDownAsync;
                                gmaps_contol.Markers.Add(marker);
                            }
                            else {
                                Ellipse ellipse = mark.Shape as Ellipse;
                                ellipse.Stroke = diffmodel_item.OldDays < 30 ? Brushes.Green :
                                        diffmodel_item.OldDays < 50 ? Brushes.Blue :
                                        diffmodel_item.OldDays < 100 ? Brushes.Yellow :
                                        Brushes.Red;
                                mark.Tag = extensionBase.NewAndromedaId;
                                mark.Shape.MouseLeftButtonDown += Shape_MouseLeftButtonDownAsync;
                                gmaps_contol.Markers.Add(mark);
                            }
                        }
                    });
                };
                bw.RunWorkerCompleted += (s, e) => {
                    Loading = false;
                };
                bw.RunWorkerAsync();
            });
        }
        /// <summary>
        /// Создаем новую заявку технику
        /// </summary>
        private RelayCommand _AddNewServiceOrderCommand;
        public RelayCommand AddNewServiceOrderCommand {
            get => _AddNewServiceOrderCommand ??= new RelayCommand(async obj => {
                //получаем объект Андромеды. 
                //TODO: требуется однозначно определить объект андромеды
                msCRMContext = GetMsCRMContext();
                List<NewAndromedaExtensionBase> andr_list = msCRMContext.NewAndromedaExtensionBase.Where(x => x.NewNumber.Value.ToString().Equals(NewServiceOrderObjectNumber) && x.NewAndromeda.DeletionStateCode == 0 && x.NewAndromeda.Statecode == 0 && x.NewAndromeda.Statuscode == 1).AsNoTracking().ToList();
                if (andr_list.Count <= 0) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Объект Андромеды не может быть определен. Проверьте правильность номера",
                        Type = NotificationType.Error
                    });
                    return;
                }
                if (andr_list.Count > 1) {
                    //метод по которому определяем однозначно
                }

                if (NewServiceOrderSelectedServiceman == null) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Выберите техника",
                        Type = NotificationType.Error
                    });
                    return;
                }
                //Требуется определить пользователя
                SystemUserBase sub = msCRMContext.SystemUserBase.FirstOrDefault(x => x.DomainName.Contains(Environment.UserName));
                if (sub == null) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Пользователь не определен",
                        Type = NotificationType.Error
                    });
                    return;
                }
                Guid id = Guid.NewGuid();
                NewTest2Base t2b = new NewTest2Base {
                    NewTest2Id = id,
                    CreatedBy = sub.SystemUserId,
                    CreatedOn = DateTime.Now.AddHours(-5),
                    DeletionStateCode = 0,
                    ModifiedBy = sub.SystemUserId,
                    ModifiedOn = DateTime.Now.AddHours(-5),
                    Statecode = 0,
                    Statuscode = 1,
                    OwningUser = sub.SystemUserId,
                    OwningBusinessUnit = sub.BusinessUnitId
                };

                msCRMContext.Add<NewTest2Base>(t2b).State = EntityState.Added;
                NewTest2ExtensionBase t2eb = new NewTest2ExtensionBase {
                    NewTest2Id = id,
                    NewWhoInit = sub.LastName,
                    NewName = string.IsNullOrEmpty(NewServiceOrderDescription) ? "Регламент ПС" : NewServiceOrderDescription,
                    NewDate = NewServiceOrderDate.AddHours(-5),
                    NewCategory = 1,
                    NewAndromedaServiceorder = andr_list[0].NewAndromedaId,
                    NewObjName = andr_list[0].NewName,
                    NewNumber = andr_list[0].NewNumber,
                    NewAddress = andr_list[0].NewAddress,
                    NewServicemanServiceorderPs = NewServiceOrderSelectedServiceman.NewServicemanId
                };
                msCRMContext.Add<NewTest2ExtensionBase>(t2eb).State = EntityState.Added;
                int result = await msCRMContext.SaveChangesAsync();
                notificationManager.Show(new NotificationContent {
                    Title = "",
                    Message = "Заявка создана",
                    Type = NotificationType.Success
                });
            }, obj => !string.IsNullOrEmpty(NewServiceOrderObjectNumber));
        }
        private RelayCommand _FlyoutNewServiceOrderVisibleCommand;
        public RelayCommand FlyoutNewServiceOrderVisibleCommand {
            get => _FlyoutNewServiceOrderVisibleCommand ??= new RelayCommand(async obj => {
                FlyoutNewServiceOrder = !FlyoutNewServiceOrder;
            });
        }
        private RelayCommand _ChangeDateOrderCommand;
        public RelayCommand ChangeDateOrderCommand {
            get => _ChangeDateOrderCommand ??= new RelayCommand(async obj => {
                DateTime? dt = obj as DateTime?;
                if (SelectedServiceOrder == null || !dt.HasValue)
                    return;
                SelectedServiceOrder.t2eb.NewDate = dt.Value.AddHours(-5);

                msCRMContext = GetMsCRMContext();
                if (!ServiceOrdersList.Any(x => x.t2eb.NewNumber.Value.Equals(SelectedServiceOrder.t2eb.NewNumber) && x.t2eb.NewTest2Id != SelectedServiceOrder.t2eb.NewTest2Id)) {
                    var local = msCRMContext.Set<NewTest2ExtensionBase>().Local.FirstOrDefault(entry => entry.NewTest2Id.Equals(SelectedServiceOrder.t2eb.NewTest2Id));
                    if (local != null)
                        msCRMContext.Entry(local).State = EntityState.Detached;
                    msCRMContext.Entry<NewTest2ExtensionBase>(SelectedServiceOrder.t2eb).State = EntityState.Modified;
                }
                else {
                    var list = ServiceOrdersList.Where(x => x.t2eb.NewNumber.Value.Equals(SelectedServiceOrder.t2eb.NewNumber) && x.t2eb.NewTest2Id != SelectedServiceOrder.t2eb.NewTest2Id);
                    foreach (var item in list) {
                        item.t2eb.NewDate = dt.Value.AddHours(-5);
                        var local = msCRMContext.Set<NewTest2ExtensionBase>().Local.FirstOrDefault(entry => entry.NewTest2Id.Equals(item.t2eb.NewTest2Id));
                        if (local != null)
                            msCRMContext.Entry(local).State = EntityState.Detached;
                        msCRMContext.Entry<NewTest2ExtensionBase>(item.t2eb).State = EntityState.Modified;
                    }
                }
                await msCRMContext.SaveChangesAsync();

                notificationManager.Show(new NotificationContent {
                    Title = "Информация",
                    Message = "Данные сохранены",
                    Type = NotificationType.Success
                });
                //ShowServiceOrderOnMapCommand.Execute(null);
            });
        }
        private RelayCommand _SelectDataGridRow;
        public RelayCommand SelectDataGridRow {
            get => _SelectDataGridRow ??= new RelayCommand(async obj => {
                if (obj == null)
                    return;
                ServiceOrdersModel som = obj as ServiceOrdersModel;
                if (som == null)
                    return;
                if (SelectedOrders.Contains(som)) {
                    SelectedOrders.Remove(som);
                    som.IsSelected = false;
                    ServiceOrdersList.FirstOrDefault(x => x.t2eb.NewTest2Id == som.t2eb.NewTest2Id).IsSelected = false;
                }
                else {
                    SelectedOrders.Add(som);
                    som.IsSelected = true;
                    ServiceOrdersList.FirstOrDefault(x => x.t2eb.NewTest2Id == som.t2eb.NewTest2Id).IsSelected = true;
                }
                OnPropertyChanged(nameof(ServiceOrdersList));
            });
        }
        private RelayCommand _OnCheckedCommand;
        public RelayCommand OnCheckedCommand {
            get => _OnCheckedCommand ??= new RelayCommand(async obj => {
                if (obj == null)
                    return;
                Guid? _id = obj as Guid?;
                if (!_id.HasValue)
                    return;
                ServiceOrdersModel som = ServiceOrdersList.First(x => x.t2eb.NewTest2Id.Equals(_id));
                if (som == null)
                    return;
                if (SelectedOrders.Contains(som))
                    SelectedOrders.Remove(som);
                else
                    SelectedOrders.Add(som);
                OnPropertyChanged(nameof(ServiceOrdersList));
            });
        }

        private RelayCommand _AddSelectedOrder;
        public RelayCommand AddSelectedOrder {
            get => _AddSelectedOrder ??= new RelayCommand(async obj => {
                if (obj == null)
                    return;
                if (!(obj is ServiceOrdersModel o))
                    return;
                if (SelectedOrders != null) {
                    var so = SelectedOrders.FirstOrDefault(x => x.t2eb.NewTest2Id == o.t2eb.NewTest2Id);
                    if (so == null)
                        SelectedOrders.Add(o);
                    else
                        SelectedOrders.Remove(o);
                }
                else
                    SelectedOrders.Add(o);
            });
        }

        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FlyoutFilterOpened = !FlyoutFilterOpened;
            });
        }
        /// <summary>
        /// Определяем свойства и настройки для контрола карты
        /// </summary>
        private RelayCommand _InitializeMapControl;
        public RelayCommand InitializeMapControl {
            get => _InitializeMapControl ??= new RelayCommand(async obj => {
                if (gmaps_contol == null)
                    gmaps_contol = new GMapControl();

                GMaps.Instance.Mode = AccessMode.ServerAndCache;
                gmaps_contol.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
                gmaps_contol.MinZoom = 5;
                gmaps_contol.MaxZoom = 17;
                gmaps_contol.Zoom = 12;
                gmaps_contol.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
                gmaps_contol.CanDragMap = true;
                gmaps_contol.DragButton = MouseButton.Left;
                gmaps_contol.CenterPosition = new PointLatLng(55.159904, 61.401919);
                gmaps_contol.IgnoreMarkerOnMouseWheel = true;

                ShowGuardObjectsWithDownTimeReglamentsCommand.Execute(null);

            });
        }

        /// <summary>
        /// Отображаем на карте объекты, по которым есть заявки на указанную дату
        /// </summary>
        //private RelayCommand _ShowServiceOrderOnMapCommand;
        //public RelayCommand ShowServiceOrderOnMapCommand {
        //    get => _ShowServiceOrderOnMapCommand ??= new RelayCommand(async obj => {
        //        msCRMContext = GetMsCRMContext();
        //        gmaps_contol.Markers.Clear();
        //        if (msCRMContext == null)
        //            return;
        //        //TODO: сообщение юзверю
        //        //TODO: переключатель мужду ос/пс
        //        var orders = (from t2eb in msCRMContext.NewTest2ExtensionBase
        //                      join t2b in msCRMContext.NewTest2Base on t2eb.NewTest2Id equals t2b.NewTest2Id
        //                      join andr in msCRMContext.NewAndromedaExtensionBase on t2eb.NewAndromedaServiceorder equals andr.NewAndromedaId
        //                      join smeb in msCRMContext.NewServicemanExtensionBase on t2eb.NewServicemanServiceorderPs equals smeb.NewServicemanId
        //                      where t2eb.NewDate.Value.Date == FilterDate.Date.AddHours(-5).Date
        //                      && t2b.DeletionStateCode == 0
        //                      && t2b.Statecode == 0
        //                      && t2b.Statuscode == 1
        //                      select new ServiceOrdersModel(t2eb, smeb, andr.NewLatitude, andr.NewLonitude)
        //                      ).AsNoTracking().Distinct().ToList();
        //        //if (orders.Count <= 0)
        //        //    return;
        //        ServiceOrdersList = new ObservableCollection<ServiceOrdersModel>(orders);
        //        //TODO: сообщение юзверю
        //        foreach (var item in orders) {
        //            GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude))) {
        //                Shape = new Ellipse {
        //                    Width = 20,
        //                    Height = 20,
        //                    Stroke = Brushes.Green,
        //                    //Stroke = c1.GetDistanceTo(c2) > 150.0 ? Brushes.Red : Brushes.Purple,
        //                    StrokeThickness = 7.5,
        //                    //ToolTip = string.Format("{0} - {1} ({2})", item.Number, item.Name, item.Address),
        //                    ToolTip = string.Format("{0} ({1})" + Environment.NewLine + "{2}",
        //                        item.t2eb.NewNumber, item.t2eb.NewObjName, item.t2eb.NewAddress),
        //                    AllowDrop = false
        //                }
        //            };
        //            marker.Tag = item.t2eb.NewTest2Id;
        //            marker.Shape.MouseLeftButtonDown += Shape_MouseLeftButtonDownAsync;
        //            gmaps_contol.Markers.Add(marker);
        //        }
        //    });
        //}

        private RelayCommand _ClearSelectionCommand;
        public RelayCommand ClearSelectionCommand {
            get => _ClearSelectionCommand ??= new RelayCommand(async obj => {
                SelectedMarkers.Clear();
                BrushesMarkersCommand.Execute(null);
            });
        }

        private RelayCommand _BrushesMarkersCommand;
        public RelayCommand BrushesMarkersCommand {
            get => _BrushesMarkersCommand ??= new RelayCommand(async obj => {
                if (SelectedMarkers == null)
                    SelectedMarkers = new ObservableCollection<GMapMarker>();
                var s_markers = gmaps_contol.Markers.Intersect(SelectedMarkers).ToList();
                foreach (var s_item in s_markers) {
                    Ellipse ellipse = s_item.Shape as Ellipse;
                    ellipse.Stroke = Brushes.Purple;
                }

                var us_markers = gmaps_contol.Markers.Except(SelectedMarkers).ToList();
                foreach (var us_item in us_markers) {
                    Ellipse ellipse = us_item.Shape as Ellipse;
                    //ellipse.Stroke = Brushes.Green;
                    ServiceOrderDiffModel diffModel = DiffModel.FirstOrDefault(x => x.ObjectID.ToString() == us_item.Tag.ToString());
                    if (diffModel == null)
                        continue;
                    ellipse.Stroke = diffModel.OldDays < 30 ? Brushes.Green :
                             diffModel.OldDays < 50 ? Brushes.Blue :
                             diffModel.OldDays < 100 ? Brushes.Yellow :
                             Brushes.Red;
                }
            });
        }
        /// <summary>
        /// Получаем техников
        /// </summary>
        private RelayCommand _GetServicemansCommand;
        public RelayCommand GetServicemansCommand {
            get => _GetServicemansCommand ??= new RelayCommand(async obj => {
                msCRMContext = GetMsCRMContext();
                //TODO: техники пс или обычные
                var sm = msCRMContext.NewServicemanExtensionBase.Where(x => x.NewCategory == 6 && x.NewIswork == true).AsNoTracking().ToList();
                ServicemanList = new ObservableCollection<NewServicemanExtensionBase>(sm);
            });
        }
        /// <summary>
        /// Назначаем техника на выбранные заявки по маркерам
        /// </summary>
        private RelayCommand _AssignServicemanOnSelectedMarkersCommand;
        public RelayCommand AssignServicemanOnSelectedMarkersCommand {
            get => _AssignServicemanOnSelectedMarkersCommand ??= new RelayCommand(async obj => {
                if (SelectedMarkers == null)
                    return;
                if (SelectedMarkers.Count <= 0)
                    return;
                if (SelectedServiceman == null)
                    return;
                msCRMContext = GetMsCRMContext();
                //foreach (var item in SelectedMarkers) {
                //    var element = msCRMContext.NewTest2ExtensionBase.Find(item.Tag);
                //    if (element == null)
                //        continue;
                //    element.NewServicemanServiceorderPs = SelectedServiceman.NewServicemanId;
                //    msCRMContext.Entry<NewTest2ExtensionBase>(element).State = EntityState.Modified;
                //}

                SystemUserBase sub = msCRMContext.SystemUserBase.FirstOrDefault(x => x.DomainName.Contains(Environment.UserName));
                if (sub == null) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Пользователь не определен",
                        Type = NotificationType.Error
                    });
                    return;
                }

                foreach (var item in SelectedMarkers) {
                    var andromeda_element = msCRMContext.NewAndromedaExtensionBase.FirstOrDefault(x => x.NewAndromedaId.ToString() == item.Tag.ToString());
                    if (andromeda_element == null)
                        continue;
                    Guid id = Guid.NewGuid();
                    NewTest2Base t2b = new NewTest2Base {
                        NewTest2Id = id,
                        CreatedBy = sub.SystemUserId,
                        CreatedOn = DateTime.Now.AddHours(-5),
                        DeletionStateCode = 0,
                        ModifiedBy = sub.SystemUserId,
                        ModifiedOn = DateTime.Now.AddHours(-5),
                        Statecode = 0,
                        Statuscode = 1,
                        OwningUser = sub.SystemUserId,
                        OwningBusinessUnit = sub.BusinessUnitId
                    };

                    msCRMContext.Add<NewTest2Base>(t2b).State = EntityState.Added;
                    NewTest2ExtensionBase t2eb = new NewTest2ExtensionBase {
                        NewTest2Id = id,
                        NewWhoInit = sub.LastName,
                        NewName = "Регламент ПС",
                        NewDate = OrderDate.AddHours(-5),
                        NewCategory = 1,
                        NewAndromedaServiceorder = andromeda_element.NewAndromedaId,
                        NewObjName = andromeda_element.NewName,
                        NewNumber = andromeda_element.NewNumber,
                        NewAddress = andromeda_element.NewAddress,
                        //NewServicemanServiceorderPs = NewServiceOrderSelectedServiceman.NewServicemanId
                        NewServicemanServiceorderPs = SelectedServiceman.NewServicemanId
                    };
                    msCRMContext.Add<NewTest2ExtensionBase>(t2eb).State = EntityState.Added;
                    int result = await msCRMContext.SaveChangesAsync();
                    notificationManager.Show(new NotificationContent {
                        Title = "",
                        Message = "Заявка создана",
                        Type = NotificationType.Success
                    });

                    //ShowGuardObjectsWithDownTimeReglamentsCommand.Execute(andromeda_element.NewNumber);
                }
                await msCRMContext.SaveChangesAsync();
                SelectServicemanCommand.Execute(SelectedServiceman);
                SelectedServiceman = null;
                FlyoutServicemanListOpened = false;
                SelectedMarkers.Clear();
                //SelectedOrders.Clear();
                BrushesMarkersCommand.Execute(null);
                //UnCheckedToggleButtonServicemanListCommand.Execute(null);
                //ShowServiceOrderOnMapCommand.Execute(null);
                notificationManager.Show(new NotificationContent {
                    Title = "Информация",
                    Message = "Данные сохранены",
                    Type = NotificationType.Success
                });
            });
        }
        /// <summary>
        /// Назначаем техника на выбранные заявки по заявкам
        /// </summary>
        private RelayCommand _AssignServicemanOnSelectedOrdersCommand;
        public RelayCommand AssignServicemanOnSelectedOrdersCommand {
            get => _AssignServicemanOnSelectedOrdersCommand ??= new RelayCommand(async obj => {
                if (SelectedMarkers == null)
                    return;
                if (SelectedOrders.Count <= 0)
                    return;
                if (SelectedServiceman == null)
                    return;
                msCRMContext = GetMsCRMContext();
                //foreach (var item in SelectedMarkers) {
                //    var element = msCRMContext.NewTest2ExtensionBase.Find(item.Tag);
                //    if (element == null)
                //        continue;
                //    element.NewServicemanServiceorderPs = SelectedServiceman.NewServicemanId;
                //    msCRMContext.Entry<NewTest2ExtensionBase>(element).State = EntityState.Modified;
                //}
                foreach (var item in SelectedOrders) {
                    var element = msCRMContext.NewTest2ExtensionBase.Find(item.t2eb.NewTest2Id);
                    if (element == null)
                        continue;
                    element.NewServicemanServiceorderPs = SelectedServiceman.NewServicemanId;
                    msCRMContext.Entry<NewTest2ExtensionBase>(element).State = EntityState.Modified;
                }
                await msCRMContext.SaveChangesAsync();
                SelectServicemanCommand.Execute(SelectedServiceman);
                SelectedServiceman = null;
                FlyoutServicemanListOpened = false;
                //SelectedMarkers.Clear();
                SelectedOrders.Clear();
                //BrushesMarkersCommand.Execute(null);
                //UnCheckedToggleButtonServicemanListCommand.Execute(null);
                //ShowServiceOrderOnMapCommand.Execute(null);
                notificationManager.Show(new NotificationContent {
                    Title = "Информация",
                    Message = "Данные сохранены",
                    Type = NotificationType.Success
                });
            });
        }
        private RelayCommand _OpenFlyoutServicemanListCommand;
        public RelayCommand OpenFlyoutServicemanListCommand {
            get => _OpenFlyoutServicemanListCommand ??= new RelayCommand(async obj => {
                FlyoutServicemanListOpened = !FlyoutServicemanListOpened;
            }, obj => SelectedMarkers.Count > 0 || SelectedOrders.Count > 0);
        }
        private RelayCommand _SelectServicemanCommand;
        public RelayCommand SelectServicemanCommand {
            get => _SelectServicemanCommand ??= new RelayCommand(async obj => {
                if (obj == null)
                    return;
                if (!(obj is ToggleButton tb))
                    return;
                Guid? ServicemanID = Guid.Parse(tb.Tag.ToString());
                if (!ServicemanID.HasValue)
                    return;
                SelectedServiceman = tb.IsChecked == true ? ServicemanList.FirstOrDefault(x => x.NewServicemanId == ServicemanID.Value) : null;
                if (SelectedServiceman == null)
                    return;
                ServicemanList.FirstOrDefault(x => x.NewServicemanId == ServicemanID.Value).Checked = true;
                if (SelectedOrders.Count > 0)
                    AssignServicemanOnSelectedOrdersCommand.Execute(SelectedServiceman);
                if (SelectedMarkers.Count > 0)
                    AssignServicemanOnSelectedMarkersCommand.Execute(SelectedServiceman);
                tb.IsChecked = false;
            });
        }

        private RelayCommand _UnCheckedToggleButtonServicemanListCommand;
        public RelayCommand UnCheckedToggleButtonServicemanListCommand {
            get => _UnCheckedToggleButtonServicemanListCommand ??= new RelayCommand(async obj => {
                foreach (var item in ServicemanList) {
                    item.Checked = false;
                }
            });
        }
        private void Shape_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e) {
            if (sender == null)
                return;
            Ellipse img = sender as Ellipse;
            if (img == null)
                return;
            if (!(img.DataContext is GMapMarker marker))
                return;
            Ellipse ellipse = marker.Shape as Ellipse;
            ellipse.Stroke = Brushes.Purple;
            var mr = SelectedMarkers.FirstOrDefault(x => x.Tag.ToString() == marker.Tag.ToString());
            if (mr == null)
                SelectedMarkers.Add(marker);
            else {
                SelectedMarkers.Remove(mr);
                //ellipse.Stroke = Brushes.Green;
            }
            BrushesMarkersCommand.Execute(null);
        }
    }
}
