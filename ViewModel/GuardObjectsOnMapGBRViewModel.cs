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

namespace VityazReports.ViewModel {
    public class GuardObjectsOnMapGBRViewModel : BaseViewModel {
        private readonly A28Context context;

        public GuardObjectsOnMapGBRViewModel() {
            context = new A28Context();

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
            //ColorList.Add(new ColorModel() { Color = Brushes.Yellow, Isfree = true });//желтый
            ColorList.Add(new ColorModel() { Color = Brushes.Blue, Isfree = true });//голубой
            //ColorList.Add(new ColorModel() { Color = Brushes.White, Isfree = true });//белый
        }
        public GMapControl gmaps_contol { get; set; } = new GMapControl();


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

        private DefaultLegend _ChartLegent;
        public DefaultLegend ChartLegent {
            get => _ChartLegent;
            set {
                _ChartLegent = value;
                OnPropertyChanged(nameof(ChartLegent));
            }
        }

        private DefaultTooltip _ChartToolTip;
        public DefaultTooltip ChartToolTip {
            get => _ChartToolTip;
            set {
                _ChartToolTip = value;
                OnPropertyChanged(nameof(ChartToolTip));
            }
        }

        private RelayCommand _CreateLegend;
        public RelayCommand CreateLegend {
            get => _CreateLegend ??= new RelayCommand(async obj => {
                ChartLegent = new DefaultLegend();
                ChartLegent.BulletSize = 15;
                ChartLegent.Foreground = Brushes.White;
                ChartLegent.Orientation = System.Windows.Controls.Orientation.Vertical;
            });
        }

        private RelayCommand _CreateToolTip;
        public RelayCommand CreateToolTip {
            get => _CreateToolTip ??= new RelayCommand(async obj => {
                ChartToolTip = new DefaultTooltip();
                ChartToolTip.Background = Brushes.Black;
                ChartToolTip.Foreground = Brushes.White;
            });
        }

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

        private RelayCommand _ShowGroups;
        public RelayCommand ShowGroups {
            get => _ShowGroups ??= new RelayCommand(async obj => {
                FlyoutShowGroupsVisibleState = !FlyoutShowGroupsVisibleState;
            });
        }

        private ObservableCollection<ColorModel> _ColorList = new ObservableCollection<ColorModel>();
        public ObservableCollection<ColorModel> ColorList {
            get => _ColorList;
            set {
                _ColorList = value;
                OnPropertyChanged(nameof(ColorList));
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
                var markers = gmaps_contol.Markers.Where(x => x.Tag.ToString() == ObjId.ToString()).ToList();
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
                    List<Models.GuardObjectsOnMapGBR.Object> ObjectsList = context.Object.Where(x => x.ObjTypeId == ObjId.Value && x.RecordDeleted == false && x.Latitude != null && x.Longitude != null).AsNoTracking().ToList();
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
                                ToolTip = Convert.ToString(item.ObjectNumber, 16) + " (" + item.Name + ")" + Environment.NewLine + item.Name,
                                AllowDrop = true
                            }
                        };
                        //marker.Tag = ObjId.ToString();
                        marker.Tag = item.ObjectId.ToString();
                        gmaps_contol.Markers.Add(marker);                        
                    }
                    toggleButton.Background = cm.First().Color;
                    toggleButton.Content += " (" + ObjectsList.Count.ToString() + ")";
                    ColorList.First(x => x.Color == cm.First().Color).Isfree = false;
                    ColorList.First(x => x.Color == cm.First().Color).ObjTypeId = ObjId.ToString();
                    ObjectTypeList.First(x => x.ObjTypeId == Int16.Parse(ObjId.ToString())).IsShowOnMap = true;
                    toggleButton.IsChecked = true;
                }
            });
        }

        private ObservableCollection<MatrixTotals> _ChartObjectsList = new ObservableCollection<MatrixTotals>();
        public ObservableCollection<MatrixTotals> ChartObjectsList {
            get => _ChartObjectsList;
            set {
                _ChartObjectsList = value;
                OnPropertyChanged(nameof(ChartObjectsList));
            }
        }

        private RelayCommand _CreateLabelGbr;
        public RelayCommand CreateLabelGbr {
            get => _CreateLabelGbr ??= new RelayCommand(async obj => {
                MouseEventArgs mouseEventArgs = obj as MouseEventArgs;
                if (mouseEventArgs.RightButton == MouseButtonState.Pressed) {
                    if (ObjectTypeList.Count(x => x.IsShowOnMap == true) == 1) {
                        if (gmaps_contol.Markers.Count(x => x.Tag == "ГБР") == 0) {
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
                            marker.Tag = "ГБР";
                            gmaps_contol.Markers.Add(marker);
                        }
                        else
                            System.Windows.Forms.MessageBox.Show("На карте уже есть экипаж", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        System.Windows.Forms.MessageBox.Show("Необходимо выбрать хоть один маршрут", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private RelayCommand _ClearGroup;
        public RelayCommand ClearGroup {
            get => _ClearGroup ??= new RelayCommand(async obj => {
                //TODO: задать вопрос на подтвреждение удаления
                var markers = gmaps_contol.Markers.Where(x => x.Tag.ToString() == "ГБР").ToList();
                foreach (var item in markers) {
                    gmaps_contol.Markers.Remove(item);
                }
            });
        }

        private MatrixTotals _SelectedObject;
        public MatrixTotals SelectedObject {
            get => _SelectedObject;
            set {
                if (_SelectedObject != null) {
                    var y = gmaps_contol.Markers.First(x => x.Tag.ToString() == _SelectedObject.Id.ToString());
                    if (y == null)
                        return;
                    GMapMarker m = y as GMapMarker;
                    if (m == null)
                        return;
                    Ellipse ellipse = m.Shape as Ellipse;
                    ellipse.StrokeThickness = 7.5;
                    ellipse.Stroke = Brushes.Red;
                }
                _SelectedObject = value;
                OnPropertyChanged(nameof(SelectedObject));
            }
        }

        private RelayCommand _SelectedObjectCommand;
        public RelayCommand SelectedObjectCommand {
            get => _SelectedObjectCommand ??= new RelayCommand(async obj => {
                var y=gmaps_contol.Markers.First(x => x.Tag.ToString() == SelectedObject.Id.ToString());
                if (y == null)
                    return;
                GMapMarker m = y as GMapMarker;
                if (m == null)
                    return;
                Ellipse ellipse = m.Shape as Ellipse;
                ellipse.StrokeThickness = 20;
                ellipse.Stroke = Brushes.Blue;

                var ms = gmaps_contol.Markers.Where(x => x.Tag.ToString() != SelectedObject.Id.ToString()).ToList();

            });
        }

        private RelayCommand _CalculateCommand;
        public RelayCommand CalculateCommand {
            get => _CalculateCommand ??= new RelayCommand(async obj => {
                //todo: проверять что series не null, если не null, то открывать просто 
                if (ChartSeries != null) {
                    ChartVisible = true;
                    return;
                }
                CreateLegend.Execute(null);
                CreateToolTip.Execute(null);
                await Dispatcher.CurrentDispatcher.Invoke(async () => {
                    ChartVisible = false;
                    Loading = true;
                    GMapMarker gbr = null;
                    //List<GMapMarker> objects = null;
                    gbr = gmaps_contol.Markers.FirstOrDefault(x => x.Tag == "ГБР");
                    var objects = gmaps_contol.Markers.Where(x => x.Tag != "ГБР").ToList();
                    //foreach (var item in objects) {
                    //    ChartObjectsList.Add(new MatrixTotals() { Duration = -1, ObjectInfo = item.Tag.ToString() });
                    //}
                    //List<MatrixTotals> matrix = new List<MatrixTotals>();
                    //matrix.Clear();
                    ChartObjectsList.Clear();

                    if (gbr != null && objects != null) {
                        if (objects.Count > 0) {
                            using (HttpClient client = new HttpClient()) {
                                foreach (var item in objects) {
                                    string resp = @"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&language=ru&origins=" + gbr.Position.Lat.ToString().Replace(',', '.') + "," + gbr.Position.Lng.ToString().Replace(',', '.') +
                                        "&destinations=" + item.Position.Lat.ToString().Replace(',', '.') + "," + item.Position.Lng.ToString().Replace(',', '.') + "&key=AIzaSyCDXENAPVyVN2TddfuGUuPR6wAV2RL7Dh4";
                                    HttpResponseMessage response = await client.GetAsync(resp);
                                    var GoogleMatrixDistance = JsonConvert.DeserializeObject<GoogleMatrixDistanceModel>(response.Content.ReadAsStringAsync().Result);
                                    if (GoogleMatrixDistance != null) {
                                        if (GoogleMatrixDistance.rows[0].elements[0].duration != null) {
                                            TimeSpan ts = TimeSpan.FromSeconds(double.Parse(GoogleMatrixDistance.rows[0].elements[0].duration.value.ToString()));
                                            ChartObjectsList.Add(new MatrixTotals(GoogleMatrixDistance.rows[0].elements[0].duration.value, (item.Shape as Ellipse).ToolTip.ToString(), ts.Minutes.ToString()+":"+ts.Seconds.ToString(), int.Parse(item.Tag.ToString())));
                                        }
                                            //matrix.Add(new MatrixTotals() {
                                            //    Duration = GoogleMatrixDistance.rows[0].elements[0].duration.value,
                                            //    ObjectInfo = item.Tag.ToString()
                                            //});
                                    }
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