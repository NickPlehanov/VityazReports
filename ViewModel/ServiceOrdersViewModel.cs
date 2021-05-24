using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.EntityFrameworkCore;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private MsCRMContext msCRMContext;
        NotificationManager notificationManager;
        public ServiceOrdersViewModel() {
            InitializeMapControl.Execute(null);

            FilterDate = DateTime.Now;
            //FlyoutFilterOpened = true;

            SelectedMarkers = new ObservableCollection<GMapMarker>();
            ServicemanList = new ObservableCollection<NewServicemanExtensionBase>();
            ServiceOrdersList = new ObservableCollection<ServiceOrdersModel>();
            SelectedOrders = new ObservableCollection<ServiceOrdersModel>();
            GetServicemansCommand.Execute(null);

            notificationManager = new NotificationManager();

            ShowServiceOrderOnMapCommand.Execute(null);
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
                OnPropertyChanged(nameof(FilterDate));
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
                //if (_SelectedServiceOrder != null)
                //SelectDataGridRow.Execute(_SelectedServiceOrder);
                OnPropertyChanged(nameof(SelectedServiceOrder));
            }
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
            });
        }

        /// <summary>
        /// Отображаем на карте объекты, по которым есть заявки на указанную дату
        /// </summary>
        private RelayCommand _ShowServiceOrderOnMapCommand;
        public RelayCommand ShowServiceOrderOnMapCommand {
            get => _ShowServiceOrderOnMapCommand ??= new RelayCommand(async obj => {
                msCRMContext = GetMsCRMContext();
                gmaps_contol.Markers.Clear();
                if (msCRMContext == null)
                    return;
                //TODO: сообщение юзверю
                //TODO: переключатель мужду ос/пс
                var orders = (from t2eb in msCRMContext.NewTest2ExtensionBase
                              join t2b in msCRMContext.NewTest2Base on t2eb.NewTest2Id equals t2b.NewTest2Id
                              join andr in msCRMContext.NewAndromedaExtensionBase on t2eb.NewAndromedaServiceorder equals andr.NewAndromedaId
                              join smeb in msCRMContext.NewServicemanExtensionBase on t2eb.NewServicemanServiceorderPs equals smeb.NewServicemanId
                              where t2eb.NewDate.Value.Date == FilterDate.Date.AddHours(-5).Date
                              && t2b.DeletionStateCode == 0
                              && t2b.Statecode == 0
                              && t2b.Statuscode == 1
                              select new ServiceOrdersModel(t2eb, smeb, andr.NewLatitude, andr.NewLonitude)
                              ).AsNoTracking().Distinct().ToList();
                if (orders.Count() <= 0)
                    return;
                ServiceOrdersList = new ObservableCollection<ServiceOrdersModel>(orders);
                //TODO: сообщение юзверю
                foreach (var item in orders) {
                    GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude))) {
                        Shape = new Ellipse {
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.Green,
                            //Stroke = c1.GetDistanceTo(c2) > 150.0 ? Brushes.Red : Brushes.Purple,
                            StrokeThickness = 7.5,
                            //ToolTip = string.Format("{0} - {1} ({2})", item.Number, item.Name, item.Address),
                            ToolTip = string.Format("{0} ({1})" + Environment.NewLine + "{2}",
                                item.t2eb.NewNumber, item.t2eb.NewObjName, item.t2eb.NewAddress),
                            AllowDrop = false
                        }
                    };
                    marker.Tag = item.t2eb.NewTest2Id;
                    marker.Shape.MouseLeftButtonDown += Shape_MouseLeftButtonDownAsync;
                    gmaps_contol.Markers.Add(marker);
                }
            });
        }

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
                var s_markers = gmaps_contol.Markers.Intersect(SelectedMarkers).ToList();
                foreach (var s_item in s_markers) {
                    Ellipse ellipse = s_item.Shape as Ellipse;
                    ellipse.Stroke = Brushes.Red;
                }

                var us_markers = gmaps_contol.Markers.Except(SelectedMarkers).ToList();
                foreach (var us_item in us_markers) {
                    Ellipse ellipse = us_item.Shape as Ellipse;
                    ellipse.Stroke = Brushes.Green;
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
                var sm = msCRMContext.NewServicemanExtensionBase.Where(x => x.NewCategory == 6).AsNoTracking().ToList();
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
                foreach (var item in SelectedMarkers) {
                    var element = msCRMContext.NewTest2ExtensionBase.Find(item.Tag);
                    if (element == null)
                        continue;
                    element.NewServicemanServiceorderPs = SelectedServiceman.NewServicemanId;
                    msCRMContext.Entry<NewTest2ExtensionBase>(element).State = EntityState.Modified;
                }
                //foreach (var item in SelectedOrders) {
                //    var element = msCRMContext.NewTest2ExtensionBase.Find(item.t2eb.NewTest2Id);
                //    if (element == null)
                //        continue;
                //    element.NewServicemanServiceorderPs = SelectedServiceman.NewServicemanId;
                //    msCRMContext.Entry<NewTest2ExtensionBase>(element).State = EntityState.Modified;
                //}
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
                ShowServiceOrderOnMapCommand.Execute(null);
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
            GMapMarker marker = img.DataContext as GMapMarker;
            if (marker == null)
                return;
            Ellipse ellipse = marker.Shape as Ellipse;
            ellipse.Stroke = Brushes.Red;
            var mr = SelectedMarkers.FirstOrDefault(x => x.Tag.ToString() == marker.Tag.ToString());
            if (mr == null)
                SelectedMarkers.Add(marker);
            else {
                SelectedMarkers.Remove(mr);
                ellipse.Stroke = Brushes.Green;
            }
        }
    }
}
