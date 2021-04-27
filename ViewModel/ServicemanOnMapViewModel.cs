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
using System.Diagnostics;
using System.IO;

namespace VityazReports.ViewModel {
    public class ServicemanOnMapViewModel : BaseViewModel {
        private readonly MsCRMContext context = new MsCRMContext();
        private readonly A28Context a28Context = new A28Context();
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
                PointsByServicemanFlyoutVisible = Points.Count > 0;
                OnPropertyChanged(nameof(Points));
            }
        }
        /// <summary>
        /// True - ПС
        /// False - ОС
        /// </summary>
        private bool _SwitchOSPS;
        public bool SwitchOSPS {
            get => _SwitchOSPS;
            set {
                ListDates.Clear();
                Points.Clear();
                PointsByServicemanFlyoutVisible = Points.Count > 0;
                _SwitchOSPS = value;
                OnPropertyChanged(nameof(SwitchOSPS));
                GetListDatesByServiceman.Execute(null);
            }
        }

        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterServicemanFlyoutVisible = !FilterServicemanFlyoutVisible;
            });
        }

        private RelayCommand _HelpCommand;
        public RelayCommand HelpCommand {
            get => _HelpCommand ??= new RelayCommand(async obj => {
                if (File.Exists(@"\\server-nass\Install\WORKPLACE\Инструкции\Техники на карте.pdf"))
                    Process.Start(new ProcessStartInfo(@"\\server-nass\Install\WORKPLACE\Инструкции\Техники на карте.pdf") { UseShellExecute = true });
            });
        }
        private bool _ListDatesEnabled;
        public bool ListDatesEnabled {
            get => _ListDatesEnabled;
            set {
                _ListDatesEnabled = value;
                OnPropertyChanged(nameof(ListDatesEnabled));
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
                                ellipse.Stroke = ellipse.AllowDrop ? Brushes.Red : ellipse.ToolTip.ToString().Contains("(пришёл)") || ellipse.ToolTip.ToString().Contains("(ушёл)") ? Brushes.Purple : Brushes.Blue;
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
                List<NewServicemanExtensionBase> sm = new List<NewServicemanExtensionBase>();
                sm = context.NewServicemanExtensionBase.Where(x => x.NewIswork == true).AsNoTracking().ToList();
                if (sm.Count <= 0) {
                    notificationManager.Show(new NotificationContent() { Type = NotificationType.Error, Title = "Ошибка", Message = "Ошибка при получении списка техников" });
                    ListDatesEnabled = false;
                    return;
                }
                foreach (var item in sm)
                    Servicemans.Add(item);
            });
        }

        private RelayCommand _GetListDatesByServiceman;
        public RelayCommand GetListDatesByServiceman {
            get => _GetListDatesByServiceman ??= new RelayCommand(async obj => {
                List<DateTime?> l_dates = new List<DateTime?>();
                ListDates.Clear();
                Points.Clear();
                PointsByServicemanFlyoutVisible = Points.Count > 0;
                gmaps_contol.Markers.Clear();
                if (SelectedServiceman == null)
                    return;
                if (SwitchOSPS) {//PS
                    var coords = (from soc in context.ServiceOrderCoordinates
                                      //join soeb in context.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId /*into lj_soeb from soeb in lj_soeb.DefaultIfEmpty()*/
                                  join soeb in context.NewTest2ExtensionBase on soc.SocServiceOrderId equals soeb.NewTest2Id
                                  where soeb.NewServicemanServiceorderPs == SelectedServiceman.NewServicemanId
                                    && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                    && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                    && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                    && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                  select new { soc, soeb }).AsNoTracking().ToList();
;
                    if (coords.Count() <= 0) {
                        notificationManager.Show(new NotificationContent() { Type = NotificationType.Error, Title = "Ошибка", Message = "Для выбранного техника не удалось найти координаты" });
                        ListDatesEnabled = false;
                        return;
                    }
                    l_dates = coords.Select(x => x.soeb.NewDate).OrderBy(x => x.Value.Date).Distinct().ToList();
                    if (l_dates.Count() <= 0)
                        return;
                }
                else {//OS
                    var coords = (from soc in context.ServiceOrderCoordinates
                                  join soeb in context.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId /*into lj_soeb from soeb in lj_soeb.DefaultIfEmpty()*/
                                  //join soeb in context.NewTest2ExtensionBase on soc.SocServiceOrderId equals soeb.NewTest2Id
                                  where soeb.NewServicemanServiceorder== SelectedServiceman.NewServicemanId
                                    && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                    && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                    && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                    && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                  select new { soc, soeb }).AsNoTracking().ToList();
                    ;
                    if (coords.Count() <= 0) {
                        notificationManager.Show(new NotificationContent() { Type = NotificationType.Error, Title = "Ошибка", Message = "Для выбранного техника не удалось найти координаты" });
                        ListDatesEnabled = false;
                        return;
                    }
                    l_dates = coords.Select(x => x.soeb.NewDate).OrderBy(x => x.Value.Date).Distinct().ToList();
                    if (l_dates.Count() <= 0)
                        return;
                }

                foreach (var item in l_dates) {
                    ListDates.Add(item.Value.AddHours(5).Date);
                }
                ListDatesEnabled = ListDates.Count > 0;
            }, obj => SelectedServiceman != null);
        }
        /// <summary>
        /// Получаем список точек (координат) где у нас был техник
        /// </summary>
        private RelayCommand _GetCoordinatesByServicemanCommand;
        public RelayCommand GetCoordinatesByServicemanCommand {
            get => _GetCoordinatesByServicemanCommand ??= new RelayCommand(async obj => {
                List<ServicemanCoords> coords = new List<ServicemanCoords>();
                if (SwitchOSPS) {
                    coords = (from soc in context.ServiceOrderCoordinates
                              join soeb in context.NewTest2ExtensionBase on soc.SocServiceOrderId equals soeb.NewTest2Id
                              where soeb.NewServicemanServiceorderPs == SelectedServiceman.NewServicemanId
                              && soeb.NewDate.Value.Date == SelectedDate.Value.AddHours(-5).Date
                                && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                              select new ServicemanCoords(soc.SocIncomeLatitude, soc.SocIncomeLongitude, soc.SocOutcomeLatitide, soc.SocOutcomeLongitude, soeb.NewNumber, soeb.NewName, soeb.NewIncome, soeb.NewOutgone,soeb.NewAddress)
                                  ).AsNoTracking().ToList();
                }
                else {
                    coords = (from soc in context.ServiceOrderCoordinates
                              join soeb in context.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                              where soeb.NewServicemanServiceorder == SelectedServiceman.NewServicemanId
                              && soeb.NewDate.Value.Date == SelectedDate.Value.AddHours(-5).Date
                                && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                              //select new { soc, soeb }
                              select new ServicemanCoords(soc.SocIncomeLatitude, soc.SocIncomeLongitude, soc.SocOutcomeLatitide, soc.SocOutcomeLongitude, soeb.NewNumber, soeb.NewName, soeb.NewIncome, soeb.NewOutgone, soeb.NewAddress)
                                  ).AsNoTracking().ToList();
                }
                if (coords.Count() <= 0)
                    return;
                Points.Clear();
                gmaps_contol.Markers.Clear();
                List<PointLatLng> points = new List<PointLatLng>();
                int counter = 1;
                foreach (var item in coords) {
                    //пришёл
                    GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.IncomeLatitude), Convert.ToDouble(item.IncomeLongitude))) {
                        Shape = new Ellipse {
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.Red,
                            StrokeThickness = 7.5,
                            ToolTip = string.Format("(пришёл) {0} - {1}", item.Number, item.Name),
                            AllowDrop = true
                        }
                    };
                    marker.Tag = counter;
                    //points.Add(new PointLatLng(Convert.ToDouble(item.soc.SocIncomeLatitude), Convert.ToDouble(item.soc.SocIncomeLongitude)));
                    gmaps_contol.Markers.Add(marker);
                    //ушёл
                    marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.OutgoneLatitude), Convert.ToDouble(item.OutgoneLongitude))) {
                        Shape = new Ellipse {
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.Blue,
                            StrokeThickness = 7.5,
                            ToolTip = string.Format("(ушёл) {0} - {1}", item.Number, item.Name),
                            AllowDrop = false
                        }
                    };
                    marker.Tag = counter;
                    gmaps_contol.Markers.Add(marker);
                    GeoCoordinate c1 = new GeoCoordinate(Convert.ToDouble(item.IncomeLatitude), Convert.ToDouble(item.IncomeLongitude));
                    GeoCoordinate c2 = new GeoCoordinate(Convert.ToDouble(item.OutgoneLatitude), Convert.ToDouble(item.OutgoneLongitude));
                    Models.GuardObjectsOnMapGBR.Object a28obj = null;
                    if (item.Number.HasValue)
                        a28obj = a28Context.Object.FirstOrDefault(x => x.ObjectNumber == Convert.ToInt32(item.Number.Value.ToString(), 16) && x.RecordDeleted == false);
                    if (a28obj != null) {

                    }

                    if (a28obj == null)
                        Points.Add(new ServicemansPoints((int)item.Number, (DateTime)item.Income.Value.AddHours(5), (DateTime)item.Outgone.Value.AddHours(5), counter, c1.GetDistanceTo(c2),item.Address));
                    else {
                        if (a28obj.Latitude.HasValue && a28obj.Longitude.HasValue) {
                            GeoCoordinate c3 = new GeoCoordinate((double)a28obj.Latitude, (double)a28obj.Longitude);
                            Points.Add(new ServicemansPoints((int)item.Number,
                                (DateTime)item.Income.Value.AddHours(5),
                                (DateTime)item.Outgone.Value.AddHours(5),
                                counter,
                                c1.GetDistanceTo(c2),
                                a28obj.Address,
                                c1.GetDistanceTo(c3),
                                c2.GetDistanceTo(c3)
                                ));
                            marker = new GMapMarker(new PointLatLng(Convert.ToDouble(a28obj.Latitude), Convert.ToDouble(a28obj.Longitude))) {
                                Shape = new Ellipse {
                                    Width = 20,
                                    Height = 20,
                                    Stroke = Brushes.Purple,
                                    StrokeThickness = 7.5,
                                    ToolTip = string.Format("{0} - {1} ({2})", item.Number, a28obj.Name, a28obj.Address),
                                    AllowDrop = false
                                }
                            };
                            marker.Tag = counter;
                            gmaps_contol.Markers.Add(marker);
                        }
                        else
                            Points.Add(new ServicemansPoints((int)item.Number, (DateTime)item.Income.Value.AddHours(5), (DateTime)item.Outgone.Value.AddHours(5), counter, c1.GetDistanceTo(c2), item.Address));
                    }
                    counter++;
                }
                PointsByServicemanFlyoutVisible = Points.Count > 0;
            }, obj => SelectedDate != null);
        }
    }
}
