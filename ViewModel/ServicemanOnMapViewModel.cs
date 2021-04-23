using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.EntityFrameworkCore;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models;
using VityazReports.Models.ServicemanOnMapViewModel;
using GeoCoordinatePortable;

namespace VityazReports.ViewModel {
    public class ServicemanOnMapViewModel : BaseViewModel {
        private readonly MsCRMContext context = new MsCRMContext();
        NotificationManager notificationManager = new NotificationManager();

        public ServicemanOnMapViewModel() {
            InitializeMapControl.Execute(null);
            gmaps_contol.IgnoreMarkerOnMouseWheel = true;
            FillServicemanCommand.Execute(null);
            FilterServicemanFlyoutVisible = true;
        }

        private GMapControl _gmaps_contol = new GMapControl();
        public GMapControl gmaps_contol {
            get => _gmaps_contol;
            set {
                _gmaps_contol = value;
                OnPropertyChanged(nameof(gmaps_contol));
            }
        }

        private bool _FilterServicemanFlyoutVisible;
        public bool FilterServicemanFlyoutVisible {
            get => _FilterServicemanFlyoutVisible;
            set {
                _FilterServicemanFlyoutVisible = value;
                OnPropertyChanged(nameof(FilterServicemanFlyoutVisible));
            }
        }
        /// <summary>
        /// Коллекция для хранения списка техников
        /// </summary>
        private ObservableCollection<NewServicemanExtensionBase> _Servicemans = new ObservableCollection<NewServicemanExtensionBase>();
        public ObservableCollection<NewServicemanExtensionBase> Servicemans {
            get => _Servicemans;
            set {
                _Servicemans = value;
                OnPropertyChanged(nameof(Servicemans));
            }
        }

        private DateTime? _SelectedDate;
        public DateTime? SelectedDate {
            get => _SelectedDate;
            set {
                _SelectedDate = value;
                if (_SelectedDate != null)
                    GetCoordinatesByServicemanCommand.Execute(null);
                OnPropertyChanged(nameof(SelectedDate));
            }
        }
        private NewServicemanExtensionBase _SelectedServiceman;
        public NewServicemanExtensionBase SelectedServiceman {
            get => _SelectedServiceman;
            set {
                _SelectedServiceman = value;
                if (_SelectedServiceman != null)
                    GetListDatesByServiceman.Execute(null);
                OnPropertyChanged(nameof(SelectedServiceman));
            }
        }
        /// <summary>
        /// Коллекция для хранения дат в заявках технику
        /// </summary>
        private ObservableCollection<DateTime> _ListDates = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> ListDates {
            get => _ListDates;
            set {
                _ListDates = value;
                OnPropertyChanged(nameof(ListDates));
            }
        }
        /// <summary>
        /// Определяет видимость плавающей области с объектами техников
        /// </summary>
        private bool _PointsByServicemanFlyoutVisible;
        public bool PointsByServicemanFlyoutVisible {
            get => _PointsByServicemanFlyoutVisible;
            set {
                _PointsByServicemanFlyoutVisible = value;
                OnPropertyChanged(nameof(PointsByServicemanFlyoutVisible));
            }
        }
        private ObservableCollection<ServicemansPoints> _Points = new ObservableCollection<ServicemansPoints>();
        public ObservableCollection<ServicemansPoints> Points {
            get => _Points;
            set {
                _Points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        private ServicemansPoints _SelectedPoint;
        public ServicemansPoints SelectedPoint {
            get => _SelectedPoint;
            set {
                if (_SelectedPoint == null && value != null) {
                    var _y = gmaps_contol.Markers.Where(x => x.Tag.ToString() == value.Order.ToString());
                    if (_y == null)
                        return;
                    foreach (var item in _y) {
                        GMapMarker _m = item as GMapMarker;
                        if (_m == null)
                            return;
                        _m.ZIndex = 1000;
                        Ellipse _ellipse = _m.Shape as Ellipse;
                        _ellipse.StrokeThickness = 12;
                        _ellipse.Stroke = Brushes.Green;
                    }
                }
                if (_SelectedPoint != null) {
                    if (_SelectedPoint != value) {
                        if (value != null) {
                            var y = gmaps_contol.Markers.Where(x => x.Tag.ToString() == _SelectedPoint.Order.ToString());
                            if (y == null)
                                return;
                            foreach (var item in y) {
                                GMapMarker m = item as GMapMarker;
                                if (m == null)
                                    return;
                                m.ZIndex = 1;
                                Ellipse ellipse = m.Shape as Ellipse;
                                ellipse.StrokeThickness = 7.5;
                                ellipse.Stroke = ellipse.AllowDrop ? Brushes.Red : Brushes.Blue;
                            }
                            var _y = gmaps_contol.Markers.Where(x => x.Tag.ToString() == value.Order.ToString());
                            if (_y == null)
                                return;
                            foreach (var item in _y) {
                                GMapMarker _m = item as GMapMarker;
                                if (_m == null)
                                    return;
                                _m.ZIndex = 1000;
                                Ellipse _ellipse = _m.Shape as Ellipse;
                                _ellipse.StrokeThickness = 12;
                                _ellipse.Stroke = Brushes.Green;
                            }
                        }
                    }
                }

                _SelectedPoint = value;
                //if (_SelectedPoint != null) {
                //    var markers = gmaps_contol.Markers.Where(x => x.Tag.ToString() == _SelectedPoint.Order.ToString());
                //}
                OnPropertyChanged(nameof(SelectedPoint));
            }
        }
        /// <summary>
        /// Инициализируем контрол для карты
        /// </summary>
        private RelayCommand _InitializeMapControl;
        public RelayCommand InitializeMapControl {
            get => _InitializeMapControl ??= new RelayCommand(async obj => {
                GMaps.Instance.Mode = AccessMode.ServerAndCache;
                gmaps_contol.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
                gmaps_contol.MinZoom = 5;
                gmaps_contol.MaxZoom = 17;
                gmaps_contol.Zoom = 12;
                gmaps_contol.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
                gmaps_contol.CanDragMap = true;
                gmaps_contol.DragButton = MouseButton.Left;
                gmaps_contol.CenterPosition = new PointLatLng(55.159904, 61.401919);
            });
        }
        /// <summary>
        /// Заполняем коллекцию техников
        /// </summary>
        private RelayCommand _FillServicemanCommand;
        public RelayCommand FillServicemanCommand {
            get => _FillServicemanCommand ??= new RelayCommand(async obj => {
                Servicemans.Clear();
                var sm = context.NewServicemanExtensionBase.Where(x => x.NewIswork == true).AsNoTracking().ToList();
                if (sm.Count <= 0) {
                    notificationManager.Show(new NotificationContent() { Type = NotificationType.Error, Title = "Ошибка", Message = "Ошибка при получении списка техников" });
                    return;
                }
                foreach (var item in sm)
                    Servicemans.Add(item);
            });
        }

        private RelayCommand _GetListDatesByServiceman;
        public RelayCommand GetListDatesByServiceman {
            get => _GetListDatesByServiceman ??= new RelayCommand(async obj => {
                var coords = (from soc in context.ServiceOrderCoordinates
                              join soeb in context.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                              where soeb.NewServicemanServiceorder == SelectedServiceman.NewServicemanId
                                && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                              //&& !soc.SocIncomeLatitude.Equals("0")
                              //&& !soc.SocIncomeLongitude.Equals("0")
                              //&& !soc.SocOutcomeLatitide.Equals("0")
                              //&& !soc.SocOutcomeLongitude.Equals("0")
                              select new { soc, soeb }
                ).AsNoTracking().ToList();
                ListDates.Clear();
                if (coords.Count() <= 0) {
                    notificationManager.Show(new NotificationContent() { Type = NotificationType.Error, Title = "Ошибка", Message = "Для выбранного техника не удалось найти координаты" });
                    return;
                }
                var l_dates = coords.Select(x => x.soeb.NewDate).OrderBy(x => x.Value.Date).Distinct();
                if (l_dates.Count() <= 0)
                    return;
                foreach (var item in l_dates) {
                    ListDates.Add(item.Value.AddHours(5).Date);
                }
            }, obj => SelectedServiceman != null);
        }
        /// <summary>
        /// Получаем список точек (координат) где у нас был техник
        /// </summary>
        private RelayCommand _GetCoordinatesByServicemanCommand;
        public RelayCommand GetCoordinatesByServicemanCommand {
            get => _GetCoordinatesByServicemanCommand ??= new RelayCommand(async obj => {
                var coords = (from soc in context.ServiceOrderCoordinates
                              join soeb in context.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                              where soeb.NewServicemanServiceorder == SelectedServiceman.NewServicemanId
                              && soeb.NewDate.Value.Date == SelectedDate.Value.AddHours(-5).Date
                                && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                              select new { soc, soeb }
                              ).AsNoTracking().ToList();
                if (coords.Count <= 0)
                    return;
                Points.Clear();
                gmaps_contol.Markers.Clear();
                List<PointLatLng> points = new List<PointLatLng>();
                int counter = 1;
                foreach (var item in coords) {
                    //пришёл
                    GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.soc.SocIncomeLatitude), Convert.ToDouble(item.soc.SocIncomeLongitude))) {
                        Shape = new Ellipse {
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.Red,
                            StrokeThickness = 4.5,
                            ToolTip = string.Format("{0} - {1}", item.soeb.NewNumber, item.soeb.NewName),
                            AllowDrop = true
                        }
                    };
                    marker.Tag = counter;
                    points.Add(new PointLatLng(Convert.ToDouble(item.soc.SocIncomeLatitude), Convert.ToDouble(item.soc.SocIncomeLongitude)));
                    gmaps_contol.Markers.Add(marker);
                    //ушёл
                    marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.soc.SocOutcomeLatitide), Convert.ToDouble(item.soc.SocOutcomeLongitude))) {
                        Shape = new Ellipse {
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.Blue,
                            StrokeThickness = 4.5,
                            ToolTip = string.Format("{0} - {1}", item.soeb.NewNumber, item.soeb.NewName),
                            AllowDrop = false
                        }
                    };
                    marker.Tag = counter;
                    points.Add(new PointLatLng(Convert.ToDouble(item.soc.SocOutcomeLatitide), Convert.ToDouble(item.soc.SocOutcomeLongitude)));
                    gmaps_contol.Markers.Add(marker);
                    GeoCoordinate c1 = new GeoCoordinate(Convert.ToDouble(item.soc.SocIncomeLatitude), Convert.ToDouble(item.soc.SocIncomeLongitude));
                    GeoCoordinate c2 = new GeoCoordinate(Convert.ToDouble(item.soc.SocOutcomeLatitide), Convert.ToDouble(item.soc.SocOutcomeLongitude));
                    Points.Add(new ServicemansPoints((int)item.soeb.NewNumber, (DateTime)item.soeb.NewIncome.Value.AddHours(5), (DateTime)item.soeb.NewOutgone.Value.AddHours(5), counter, c1.GetDistanceTo(c2)));
                    counter++;
                }
                PointsByServicemanFlyoutVisible = Points.Count > 0;
                //MapRoute route = new MapRoute(points, "1");
                //route.Tag = "route";
                //GMapRoute gmRoute = new GMapRoute(route.Points);
                //gmaps_contol.Markers.Add(gmRoute);
            }, obj => SelectedDate != null);
        }
    }
}
