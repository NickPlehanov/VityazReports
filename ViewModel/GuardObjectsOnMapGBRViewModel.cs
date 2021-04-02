using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Linq;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Diagnostics;
using Notifications.Wpf;
using System.ComponentModel;
using System.Threading.Tasks;
using VityazReports.Models;

namespace VityazReports.ViewModel {
    public class GuardObjectsOnMapGBRViewModel : BaseViewModel {
        private readonly A28Context context = new A28Context();
        private readonly ReportBaseContext reportContext = new ReportBaseContext();
        private readonly CommonMethods commonMethods = new CommonMethods();
        private readonly MsCRMContext CrmContext = new MsCRMContext();

        NotificationManager notificationManager = new NotificationManager();

        public GuardObjectsOnMapGBRViewModel() {
            LoadingText = "";
            ContextMenuIsOpen = true;
            FlyoutShowGroupsVisibleState = true;

            InitializeMapControl.Execute(null);

            GetObjTypes.Execute(null);

            InitializeColorList.Execute(null);

            CalculateCommandContent = "Рассчитать прибытие";

            notificationManager.Show(new NotificationContent {
                Title = "Информация",
                Message = "Кнопка помощи расположена справа снизу",
                Type = NotificationType.Information
            });
            flag = false;
        }
        /// <summary>
        /// Команда. Открывает PDF инструкцию по указанному пути
        /// </summary>
        private RelayCommand _HelpCommand;
        public RelayCommand HelpCommand {
            get => _HelpCommand ??= new RelayCommand(async obj => {
                if (File.Exists(@"\\server-nass\Install\WORKPLACE\Инструкции\Охраняемые объекты на карте.pdf"))
                    Process.Start(new ProcessStartInfo(@"\\server-nass\Install\WORKPLACE\Инструкции\Охраняемые объекты на карте.pdf") { UseShellExecute = true });
                else
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Файл инструкции не найден",
                        Type = NotificationType.Error
                    });
            });
        }
        /// <summary>
        /// Команда инициализации контрола карты
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
        /// Команда инициализации коллекции цветов для ГБР
        /// </summary>
        private RelayCommand _InitializeColorList;
        public RelayCommand InitializeColorList {
            get => _InitializeColorList ??= new RelayCommand(async obj => {
                ColorList.Add(new ColorModel(Brushes.Red));//красный
                ColorList.Add(new ColorModel(Brushes.DarkBlue));//синий
                ColorList.Add(new ColorModel(Brushes.Green));//зеленый
                ColorList.Add(new ColorModel(Brushes.Black));//черный
                ColorList.Add(new ColorModel(Brushes.Purple));//фиолетовый
                ColorList.Add(new ColorModel(Brushes.Orange));//оранжевый
                ColorList.Add(new ColorModel(Brushes.DodgerBlue));//защитный голубой
                ColorList.Add(new ColorModel(Brushes.Aquamarine));//аквамарин
                ColorList.Add(new ColorModel(Brushes.BurlyWood));
                ColorList.Add(new ColorModel(Brushes.Crimson));
                ColorList.Add(new ColorModel(Brushes.Lime));
                ColorList.Add(new ColorModel(Brushes.Pink));
                ColorList.Add(new ColorModel(Brushes.Khaki));
                ColorList.Add(new ColorModel(Brushes.Magenta));
                ColorList.Add(new ColorModel(Brushes.Indigo));
                ColorList.Add(new ColorModel(Brushes.LightSeaGreen));
                ColorList.Add(new ColorModel(Brushes.Blue));
                ColorList.Add(new ColorModel(Brushes.DarkOrange));
                ColorList.Add(new ColorModel(Brushes.Olive));
                ColorList.Add(new ColorModel(Brushes.HotPink));
                ColorList.Add(new ColorModel(Brushes.MediumTurquoise));
            });
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
        /// Коллекция хранящая список сохраненных расчетов прибытий
        /// </summary>
        private ObservableCollection<SeriesCustomCollection> _SeriesCollectionList = new ObservableCollection<SeriesCustomCollection>();
        public ObservableCollection<SeriesCustomCollection> SeriesCollectionList {
            get => _SeriesCollectionList;
            set {
                _SeriesCollectionList = value;
                OnPropertyChanged(nameof(SeriesCollectionList));
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
                ChartLegent = new DefaultLegend {
                    BulletSize = 15,
                    Foreground = Brushes.White,
                    Orientation = System.Windows.Controls.Orientation.Vertical
                };
            });
        }
        /// <summary>
        /// Команда создания и настройки всплывающей подсказки графика
        /// </summary>
        private RelayCommand _CreateToolTip;
        public RelayCommand CreateToolTip {
            get => _CreateToolTip ??= new RelayCommand(async obj => {
                ChartToolTip = new DefaultTooltip {
                    Background = Brushes.Black,
                    Foreground = Brushes.White
                };
            });
        }
        /// <summary>
        /// Свойство определяющее видимость панель анализа расстояния между центром города и охраняемым объектом
        /// </summary>
        private bool _AnalyzeVisible;
        public bool AnalyzeVisible {
            get => _AnalyzeVisible;
            set {
                _AnalyzeVisible = value;
                OnPropertyChanged(nameof(AnalyzeVisible));
            }
        }
        /// <summary>
        /// Команда. Управляет видимостью панель с результами анализа расстояния
        /// </summary>
        private RelayCommand _ChangeVisibleAnalyzeCommand;
        public RelayCommand ChangeVisibleAnalyzeCommand {
            get => _ChangeVisibleAnalyzeCommand ??= new RelayCommand(async obj => {
                if (FarDistanceList.Count() <= 0)
                    GetCountFarObjectCommand.Execute(null);
                else
                    AnalyzeVisible = !AnalyzeVisible;
            }, obj => ObjectTypeList.Count(x => x.IsShowOnMap == true) >= 1);
        }
        /// <summary>
        /// Хранит выбранный маршрут
        /// </summary>
        private ObjType _SelectedObjType;
        public ObjType SelectedObjType {
            get => _SelectedObjType;
            set {
                _SelectedObjType = value;
                OnPropertyChanged(nameof(SelectedObjType));
            }
        }
        /// <summary>
        /// Команда получения количества объектов удаленных от центра Челябинска, более чем на N километров (Анализ расстояния)
        /// </summary>
        private RelayCommand _GetCountFarObjectCommand;
        public RelayCommand GetCountFarObjectCommand {
            get => _GetCountFarObjectCommand ??= new RelayCommand(async obj => {
                notificationManager.Show(new NotificationContent {
                    Title = "Информация",
                    Message = "Анализ расстояния по объектам начат и будет продолжен в фоновом режиме, по окончанию анализа, откроется окно с результатами",
                    Type = NotificationType.Information
                });
                SelectedObjType = ObjectTypeList.FirstOrDefault(x => x.IsShowOnMap == true);
                if (SelectedObjType == null)
                    return;

                //TODO: вынести в ini расстояние и координаты центра от которого считаем
                //За центр Челябинска примем центральную точку GmapControl
                PointLatLng center = gmaps_contol.CenterPosition;
                int? Nkm = 50;
                if (center == null)
                    return;
                int number = commonMethods.ParseDigit(ObjectTypeList.FirstOrDefault(x => x.IsShowOnMap == true).ObjTypeName);
                if (number == 0)
                    return;
                List<Models.GuardObjectsOnMapGBR.Object> ObjectsList = (from o in context.Object
                                                                        join ot in context.ObjType on o.ObjTypeId equals ot.ObjTypeId
                                                                        where ot.ObjTypeName.Contains(number.ToString())
                                                                        && o.RecordDeleted == false
                                                                        && ot.RecordDeleted == false
                                                                        select o).AsNoTracking().ToList();
                if (!ObjectsList.Any())
                    return;
                List<Task> Tasks = new List<Task>();
                await Dispatcher.CurrentDispatcher.InvokeAsync((Action)delegate {
                    FarDistanceList.Clear();
                });
                HttpClient client = new HttpClient();
                int counter = 1;
                foreach (var item in ObjectsList) {
                    //Tasks.Add(new Task((Action)async delegate {
                    if (!SelectedObjType.Equals(ObjectTypeList.FirstOrDefault(x => x.IsShowOnMap == true))) {
                        LoadingText = null;
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "Анализ расстояния по объектам был завершен, так как изменились объекты на карте",
                            Type = NotificationType.Error
                        });
                        return;
                    }
                    LoadingText = string.Format("Обрабатывается {0} из {1}", counter.ToString(), ObjectsList.Count.ToString());
                    if (item.Latitude != null && item.Longitude != null) {
                        string resp = @"http://router.project-osrm.org/route/v1/driving/" + center.Lng.ToString().Replace(',', '.') + "," + center.Lat.ToString().Replace(',', '.') + ";" + item.Longitude.ToString().Replace(',', '.') + "," + item.Latitude.ToString().Replace(',', '.') + "?geometries=geojson";
                        HttpResponseMessage response = await client.GetAsync(resp);
                        if (response.IsSuccessStatusCode) {
                            var osrm = JsonConvert.DeserializeObject<OSRM>(response.Content.ReadAsStringAsync().Result);
                            if (osrm.code.Equals("Ok")) //так же обработать noRoutes
                                if (osrm.routes.Count >= 1)
                                    if (osrm.routes[0].distance / 1000 > Nkm)
                                        await Dispatcher.CurrentDispatcher.InvokeAsync((Action)delegate {
                                            FarDistanceList.Add(new FarDistanceModel(Convert.ToInt32(Convert.ToString(item.ObjectNumber, 16)), item.Name, item.Address, osrm.routes[0].distance / 1000));
                                        });
                            if (osrm.code.Equals("NoRoute"))
                                await Dispatcher.CurrentDispatcher.InvokeAsync((Action)delegate {
                                    FarDistanceList.Add(new FarDistanceModel(Convert.ToInt32(Convert.ToString(item.ObjectNumber, 16)), item.Name, item.Address, -1));
                                });
                        }
                        else {
                            await Dispatcher.CurrentDispatcher.InvokeAsync((Action)delegate {
                                FarDistanceList.Add(new FarDistanceModel(Convert.ToInt32(Convert.ToString(item.ObjectNumber, 16)), item.Name, item.Address, -1));
                            });
                        }
                    }
                    else {
                        await Dispatcher.CurrentDispatcher.InvokeAsync((Action)delegate {
                            FarDistanceList.Add(new FarDistanceModel(Convert.ToInt32(Convert.ToString(item.ObjectNumber, 16)), item.Name, item.Address, -1));
                        });
                    }
                    counter++;
                    //},TaskCreationOptions.RunContinuationsAsynchronously));
                }
                //var block = 50;
                //var numberblocks = Tasks.Count / 50;
                //for (int i=0;i<(int)Math.Ceiling((double)(Tasks.Count / 50))+1;i++) {
                //    var current = Tasks.Skip(i * 50).Take(50);
                //    await Task.WhenAll(current);
                //}
                //await Task.WhenAll(Tasks);
                AnalyzeVisible = true;
                LoadingText = null;
            }, obj => ObjectTypeList.Count(x => x.IsShowOnMap == true) >= 1);
        }
        /// <summary>
        /// Коллекция хранящая список результатов анализа расстояния
        /// </summary>
        private ObservableCollection<FarDistanceModel> _FarDistanceList = new ObservableCollection<FarDistanceModel>();
        public ObservableCollection<FarDistanceModel> FarDistanceList {
            get => _FarDistanceList;
            set {
                _FarDistanceList = value;
                OnPropertyChanged(nameof(FarDistanceList));
            }
        }
        /// <summary>
        /// Команда получения списка маршрутов из базы
        /// </summary>
        private RelayCommand _GetObjTypes;
        public RelayCommand GetObjTypes {
            get => _GetObjTypes ??= new RelayCommand(async obj => {
                var _obj_types = context.ObjType.Where(x => x.RecordDeleted == false && x.ObjTypeName.ToLower().Contains("маршрут")).AsNoTracking().ToList();
                if (_obj_types.Count <= 0)
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Типы объектов не найдены",
                        Type = NotificationType.Error
                    });
                foreach (var item in _obj_types) {
                    int number = commonMethods.ParseDigit(item.ObjTypeName);
                    if (number == 0)
                        continue;
                    int? _obj_count = (from o in context.Object
                                       join ot in context.ObjType on o.ObjTypeId equals ot.ObjTypeId
                                       where ot.ObjTypeName.Contains(number.ToString())
                                       && o.RecordDeleted == false
                                       && ot.RecordDeleted == false
                                       && o.Latitude != null
                                       && o.Longitude != null
                                       select o).AsNoTracking().ToList().Count;
                    ObjectTypeList.Add(new ObjType(item.ObjTypeId, item.OrderNumber, item.ObjTypeName, item.Description, item.RecordDeleted, false, item.ObjTypeName, _obj_count));
                }
            });
        }
        /// <summary>
        /// Команда закрытия окна, 
        /// </summary>
        private RelayCommand _WindowCloseCommand;
        public RelayCommand WindowCloseCommand {
            get => _WindowCloseCommand ??= new RelayCommand(async obj => {
                notificationManager = null;
            });
        }

        private RelayCommand _UpdateObjTypes;
        public RelayCommand UpdateObjTypes {
            get => _UpdateObjTypes ??= new RelayCommand(async obj => {
                if (ObjectTypeList == null) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Невозможно обновить список типов объектов, так как типы объектов не найдены",
                        Type = NotificationType.Error
                    });
                    return;
                }
                foreach (var item in ObjectTypeList) {
                    int number = commonMethods.ParseDigit(item.ObjTypeName);
                    if (number == 0)
                        continue;
                    int? _obj_count = (from o in context.Object
                                       join ot in context.ObjType on o.ObjTypeId equals ot.ObjTypeId
                                       where ot.ObjTypeName.Contains(number.ToString())
                                       && o.RecordDeleted == false
                                       && ot.RecordDeleted == false
                                       && o.Latitude != null
                                       && o.Longitude != null
                                       select o).AsNoTracking().ToList().Count;
                    item.ObjTypeName = item.ObjTypeName;
                    item.Name = item.ObjTypeName;
                    item.CountObjects = _obj_count;
                }
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
        /// Обновление списка маркеров
        /// </summary>
        private RelayCommand _UpdateMarkers;
        public RelayCommand UpdateMarkers {
            get => _UpdateMarkers ??= new RelayCommand(async obj => {
                await Dispatcher.CurrentDispatcher.Invoke(async () => {
                    Loading = true;
                    UpdateObjTypes.Execute(null);
                    SelectedRoute = ObjectTypeList.FirstOrDefault(x => x.IsShowOnMap == true).TgBtn;
                    SelectGroupCommand.Execute(SelectedRoute);
                    SelectGroupCommand.Execute(SelectedRoute);
                    ContextMenuIsOpen = false;
                    Loading = false;
                    notificationManager.Show(new NotificationContent {
                        Title = "Успех",
                        Message = "Обновление данных завершено",
                        Type = NotificationType.Success
                    });
                });
            }, obj => ObjectTypeList.Count(x => x.IsShowOnMap == true) == 1);
        }

        private RelayCommand _CreateReportCommand;
        public RelayCommand CreateReportCommand {
            get => _CreateReportCommand ??= new RelayCommand(async obj => {
                if (ChartObjectsList.Count <= 0) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Невозможно выгрузить отчёт, данные для построения не обнаружены",
                        Type = NotificationType.Error
                    });
                    return;
                }
                if (ChartObjectsList.Count(x => x.Duration > 900) <= 0) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Невозможно выгрузить отчёт, прибытий на объекты более 15 минут нет. Операция будет прервана",
                        Type = NotificationType.Error
                    });
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV-file (.csv)|*.csv";
                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    //using (StreamWriter sw = new StreamWriter("1.csv", true, System.Text.Encoding.UTF8)) {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, true, System.Text.Encoding.UTF8)) {
                        foreach (MatrixTotals item in ChartObjectsList.Where(x => x.Duration > 900).OrderByDescending(x => x.Duration)) {
                            await sw.WriteLineAsync(item.DurationText + ";" + item.ObjectInfo.Replace(Environment.NewLine, ";"));
                        }
                    }
                }
                notificationManager.Show(new NotificationContent {
                    Title = "Успех",
                    Message = "Отчёт сохранен",
                    Type = NotificationType.Success
                });
            }, obj => ChartObjectsList.Count > 0);
        }

        private ToggleButton _SelectedRoute;
        public ToggleButton SelectedRoute {
            get => _SelectedRoute;
            set {
                _SelectedRoute = value;
                OnPropertyChanged(nameof(SelectedRoute));
            }
        }
        /// <summary>
        /// Команда выбора группы. 
        /// Выбрав в плавающем меню экипаж, начинаем рисовать на карте объекты относящиеся к этому экипажу
        /// </summary>
        private RelayCommand _SelectGroupCommand;
        public RelayCommand SelectGroupCommand {
            get => _SelectGroupCommand ??= new RelayCommand(async obj => {
                if (!(obj is ToggleButton toggleButton))
                    return;
                int? ObjId = int.Parse(toggleButton.Tag.ToString());
                if (!ObjId.HasValue)
                    return;
                List<ColorModel> cm = ColorList.Where(x => x.Isfree == true).ToList();
                App.Current.Dispatcher.Invoke((System.Action)delegate {
                    Loading = true;
                    //требуется проверить что такой маршрут уже есть/нет на карте
                    var markers = gmaps_contol.Markers.Where(x => x.ZIndex.ToString() == ObjId.ToString()).ToList();
                    if (markers.Count > 0) {
                        //ChartSeries = null;
                        //ChartVisible = false;
                        ClearGroup.Execute(null);
                        SeriesCollectionList.Clear();
                    }

                    foreach (GMapMarker m in markers)
                        gmaps_contol.Markers.Remove(m);
                    //ObjectTypeList.First(x => x.ObjTypeId == Int16.Parse(ObjId.ToString())).IsShowOnMap = false;
                    if (ColorList.Any(x => x.ObjTypeId == ObjId.ToString())) {
                        ColorList.First(x => x.ObjTypeId == ObjId.ToString()).Isfree = true;
                        toggleButton.IsChecked = false;
                        toggleButton.Background = null;
                        cm = ColorList.Where(x => x.Isfree == true).ToList();
                    }
                    if (cm.Count <= 0) {
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "Достигнуто ограничение по количеству одновременно отображаемых экипажей",
                            Type = NotificationType.Error
                        });
                        toggleButton.IsChecked = false;
                        toggleButton.Background = null;
                        Loading = false;
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
                            notificationManager.Show(new NotificationContent {
                                Title = "Ошибка",
                                Message = "Для данного маршрута отсутсвуют объекты",
                                Type = NotificationType.Error
                            });
                            Loading = false;
                            return;
                        }
                        //Добавляем метку ГБР из базы данных (CRM - расположение экипажей)
                        NewPlacesGbrextensionBase gbr_place = CrmContext.NewPlacesGbrextensionBase.FirstOrDefault(x => x.NewName.Contains(number.ToString()));
                        if (gbr_place == null) {
                            notificationManager.Show(new NotificationContent {
                                Title = "Ошибка",
                                Message = "Не найдено сохраненное расположение экипажа на карте",
                                Type = NotificationType.Error
                            });
                        }
                        else {
                            if (!string.IsNullOrEmpty(gbr_place.NewLatitude) && !string.IsNullOrEmpty(gbr_place.NewLongitude)) {
                                GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(gbr_place.NewLatitude), Convert.ToDouble(gbr_place.NewLongitude))) {
                                    //Shape = new Ellipse {
                                    //    Width = 20,
                                    //    Height = 20,
                                    //    Stroke = Brushes.Chocolate,
                                    //    StrokeThickness = 7.5,
                                    //    ToolTip = "ГБР",
                                    //    AllowDrop = true
                                    //}
                                    Shape = new System.Windows.Controls.Image() {
                                        Source = Conv.ToImageSource(Properties.Resources.Icon),
                                        Stretch = Stretch.UniformToFill,
                                        ToolTip = gbr_place.NewName
                                    }
                                };
                                marker.ZIndex = 1000;
                                marker.Tag = "ГБР";
                                gmaps_contol.Markers.Add(marker);
                            }
                        }
                        foreach (Models.GuardObjectsOnMapGBR.Object item in ObjectsList) {
                            PointLatLng point = new PointLatLng((double)item.Latitude, (double)item.Longitude);
                            GMapMarker marker = new GMapMarker(point) {
                                Shape = new Ellipse {
                                    Width = 12,
                                    Height = 12,
                                    Stroke = cm.First().Color,
                                    StrokeThickness = 7.5,
                                    ToolTip = Convert.ToString(item.ObjectNumber, 16) + Environment.NewLine + item.Name + Environment.NewLine + item.Address,
                                    AllowDrop = true
                                }
                            };
                            marker.ZIndex = (int)ObjId;
                            marker.Shape.MouseLeftButtonDown += Shape_MouseLeftButtonDownAsync;
                            marker.Tag = item.ObjectId.ToString();
                            gmaps_contol.Markers.Add(marker);
                        }

                        toggleButton.Background = cm.First().Color;
                        ColorList.First(x => x.Color == cm.First().Color).Isfree = false;
                        ColorList.First(x => x.Color == cm.First().Color).ObjTypeId = ObjId.ToString();
                        ObjectTypeList.First(x => x.ObjTypeId == Int16.Parse(ObjId.ToString())).IsShowOnMap = true;
                        ObjectTypeList.First(x => x.ObjTypeId == Int16.Parse(ObjId.ToString())).TgBtn = toggleButton;
                        toggleButton.IsChecked = true;
                        AddSeriesCollection.Execute(null);
                    }
                    Loading = false;
                });
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
            GMapMarker gbr = null;
            gbr = gmaps_contol.Markers.FirstOrDefault(x => x.ZIndex.ToString() == "1000");
            if (gbr == null) {
                notificationManager.Show(new NotificationContent {
                    Title = "Ошибка",
                    Message = "Нельзя построить маршрут, на карте не расположен ГБР",
                    Type = NotificationType.Error
                });
                return;
            }
            using (HttpClient client = new HttpClient()) {
                string resp = @"http://router.project-osrm.org/route/v1/driving/" + gbr.Position.Lng.ToString().Replace(',', '.') + "," + gbr.Position.Lat.ToString().Replace(',', '.') + ";" + marker.Position.Lng.ToString().Replace(',', '.') + "," + marker.Position.Lat.ToString().Replace(',', '.') + "?geometries=geojson";
                HttpResponseMessage response = client.GetAsync(resp).Result;
                if (response.IsSuccessStatusCode) {
                    //TODO: вероятно стоит сделать список и сохранять целиком ответ
                    var osrm = JsonConvert.DeserializeObject<OSRM>(response.Content.ReadAsStringAsync().Result);
                    if (osrm.code.Equals("Ok")) {
                        if (osrm.routes.Count >= 1) {
                            TimeSpan ts = TimeSpan.FromSeconds(osrm.routes[0].duration);
                            Ellipse el = marker.Shape as Ellipse;
                            el.ToolTip = el.ToolTip.ToString().LastIndexOf(':') != -1 ?
                            el.ToolTip.ToString().Substring(0, el.ToolTip.ToString().LastIndexOf(':') - 2) + Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString() :
                            el.ToolTip + Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();

                            notificationManager.Show(new NotificationContent {
                                Title = "Информация",
                                Message = el.ToolTip.ToString(),
                                Type = NotificationType.Information
                            });

                            var route_markers = gmaps_contol.Markers.Where(x => x.Tag == null).ToList();
                            foreach (var item in route_markers)
                                gmaps_contol.Markers.Remove(item);


                            List<PointLatLng> points = new List<PointLatLng>();
                            foreach (var item in osrm.routes[0].geometry.coordinates)
                                points.Add(new PointLatLng(item[1], item[0]));

                            MapRoute route = new MapRoute(points, "1");
                            route.Tag = "route";
                            GMapRoute gmRoute = new GMapRoute(route.Points);
                            gmaps_contol.Markers.Add(gmRoute);
                        }
                    }
                    else {
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "Маршрут не найден",
                            Type = NotificationType.Error
                        });
                        return;
                    }
                }
                else {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Ошибка выполнения запроса. Маршрут не был построен",
                        Type = NotificationType.Error
                    });
                    return;
                }
            }
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

        private TimeSpan _StartSpan;
        public TimeSpan StartSpan {
            get => _StartSpan;
            set {
                _StartSpan = value;
                OnPropertyChanged(nameof(StartSpan));
            }
        }
        /// <summary>
        /// определяет состояние открыто/закрыто контекстного меню на карте
        /// </summary>
        private bool _ContextMenuIsOpen;
        public bool ContextMenuIsOpen {
            get => _ContextMenuIsOpen;
            set {
                _ContextMenuIsOpen = value;
                OnPropertyChanged(nameof(ContextMenuIsOpen));
            }
        }
        /// <summary>
        /// Помещаем на карту метку ГБР
        /// Отлавливаем нажатие правой кнопки мыши, получаем позицию мыши на контроле, из которого вычисляем координаты и на них ставим точку.
        /// </summary>
        private RelayCommand _CreateLabelGbr;
        public RelayCommand CreateLabelGbr {
            get => _CreateLabelGbr ??= new RelayCommand(async obj => {
                var u = obj as System.Windows.Controls.Button;
                if (u == null)
                    return;
                if (ObjectTypeList.Count(x => x.IsShowOnMap == true) == 1) {
                    if (gmaps_contol.Markers.Count(x => x.ZIndex.ToString() == "1000") == 0) {
                        Point point = u.TranslatePoint(new Point(), gmaps_contol);
                        GMapMarker marker = new GMapMarker(new PointLatLng()) {
                            //Shape = new Ellipse {
                            //    Width = 20,
                            //    Height = 20,
                            //    Stroke = Brushes.Chocolate,
                            //    StrokeThickness = 7.5,
                            //    ToolTip = "ГБР",
                            //    AllowDrop = true
                            //}
                            Shape = new System.Windows.Controls.Image() {
                                Source = Conv.ToImageSource(Properties.Resources.Icon),
                                Stretch = Stretch.UniformToFill,
                                ToolTip = "ГБР"
                            }
                        };
                        marker.Position = gmaps_contol.FromLocalToLatLng((int)point.X, (int)point.Y);
                        marker.ZIndex = 1000;
                        marker.Tag = "ГБР";
                        marker.Offset = new Point(-35, -15);
                        gmaps_contol.Markers.Add(marker);
                        ChartSeries = null;
                        //SeriesCollectionList.Clear();
                        ChartVisible = false;
                    }
                    else
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "На карте уже есть экипаж",
                            Type = NotificationType.Error
                        });
                }
                else {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Необходимо выбрать хоть один маршрут",
                        Type = NotificationType.Error
                    });
                    FlyoutShowGroupsVisibleState = true;
                }
                ContextMenuIsOpen = false;
            });
        }
        /// <summary>
        /// Убираем с карты метку ГБР
        /// </summary>
        private RelayCommand _ClearGroup;
        public RelayCommand ClearGroup {
            get => _ClearGroup ??= new RelayCommand(async obj => {
                //TODO: задать вопрос на подтвреждение удаления
                ChartVisible = false;
                var markers = gmaps_contol.Markers.Where(x => x.ZIndex.ToString() == "1000" || x.Tag == null).ToList();
                foreach (var item in markers) {
                    gmaps_contol.Markers.Remove(item);
                }
                var route_markers = gmaps_contol.Markers.Where(x => x.Tag == null || x.Tag == "route").ToList();
                foreach (var item in route_markers)
                    gmaps_contol.Markers.Remove(item);

                //ChartObjectsList.Clear();
            }, obj => gmaps_contol.Markers.Count(x => x.ZIndex.ToString() == "1000" || x.Tag == null) != 0);
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
                //MatrixTotals mt = obj as MatrixTotals;
                //if (mt != null) {
                //    GMapMarker mr = gmaps_contol.Markers.FirstOrDefault(x => x.ZIndex == mt.Id);
                //    if (mr != null)
                //        gmaps_contol.ZoomAndCenterMarkers(mr.ZIndex);
                ////}
                //GMapMarker mr = gmaps_contol.Markers.FirstOrDefault(x => x.Tag.ToString() == SelectedObject.Id.ToString());
                //if (mr != null)
                //    gmaps_contol.ZoomAndCenterMarkers(mr.ZIndex);                

                var route_markers = gmaps_contol.Markers.Where(x => x.Tag == null).ToList();
                foreach (var item in route_markers)
                    gmaps_contol.Markers.Remove(item);

                if (SelectedObject.coordinates != null) {
                    List<PointLatLng> points = new List<PointLatLng>();
                    foreach (var item in SelectedObject.coordinates)
                        points.Add(new PointLatLng(item[1], item[0]));

                    MapRoute route = new MapRoute(points, "1");
                    route.Tag = "route";
                    GMapRoute gmRoute = new GMapRoute(route.Points);
                    gmaps_contol.Markers.Add(gmRoute);
                }
                else {
                    if (flag) {
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "Для загруженных расчетов построение маршрута невозможно" + Environment.NewLine + Environment.NewLine + "Вы можете нажать на метку объекта, для построения маршрута на текущий момент",
                            Type = NotificationType.Error
                        });
                        flag = false;
                    }
                }
            });
        }
        /// <summary>
        /// Определяем показ сообщений
        /// </summary>
        private bool _flag;
        public bool flag {
            get => _flag;
            set {
                _flag = value;
                OnPropertyChanged(nameof(flag));
            }
        }
        private string _LoadingText;
        public string LoadingText {
            get => _LoadingText;
            set {
                _LoadingText = value;
                OnPropertyChanged(nameof(LoadingText));
            }
        }

        private string _CalculateCommandContent;
        public string CalculateCommandContent {
            get => _CalculateCommandContent;
            set {
                _CalculateCommandContent = value;
                OnPropertyChanged(nameof(CalculateCommandContent));
            }
        }

        private RelayCommand _CalculateCommand;
        public RelayCommand CalculateCommand {
            get => _CalculateCommand ??= new RelayCommand(async obj => {
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
                    gbr = gmaps_contol.Markers.FirstOrDefault(x => x.ZIndex.ToString() == "1000");
                    var objects = gmaps_contol.Markers.Where(x => x.ZIndex.ToString() != "1000").ToList();
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
                                                    ts.Minutes.ToString() + ":" + ts.Seconds.ToString(),
                                                    (item.Shape as Ellipse).ToolTip.ToString().LastIndexOf(':') != -1 ?
                                                    (item.Shape as Ellipse).ToolTip.ToString().Substring(0, (item.Shape as Ellipse).ToolTip.ToString().LastIndexOf(':') - 2) + Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString() :
                                                (item.Shape as Ellipse).ToolTip + Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString(),
                                                    int.Parse(item.Tag.ToString()),
                                                    osrm.routes[0].geometry.coordinates
                                                    ));
                                                Ellipse el = item.Shape as Ellipse;
                                                el.ToolTip = el.ToolTip.ToString().LastIndexOf(':') != -1 ?
                                                el.ToolTip.ToString().Substring(0, el.ToolTip.ToString().LastIndexOf(':') - 2) + Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString() :
                                                el.ToolTip + Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                                                //el.ToolTip += Environment.NewLine + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
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
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "Расчет маршрута невозможен. Не указано расположение ГБР на карте или не найдено охр. объектов",
                            Type = NotificationType.Error
                        });
                    }
                    CalculateCommandContent = "Показать/скрыть расчёт";
                    Loading = false;
                    PrivateID = null;
                });
            }, obj => gmaps_contol.Markers.Count(x => x.ZIndex.ToString() == "1000") == 1 && gmaps_contol.Markers.Count(x => x.ZIndex.ToString() != "1000") > 0 && ObjectTypeList.Count(x => x.IsShowOnMap == true) == 1);
        }

        private SeriesCustomCollection _SelectedSeriesCollection;
        public SeriesCustomCollection SelectedSeriesCollection {
            get => _SelectedSeriesCollection;
            set {
                _SelectedSeriesCollection = value;
                OnPropertyChanged(nameof(SelectedSeriesCollection));
            }
        }

        private string _PrivateID;
        public string PrivateID {
            get => _PrivateID;
            set {
                _PrivateID = value;
                OnPropertyChanged(nameof(PrivateID));
            }
        }
        /// <summary>
        /// Выбираем расчет, которые выгрузили из БД (дата время сохранения отчета является именем)
        /// </summary>
        private RelayCommand _ChangeSeriesCommand;
        public RelayCommand ChangeSeriesCommand {
            get => _ChangeSeriesCommand ??= new RelayCommand(async obj => {
                await Dispatcher.CurrentDispatcher.Invoke(async () => {
                    Loading = true;
                    if (SeriesCollectionList == null)
                        return;
                    if (SeriesCollectionList.Count <= 0)
                        return;
                    SeriesCustomCollection _id = obj as SeriesCustomCollection;
                    if (_id == null)
                        return;
                    if (PrivateID != null)
                        if (PrivateID.Equals(_id.Id)) {
                            ChartVisible = !ChartVisible;
                            return;
                        }

                    var route_markers = gmaps_contol.Markers.Where(x => x.Tag == null).ToList();
                    foreach (var item in route_markers)
                        gmaps_contol.Markers.Remove(item);

                    ChartObjectsList.Clear();
                    var gbrs = gmaps_contol.Markers.Where(x => x.ZIndex == 1000).ToList();
                    if (gbrs != null) {
                        foreach (var item in gbrs) {
                            gmaps_contol.Markers.Remove(item);
                        }
                    }
                    var col = SeriesCollectionList.FirstOrDefault(x => x.Id.Equals(_id.Id));

                    PrivateID = col.Id;
                    ChartSeries = col.SeriesCollectionBuild;
                    foreach (var item in col.COL.ToList()) {
                        var _obj = context.Object.FirstOrDefault(x => x.ObjectId == item.Id);
                        ChartObjectsList.Add(new MatrixTotals(
                            duration: item.Duration,
                            durationText: TimeSpan.FromSeconds(item.Duration).Minutes.ToString() + ":" + TimeSpan.FromSeconds(item.Duration).Seconds.ToString(),
                            objectInfo: Convert.ToString(_obj.ObjectNumber, 16) + Environment.NewLine + _obj.Name,
                            item.Id,
                            null
                            ));
                    }
                    if (col.Latitude.HasValue && col.Longitude.HasValue) {
                        //Добавляем экипаж на карту
                        SelectedRoute = ObjectTypeList.FirstOrDefault(x => x.IsShowOnMap == true).TgBtn;
                        string name = null;
                        if (SelectedRoute != null)
                            name = SelectedRoute.Content.ToString();
                        GMapMarker marker = new GMapMarker(new PointLatLng((double)col.Latitude, (double)col.Longitude)) {
                            Shape = new System.Windows.Controls.Image() {
                                Source = Conv.ToImageSource(Properties.Resources.Icon),
                                Stretch = Stretch.UniformToFill,
                                ToolTip= name
                            }
                        };
                        marker.ZIndex = 1000;
                        marker.Tag = "ГБР";
                        gmaps_contol.Markers.Add(marker);
                    }

                    ChartVisible = true;
                    CreateLegend.Execute(null);
                    CreateToolTip.Execute(null);
                    Loading = false;
                });
            });
        }
        /// <summary>
        /// Команда сохранения текущего расчета в коллекцию
        /// </summary>
        private RelayCommand _SaveChartSeriesCommand;
        public RelayCommand SaveChartSeriesCommand {
            get => _SaveChartSeriesCommand ??= new RelayCommand(async obj => {
                if (ChartSeries == null) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Сохранение пустой коллекции невозможно",
                        Type = NotificationType.Error
                    });
                    return;
                }
                var gbr = gmaps_contol.Markers.First(x => x.ZIndex == 1000);
                int objtype = gmaps_contol.Markers.First(x => x.ZIndex != 1000).ZIndex;
                Guid ClcKey = Guid.NewGuid();
                string nm = DateTime.Now.ToString();
                foreach (var item in ChartObjectsList) {
                    reportContext.CalculatedRoutes.Add(new Models.MainWindow.CalculatedRoutes() {
                        ClcRecordId = Guid.NewGuid(),
                        ClcObjectId = item.Id,
                        ClcDuration = item.Duration,
                        ClcGroup = objtype,
                        ClcKeyCalcId = ClcKey,
                        ClcCalcName = nm,
                        ClcGroupLatitude = gbr.Position.Lat,
                        ClcGroupLongitude = gbr.Position.Lng
                    });
                }
                await reportContext.SaveChangesAsync();
                SeriesCollectionList.Add(new SeriesCustomCollection(DateTime.Now.ToString(), ChartSeries, null, gbr.Position.Lat, gbr.Position.Lng));
                notificationManager.Show(new NotificationContent {
                    Title = "Информация",
                    Message = "Расчет сохранен",
                    Type = NotificationType.Success
                });
            }, obj => ChartSeries != null && PrivateID == null /*&& !SeriesCollectionList.Any(x=>x.Id)*/);
        }

        /// <summary>
        /// Извлекаем из базы данных запись расчета
        /// </summary>
        private RelayCommand _AddSeriesCollection;
        public RelayCommand AddSeriesCollection {
            get => _AddSeriesCollection ??= new RelayCommand(async obj => {
                int? objtype = gmaps_contol.Markers.FirstOrDefault(x => x.ZIndex != 1000).ZIndex;
                if (objtype == null)
                    return;
                var collection = reportContext.CalculatedRoutes.Where(x => x.ClcGroup == objtype).AsNoTracking().ToList();
                if (collection == null)
                    return;
                if (!collection.Any())
                    return;
                SeriesCollectionList.Clear();
                //TODO: требуется сделать группировки по ключу расчета, так как у нас может быть много расчетов
                var grouped_collection = collection.GroupBy(a => new { a.ClcKeyCalcId }).ToList();
                SeriesCollection series = null;
                string id = null;
                double? lat = null;
                double? lng = null;
                List<MatrixTotals> mt = new List<MatrixTotals>();
                foreach (var grp in grouped_collection) {
                    mt = new List<MatrixTotals>();
                    foreach (var item in grp) {
                        id = item.ClcCalcName;
                        lat = item.ClcGroupLatitude;
                        lng = item.ClcGroupLongitude;
                        var _obj = context.Object.FirstOrDefault(x => x.ObjectId == item.ClcObjectId);
                        mt.Add(new MatrixTotals(
                            duration: item.ClcDuration,
                            durationText: TimeSpan.FromSeconds(item.ClcDuration).Minutes.ToString() + ":" + TimeSpan.FromSeconds(item.ClcDuration).Seconds.ToString(),
                            objectInfo: Convert.ToString(_obj.ObjectNumber, 16) + Environment.NewLine + _obj.Name,
                            item.ClcObjectId,
                            null
                            ));
                    }
                    series = new SeriesCollection() {
                            new PieSeries
                            {
                                Title = ">15",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(mt.Count(x => x.Duration > 900)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "12-15",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(mt.Count(x => x.Duration >= 720 && x.Duration < 900)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "7-12",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(mt.Count(x => x.Duration >= 420 && x.Duration < 720)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "5-7",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(mt.Count(x => x.Duration >= 300 && x.Duration < 420)) },
                                DataLabels = true
                            },
                            new PieSeries
                            {
                                Title = "<5",
                                Values = new ChartValues<ObservableValue>{new ObservableValue(mt.Count(x => x.Duration < 300)) },
                                DataLabels = true
                            }
                        };

                    if (series != null) {
                        SeriesCollectionList.Add(new SeriesCustomCollection(id, series, mt, lat, lng));
                    }
                }
            });
        }
    }
}