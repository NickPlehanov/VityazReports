using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models.GuardObjectsOnMapGBR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Controls.Primitives;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Threading;
using LiveCharts.Defaults;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.ComponentModel;
using GMap.NET.MapProviders;

namespace VityazReports.ViewModel {
    public class GuardObjectsOnMapGBRViewModel : BaseViewModel {
        private readonly A28Context context;
        private readonly CommonMethods commonMethods;

        public GuardObjectsOnMapGBRViewModel() {
            context = new A28Context();
            commonMethods = new CommonMethods();
            LoadingText = "";

            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gmaps_contol.MapProvider = GMap.NET.MapProviders.YandexMapProvider.Instance;
            gmaps_contol.MinZoom = 5;
            gmaps_contol.MaxZoom = 17;
            gmaps_contol.Zoom = 5;
            gmaps_contol.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            gmaps_contol.CanDragMap = true;
            gmaps_contol.DragButton = MouseButton.Left;
            gmaps_contol.CenterPosition = new PointLatLng(55.159904, 61.401919);

            GetObjTypes.Execute(null);

            ColorList.Add(new ColorModel() { Color = Brushes.Red, Isfree = true });//красный
            ColorList.Add(new ColorModel() { Color = Brushes.DarkBlue, Isfree = true });//синий
            ColorList.Add(new ColorModel() { Color = Brushes.Green, Isfree = true });//зеленый
            ColorList.Add(new ColorModel() { Color = Brushes.Black, Isfree = true });//черный
            ColorList.Add(new ColorModel() { Color = Brushes.Purple, Isfree = true });//фиолетовый
            ColorList.Add(new ColorModel() { Color = Brushes.Orange, Isfree = true });//оранжевый
            ColorList.Add(new ColorModel() { Color = Brushes.DodgerBlue, Isfree = true });//защитный голубой
            ColorList.Add(new ColorModel() { Color = Brushes.Aquamarine, Isfree = true });//аквамарин
            ColorList.Add(new ColorModel() { Color = Brushes.BurlyWood, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Crimson, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Lime, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Pink, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Khaki, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Magenta, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Indigo, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.LightSeaGreen, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Blue, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.DarkOrange, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.Olive, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.HotPink, Isfree = true });
            ColorList.Add(new ColorModel() { Color = Brushes.MediumTurquoise, Isfree = true });

        }
        /// <summary>
        /// Контрол карты
        /// </summary>
        public GMapControl gmaps_contol { get; set; } = new GMapControl();

        /// <summary>
        /// Список экипажей
        /// </summary>
        private ObservableCollection<ObjType> _ObjectTypeList = new ObservableCollection<ObjType>();
        public ObservableCollection<ObjType> ObjectTypeList {
            get => _ObjectTypeList;
            set {
                _ObjectTypeList = value;
                OnPropertyChanged(nameof(ObjectTypeList));
            }
        }

        /// <summary>
        /// Определяет видимость правой плавающей панели со списком типов объектов(маршрутов)
        /// </summary>
        private bool _FlyoutShowGroupsVisibleState;
        public bool FlyoutShowGroupsVisibleState {
            get => _FlyoutShowGroupsVisibleState;
            set {
                _FlyoutShowGroupsVisibleState = value;
                OnPropertyChanged(nameof(FlyoutShowGroupsVisibleState));
            }
        }
        /// <summary>
        /// Определяет видимость нижней плавающей панели с диаграммой
        /// </summary>
        private bool _ChartVisible;
        public bool ChartVisible {
            get => _ChartVisible;
            set {
                _ChartVisible = value;
                OnPropertyChanged(nameof(ChartVisible));
            }
        }
        /// <summary>
        /// Строки данных для диаграммы
        /// </summary>
        private SeriesCollection _ChartSeries;
        public SeriesCollection ChartSeries {
            get => _ChartSeries;
            set {
                _ChartSeries = value;
                OnPropertyChanged(nameof(ChartSeries));
            }
        }
        /// <summary>
        /// Легенда диаграммы для кастомизации
        /// </summary>
        private DefaultLegend _ChartLegent;
        public DefaultLegend ChartLegent {
            get => _ChartLegent;
            set {
                _ChartLegent = value;
                OnPropertyChanged(nameof(ChartLegent));
            }
        }
        /// <summary>
        /// Всплывающая подсказка на диаграмме, для кастомизации
        /// </summary>
        private DefaultTooltip _ChartToolTip;
        public DefaultTooltip ChartToolTip {
            get => _ChartToolTip;
            set {
                _ChartToolTip = value;
                OnPropertyChanged(nameof(ChartToolTip));
            }
        }
        /// <summary>
        /// Команда создания и настройки легенды графика
        /// </summary>
        private RelayCommand _CreateLegend;
        public RelayCommand CreateLegend {
            get => _CreateLegend ??= new RelayCommand(async obj => {
                ChartLegent = new DefaultLegend();
                ChartLegent.BulletSize = 15;
                ChartLegent.Foreground = Brushes.White;
                ChartLegent.Orientation = System.Windows.Controls.Orientation.Vertical;
            });
        }
        /// <summary>
        /// Команда создания и настройки всплывающей подсказки графика
        /// </summary>
        private RelayCommand _CreateToolTip;
        public RelayCommand CreateToolTip {
            get => _CreateToolTip ??= new RelayCommand(async obj => {
                ChartToolTip = new DefaultTooltip();
                ChartToolTip.Background = Brushes.Black;
                ChartToolTip.Foreground = Brushes.White;
            });
        }
        /// <summary>
        /// Команда получения списка маршрутов из базы
        /// </summary>
        private RelayCommand _GetObjTypes;
        public RelayCommand GetObjTypes {
            get => _GetObjTypes ??= new RelayCommand(async obj => {
                var _obj_types = context.ObjType.Where(x => x.RecordDeleted == false && x.ObjTypeName.ToLower().Contains("маршрут")).ToList();
                if (_obj_types.Count <= 0)
                    throw new Exception("Не найдено типов объектов");
                foreach (var item in _obj_types)
                    ObjectTypeList.Add(item);
            });
        }
        /// <summary>
        /// Плавающее меню со списком групп
        /// </summary>
        private RelayCommand _ShowGroups;
        public RelayCommand ShowGroups {
            get => _ShowGroups ??= new RelayCommand(async obj => {
                FlyoutShowGroupsVisibleState = !FlyoutShowGroupsVisibleState;
            });
        }
        /// <summary>
        /// список цветов для маршрутов
        /// </summary>
        private ObservableCollection<ColorModel> _ColorList = new ObservableCollection<ColorModel>();
        public ObservableCollection<ColorModel> ColorList {
            get => _ColorList;
            set {
                _ColorList = value;
                OnPropertyChanged(nameof(ColorList));
            }
        }
        /// <summary>
        /// Видимость индикатора загрузки
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
        /// Команда выбора группы. 
        /// Выбрав в плавающем меню экипаж, начинаем рисовать на карте объекты относящиеся к этому экипажу
        /// </summary>
        private RelayCommand _SelectGroupCommand;
        public RelayCommand SelectGroupCommand {
            get => _SelectGroupCommand ??= new RelayCommand(async obj => {
                ToggleButton toggleButton = obj as ToggleButton;
                //TODO: место потенциальной ошибки
                int? ObjId = int.Parse(toggleButton.Tag.ToString());
                if (!ObjId.HasValue)
                    throw new Exception("Не указан идентификатор объекта");
                List<ColorModel> cm = ColorList.Where(x => x.Isfree == true).ToList();

                //требуется проверить что такой маршрут уже есть/нет на карте
                var markers = gmaps_contol.Markers.Where(x => x.ZIndex.ToString() == ObjId.ToString()).ToList();
                foreach (GMapMarker m in markers)
                    gmaps_contol.Markers.Remove(m);
                ObjectTypeList.First(x => x.ObjTypeId == Int16.Parse(ObjId.ToString())).IsShowOnMap = false;
                if (ColorList.Any(x => x.ObjTypeId == ObjId.ToString())) {
                    ColorList.First(x => x.ObjTypeId == ObjId.ToString()).Isfree = true;
                    toggleButton.IsChecked = false;
                    toggleButton.Background = null;
                    toggleButton.Content = toggleButton.Content.ToString().Substring(0, 10);
                    cm = ColorList.Where(x => x.Isfree == true).ToList();
                }
                if (cm.Count <= 0) {
                    System.Windows.Forms.MessageBox.Show("Достигнуто ограничение по количеству одновременно отображаемых экипажей", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toggleButton.IsChecked = false;
                    toggleButton.Background = null;
                    toggleButton.Content = toggleButton.Content.ToString().Substring(0, 10);
                    return;
                }

                if (markers.Count <= 0) {
                    //а теперь можно добавлять на карту
                    string name = context.ObjType.FirstOrDefault(x => x.ObjTypeId == ObjId.Value).ObjTypeName;
                    if (string.IsNullOrEmpty(name))
                        return;
                    int number = commonMethods.ParseDigit(name);
                    if (number == 0)
                        return;
                    if (!context.ObjType.Any(x => x.RecordDeleted == false && x.ObjTypeName.Contains(number.ToString())))
                        return;
                    List<Models.GuardObjectsOnMapGBR.Object> ObjectsList = (from o in context.Object
                                                                            join ot in context.ObjType on o.ObjTypeId equals ot.ObjTypeId
                                                                            where ot.ObjTypeName.Contains(number.ToString())
                                                                            && o.RecordDeleted == false
                                                                            && ot.RecordDeleted == false
                                                                            && o.Latitude != null
                                                                            && o.Longitude != null
                                                                            select o).AsNoTracking().ToList();
                    if (ObjectsList.Count <= 0) {
                        System.Windows.Forms.MessageBox.Show("Для данного маршрута отсутсвуют объекты", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    foreach (Models.GuardObjectsOnMapGBR.Object item in ObjectsList) {
                        PointLatLng point = new PointLatLng((double)item.Latitude, (double)item.Longitude);
                        GMapMarker marker = new GMapMarker(point) {
                            Shape = new Ellipse {
                                Width = 12,
                                Height = 12,
                                Stroke = cm.First().Color,
                                StrokeThickness = 7.5,
                                ToolTip = Convert.ToString(item.ObjectNumber, 16) + " (" + item.Name + ")" + Environment.NewLine + item.Name + Environment.NewLine + item.Address,
                                AllowDrop = true
                            }
                        };
                        marker.ZIndex = (int)ObjId;
                        marker.Tag = item.ObjectId.ToString();
                        gmaps_contol.Markers.Add(marker);
                    }

                    toggleButton.Background = cm.First().Color;
                    toggleButton.Content += " (" + ObjectsList.Count.ToString() + ")";
                    ColorList.First(x => x.Color == cm.First().Color).Isfree = false;
                    ColorList.First(x => x.Color == cm.First().Color).ObjTypeId = ObjId.ToString();
                    ObjectTypeList.First(x => x.ObjTypeId == Int16.Parse(ObjId.ToString())).IsShowOnMap = true;
                    toggleButton.IsChecked = true;
                    //}
                }
            });
        }
        /// <summary>
        /// Таблица выводимая рядом с графиком отображающая информацию о времени прибытия и наименовании объекта
        /// </summary>
        private ObservableCollection<MatrixTotals> _ChartObjectsList = new ObservableCollection<MatrixTotals>();
        public ObservableCollection<MatrixTotals> ChartObjectsList {
            get => _ChartObjectsList;
            set {
                _ChartObjectsList = value;
                OnPropertyChanged(nameof(ChartObjectsList));
            }
        }
        /// <summary>
        /// Помещаем на карту метку ГБР
        /// Отлавливаем нажатие правой кнопки мыши, получаем позицию мыши на контроле, из которого вычисляем координаты и на них ставим точку.
        /// </summary>
        private RelayCommand _CreateLabelGbr;
        public RelayCommand CreateLabelGbr {
            get => _CreateLabelGbr ??= new RelayCommand(async obj => {
                MouseEventArgs mouseEventArgs = obj as MouseEventArgs;
                var y = mouseEventArgs.Source;
                if (mouseEventArgs.RightButton == MouseButtonState.Pressed) {
                    if (ObjectTypeList.Count(x => x.IsShowOnMap == true) == 1) {
                        if (gmaps_contol.Markers.Count(x => x.ZIndex.ToString() == "-1") == 0) {
                            Point p = mouseEventArgs.GetPosition((IInputElement)mouseEventArgs.Source);

                            GMapMarker marker = new GMapMarker(new PointLatLng()) {
                                Shape = new Ellipse {
                                    Width = 20,
                                    Height = 20,
                                    Stroke = Brushes.Chocolate,
                                    StrokeThickness = 7.5,
                                    ToolTip = "ГБР",
                                    AllowDrop = true
                                }
                            };
                            marker.Position = gmaps_contol.FromLocalToLatLng((int)p.X, (int)p.Y);
                            marker.ZIndex = -1;
                            marker.Tag = "ГБР";
                            gmaps_contol.Markers.Add(marker);
                            ChartSeries = null;
                        }
                        else
                            System.Windows.Forms.MessageBox.Show("На карте уже есть экипаж", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        System.Windows.Forms.MessageBox.Show("Необходимо выбрать хоть один маршрут", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
        /// <summary>
        /// Убираем с карты метку ГБР
        /// </summary>
        private RelayCommand _ClearGroup;
        public RelayCommand ClearGroup {
            get => _ClearGroup ??= new RelayCommand(async obj => {
                //TODO: задать вопрос на подтвреждение удаления
                var markers = gmaps_contol.Markers.Where(x => x.ZIndex.ToString() == "-1" || x.Tag == null).ToList();
                foreach (var item in markers) {
                    gmaps_contol.Markers.Remove(item);
                }
            });
        }

        private MatrixTotals _SelectedObject;
        public MatrixTotals SelectedObject {
            get => _SelectedObject;
            set {
                if (_SelectedObject == null && value != null) {
                    var _y = gmaps_contol.Markers.First(x => x.Tag.ToString() == value.Id.ToString());
                    if (_y == null)
                        return;
                    GMapMarker _m = _y as GMapMarker;
                    if (_m == null)
                        return;
                    Ellipse _ellipse = _m.Shape as Ellipse;
                    _ellipse.StrokeThickness = 12;
                    _ellipse.Stroke = Brushes.BlueViolet;
                    ObjAddress = context.Object.First(x => x.ObjectId == value.Id).Address;
                }
                if (_SelectedObject != null) {
                    if (_SelectedObject != value) {
                        if (value != null) {
                            var y = gmaps_contol.Markers.First(x => x.Tag.ToString() == _SelectedObject.Id.ToString());
                            if (y == null)
                                return;
                            GMapMarker m = y as GMapMarker;
                            if (m == null)
                                return;
                            Ellipse ellipse = m.Shape as Ellipse;
                            ellipse.StrokeThickness = 7.5;
                            ellipse.Stroke = Brushes.Red;
                            ObjAddress = context.Object.First(x => x.ObjectId == _SelectedObject.Id).Address;

                            var _y = gmaps_contol.Markers.First(x => x.Tag.ToString() == value.Id.ToString());
                            if (_y == null)
                                return;
                            GMapMarker _m = _y as GMapMarker;
                            if (_m == null)
                                return;
                            Ellipse _ellipse = _m.Shape as Ellipse;
                            _ellipse.StrokeThickness = 12;
                            _ellipse.Stroke = Brushes.BlueViolet;
                            ObjAddress = context.Object.First(x => x.ObjectId == value.Id).Address;
                        }
                    }
                    SelectedObjectCommand.Execute(_SelectedObject);
                }
                _SelectedObject = value;
                OnPropertyChanged(nameof(SelectedObject));
            }
        }

        private string _ObjAddress;
        public string ObjAddress {
            get => _ObjAddress;
            set {
                _ObjAddress = value;
                OnPropertyChanged(nameof(ObjAddress));
            }
        }

        private RelayCommand _SelectedObjectCommand;
        public RelayCommand SelectedObjectCommand {
            get => _SelectedObjectCommand ??= new RelayCommand(async obj => {

                var route_markers = gmaps_contol.Markers.Where(x => x.Tag == null).ToList();
                foreach (var item in route_markers)
                    gmaps_contol.Markers.Remove(item);


                List<PointLatLng> points = new List<PointLatLng>();
                foreach (var item in SelectedObject.coordinates)
                    points.Add(new PointLatLng(item[1], item[0]));

                MapRoute route = new MapRoute(points, "1");
                route.Tag = "route";
                GMapRoute gmRoute = new GMapRoute(route.Points);
                gmaps_contol.Markers.Add(gmRoute);
            });
        }

        private string _LoadingText;
        public string LoadingText {
            get => _LoadingText;
            set {
                _LoadingText = value;
                OnPropertyChanged(nameof(LoadingText));
            }
        }

        private RelayCommand _CalculateCommand;
        public RelayCommand CalculateCommand {
            get => _CalculateCommand ??= new RelayCommand(async obj => {
                //todo: проверять что series не null, если не null, то открывать просто 
                if (ChartSeries != null) {
                    ChartVisible = !ChartVisible;
                    return;
                }
                CreateLegend.Execute(null);
                CreateToolTip.Execute(null);
                await Dispatcher.CurrentDispatcher.Invoke(async () => {
                    ChartVisible = false;
                    Loading = true;
                    GMapMarker gbr = null;
                    gbr = gmaps_contol.Markers.FirstOrDefault(x => x.ZIndex.ToString() == "-1");
                    var objects = gmaps_contol.Markers.Where(x => x.ZIndex.ToString() != "-1").ToList();
                    ChartObjectsList.Clear();

                    if (gbr != null && objects != null) {
                        if (objects.Count > 0) {
                            using (HttpClient client = new HttpClient()) {
                                int counter = 1;
                                foreach (var item in objects) {
                                    LoadingText = string.Format("Обрабатывается {0} из {1}", counter.ToString(), objects.Count.ToString());
                                    string resp = @"http://router.project-osrm.org/route/v1/driving/" + gbr.Position.Lng.ToString().Replace(',', '.') + "," + gbr.Position.Lat.ToString().Replace(',', '.') + ";" + item.Position.Lng.ToString().Replace(',', '.') + "," + item.Position.Lat.ToString().Replace(',', '.') + "?geometries=geojson";
                                    HttpResponseMessage response = await client.GetAsync(resp);
                                    if (response.IsSuccessStatusCode) {
                                        //TODO: вероятно стоит сделать список и сохранять целиком ответ
                                        var osrm = JsonConvert.DeserializeObject<OSRM>(response.Content.ReadAsStringAsync().Result);
                                        if (osrm.code.Equals("Ok")) {
                                            if (osrm.routes.Count >= 1) {
                                                TimeSpan ts = TimeSpan.FromSeconds(osrm.routes[0].duration);
                                                ChartObjectsList.Add(new MatrixTotals(
                                                    osrm.routes[0].duration,
                                                    (item.Shape as Ellipse).ToolTip.ToString(),
                                                    ts.Minutes.ToString() + ":" + ts.Seconds.ToString(),
                                                    int.Parse(item.Tag.ToString()),
                                                    osrm.routes[0].geometry.coordinates
                                                    ));
                                                Ellipse el = item.Shape as Ellipse;
                                                el.ToolTip += Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                                            }
                                        }
                                    }
                                    counter++;
                                }
                            }
                            ChartVisible = true;

                            ChartSeries = new SeriesCollection() {
                            new PieSeries
                            {
                                Title = ">15",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(ChartObjectsList.Count(x => x.Duration > 900)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "12-15",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(ChartObjectsList.Count(x => x.Duration >= 720 && x.Duration < 900)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "7-12",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(ChartObjectsList.Count(x => x.Duration >= 420 && x.Duration < 720)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "5-7",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(ChartObjectsList.Count(x => x.Duration >= 300 && x.Duration < 420)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "<5",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(ChartObjectsList.Count(x => x.Duration < 300)) },
                                DataLabels = true
                            },
                        };
                            CreateLegend.Execute(null);

                            CreateToolTip.Execute(null);
                        }
                    }
                    else {
                        System.Windows.Forms.MessageBox.Show("Расчет маршрута невозможен. Не указано расположение ГБР на карте или не найдено охр. объектов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Loading = false;
                });
            });
        }
    }
}