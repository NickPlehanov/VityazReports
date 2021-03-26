using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models;
using VityazReports.Models.GuardObjectsOnMapGBR;
using VityazReports.Models.GuardsOnMap;

namespace VityazReports.ViewModel {
    public class GuardsOnMapViewModel : BaseViewModel {
        private readonly MsCRMContext context = new MsCRMContext();
        NotificationManager notificationManager = new NotificationManager();

        public GuardsOnMapViewModel() {
            InitializeMapControl.Execute(null);
            InitializeColorList.Execute(null);
            ContextMenuIsOpen = false;
        }

        private GMapControl _gmaps_contol = new GMapControl();
        public GMapControl gmaps_contol {
            get => _gmaps_contol;
            set {
                _gmaps_contol = value;
                OnPropertyChanged(nameof(gmaps_contol));
            }
        }
        private ObservableCollection<ColorModel> _ColorList = new ObservableCollection<ColorModel>();
        public ObservableCollection<ColorModel> ColorList {
            get => _ColorList;
            set {
                _ColorList = value;
                OnPropertyChanged(nameof(ColorList));
            }
        }

        private bool _ContextMenuIsOpen;
        public bool ContextMenuIsOpen {
            get => _ContextMenuIsOpen;
            set {
                _ContextMenuIsOpen = value;
                OnPropertyChanged(nameof(ContextMenuIsOpen));
            }
        }


        private bool _GroupInfoFlyoutVisible;
        public bool GroupInfoFlyoutVisible {
            get => _GroupInfoFlyoutVisible;
            set {
                if (_GroupInfoFlyoutVisible && !value && NewMarker != null)
                    gmaps_contol.Markers.Remove(NewMarker);
                _GroupInfoFlyoutVisible = value;
                OnPropertyChanged(nameof(GroupInfoFlyoutVisible));
            }
        }

        private string _Password;
        public string Password {
            get => _Password;
            set {
                _Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private bool _IsAccess;
        public bool IsAccess {
            get => _IsAccess;
            set {
                _IsAccess = value;
                OnPropertyChanged(nameof(IsAccess));
            }
        }

        private RelayCommand _SetPasswordCommand;
        public RelayCommand SetPasswordCommand {
            get => _SetPasswordCommand ??= new RelayCommand(async obj => {
                //TODO:Потом сделать из инишников 
                if (Password.Equals("0350")) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Вход",
                        Message = "Пароль принят",
                        Type = NotificationType.Success
                    });
                    IsAccess = true;
                    PasswordFlyoutVisible = false;
                }
                else {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Неправильный пароль",
                        Type = NotificationType.Error
                    });
                    IsAccess = false;
                }
                Password = null;
            });
        }

        private string _GroupName;
        public string GroupName {
            get => _GroupName;
            set {
                _GroupName = value;
                OnPropertyChanged(nameof(GroupName));
            }
        }

        private string _Longitude;
        public string Longitude {
            get => _Longitude;
            set {
                _Longitude = value;
                OnPropertyChanged(nameof(Longitude));
            }
        }

        private string _Latitude;
        public string Latitude {
            get => _Latitude;
            set {
                _Latitude = value;
                OnPropertyChanged(nameof(Latitude));
            }
        }

        private RelayCommand _SaveIntoDatabaseCommand;
        public RelayCommand SaveIntoDatabaseCommand {
            get => _SaveIntoDatabaseCommand ??= new RelayCommand(async obj => {
                if (string.IsNullOrEmpty(Latitude) && string.IsNullOrEmpty(Longitude) && string.IsNullOrEmpty(GroupName))
                    return;
                var explace = context.NewPlacesGbrextensionBase.FirstOrDefault(x => x.NewLatitude == Latitude && x.NewLongitude == Longitude);
                if (explace != null) {
                    notificationManager.Show(new NotificationContent {
                        Title = "Ошибка",
                        Message = "Экипаж с такими координатми уже есть в базе",
                        Type = NotificationType.Error
                    });
                    GroupInfoFlyoutVisible = false;
                    return;
                }
                Guid NewPlacesGbrbaseID = Guid.NewGuid();
                Models.SystemUserBase user = context.SystemUserBase.FirstOrDefault(x => x.DomainName.Contains(Environment.UserName));
                context.NewPlacesGbrbase.Add(new Models.NewPlacesGbrbase() {
                    NewPlacesGbrid = NewPlacesGbrbaseID,
                    CreatedOn = DateTime.Now.AddHours(-5),
                    ModifiedOn = DateTime.Now.AddHours(-5),
                    CreatedBy = user.SystemUserId,
                    ModifiedBy = user.SystemUserId,
                    Statecode = 0,
                    Statuscode = 1,
                    DeletionStateCode = 0,
                    OwningUser = user.SystemUserId,
                    OwningBusinessUnit = user.BusinessUnitId
                });
                await context.SaveChangesAsync();
                context.NewPlacesGbrextensionBase.Add(new Models.NewPlacesGbrextensionBase() { NewPlacesGbrid = NewPlacesGbrbaseID, NewName = GroupName, NewLatitude = Latitude, NewLongitude = Longitude });
                await context.SaveChangesAsync();
                GroupInfoFlyoutVisible = false;
                notificationManager.Show(new NotificationContent {
                    Title = "Успех",
                    Message = "Сохранение успешно",
                    Type = NotificationType.Success
                });
            });
        }
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

        private GMapMarker _NewMarker;
        public GMapMarker NewMarker {
            get => _NewMarker;
            set {
                _NewMarker = value;
                OnPropertyChanged(nameof(NewMarker));
            }
        }

        private bool _PasswordFlyoutVisible;
        public bool PasswordFlyoutVisible {
            get => _PasswordFlyoutVisible;
            set {
                _PasswordFlyoutVisible = value;
                OnPropertyChanged(nameof(PasswordFlyoutVisible));
            }
        }

        private RelayCommand _CheckAccessCommand;
        public RelayCommand CheckAccessCommand {
            get => _CheckAccessCommand ??= new RelayCommand(async obj => {
                PasswordFlyoutVisible = !PasswordFlyoutVisible;
            }, obj => !IsAccess);
        }

        private RelayCommand _ClearMapsCommand;
        public RelayCommand ClearMapsCommand {
            get => _ClearMapsCommand ??= new RelayCommand(async obj => {
                List<GMapMarker> markers = gmaps_contol.Markers.ToList();
                if (!markers.Any())
                    return;
                foreach (GMapMarker item in markers)
                    gmaps_contol.Markers.Remove(item);
                ContextMenuIsOpen = false;
            }, obj => gmaps_contol.Markers.Any());
        }

        private RelayCommand _CreateLabelGBRCommand;
        public RelayCommand CreateLabelGBRCommand {
            get => _CreateLabelGBRCommand ??= new RelayCommand(async obj => {
                GroupInfoFlyoutVisible = true;
                var u = obj as System.Windows.Controls.Button;
                if (u == null)
                    return;
                System.Windows.Point point = u.TranslatePoint(new System.Windows.Point(), gmaps_contol);
                GMapMarker marker = new GMapMarker(new PointLatLng()) {
                    Shape = new Ellipse {
                        Width = 20,
                        Height = 20,
                        Stroke = Brushes.Chocolate,
                        StrokeThickness = 7.5,
                        AllowDrop = true
                    }
                };
                marker.Position = gmaps_contol.FromLocalToLatLng((int)point.X - 35, (int)point.Y - 15);
                marker.ZIndex = -1;
                marker.Shape.MouseRightButtonUp += Shape_MouseRightButtonUp;
                gmaps_contol.Markers.Add(marker);
                NewMarker = marker;
                PointLatLng p = gmaps_contol.FromLocalToLatLng((int)point.X - 35, (int)point.Y - 15);
                Latitude = p.Lat.ToString().Replace('.', ',');
                Longitude = p.Lng.ToString().Replace('.', ',');
                //Отрабатывает в случае если это редактирование
                if (SelectedMarker != null) {
                    GroupInfoFlyoutVisible = false;
                    Models.NewPlacesGbrbase gbrbase = context.NewPlacesGbrbase.FirstOrDefault(x => x.NewPlacesGbrid.ToString() == SelectedMarker.Tag.ToString());
                    if (gbrbase == null)
                        return;
                    Models.NewPlacesGbrextensionBase gbrextensionBase = context.NewPlacesGbrextensionBase.FirstOrDefault(x => x.NewPlacesGbrid.ToString() == SelectedMarker.Tag.ToString());
                    if (gbrextensionBase == null)
                        return;
                    gbrextensionBase.NewLatitude = Latitude;
                    gbrextensionBase.NewLongitude = Longitude;
                    gbrextensionBase.NewName = GroupName;
                    context.SaveChanges();
                    SelectedMarker = null;
                    ShowAllGroupsCommand.Execute(null);
                    notificationManager.Show(new NotificationContent {
                        Title = "Успех",
                        Message = "Расположение экипажа изменено",
                        Type = NotificationType.Success
                    });
                }
                ContextMenuIsOpen = false;
            }, obj => IsAccess);
        }

        private GMapMarker _SelectedMarker;
        public GMapMarker SelectedMarker {
            get => _SelectedMarker;
            set {
                _SelectedMarker = value;
                OnPropertyChanged(nameof(SelectedMarker));
            }
        }

        private void Shape_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            if (sender == null)
                return;
            Ellipse ellipse = sender as Ellipse;
            if (ellipse == null)
                return;
            GMapMarker marker = ellipse.DataContext as GMapMarker;
            if (marker == null)
                return;
            SelectedMarker = marker;
            Guid id = Guid.TryParse(marker.Tag.ToString(), out id) ? id : Guid.Empty;
            if (id == Guid.Empty)
                return;
            Models.NewPlacesGbrextensionBase gbrextensionBase = context.NewPlacesGbrextensionBase.FirstOrDefault(x => x.NewPlacesGbrid == id);
            if (gbrextensionBase == null)
                return;
            Latitude = gbrextensionBase.NewLatitude;
            Longitude = gbrextensionBase.NewLongitude;
            GroupName = gbrextensionBase.NewName;
        }

        private RelayCommand _DeleteNewLabelCommand;
        public RelayCommand DeleteNewLabelCommand {
            get => _DeleteNewLabelCommand ??= new RelayCommand(async obj => {
                if (NewMarker == null)
                    return;
                gmaps_contol.Markers.Remove(NewMarker);
                GroupInfoFlyoutVisible = false;
            });
        }
        private RelayCommand _EditMarkerCommand;
        public RelayCommand EditMarkerCommand {
            get => _EditMarkerCommand ??= new RelayCommand(async obj => {
                string lat = Latitude;
                string lon = Longitude;
                GroupInfoFlyoutVisible = true;
                notificationManager.Show(new NotificationContent {
                    Title = "Внимание",
                    Message = string.Format("Укажите новые координаты для экипажа: {0}", GroupName),
                    Type = NotificationType.Warning
                });
                gmaps_contol.Markers.Remove(SelectedMarker);
            }, obj => !string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude) && !string.IsNullOrEmpty(GroupName) && IsAccess);
        }

        private RelayCommand _DeleteMarkerCommand;
        public RelayCommand DeleteMarkerCommand {
            get => _DeleteMarkerCommand ??= new RelayCommand(async obj => {
                if (SelectedMarker == null)
                    return;
                Guid id = Guid.TryParse(SelectedMarker.Tag.ToString(), out id) ? id : Guid.Empty;
                if (id == Guid.Empty)
                    return;
                Models.NewPlacesGbrextensionBase exbase = context.NewPlacesGbrextensionBase.FirstOrDefault(x => x.NewPlacesGbrid == id);
                Models.NewPlacesGbrbase @base = context.NewPlacesGbrbase.FirstOrDefault(x => x.NewPlacesGbrid == id);
                if (exbase == null && @base == null)
                    return;
                context.NewPlacesGbrextensionBase.Remove(exbase);
                context.NewPlacesGbrbase.Remove(@base);
                await context.SaveChangesAsync();
                notificationManager.Show(new NotificationContent {
                    Title = "Успех",
                    Message = "Объект удален",
                    Type = NotificationType.Success
                });

                gmaps_contol.Markers.Remove(SelectedMarker);
            }, obj => !string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude) && !string.IsNullOrEmpty(GroupName) && IsAccess);
        }
        /// <summary>
        /// Команда. Показать все группы
        /// </summary>
        private RelayCommand _ShowAllGroupsCommand;
        public RelayCommand ShowAllGroupsCommand {
            get => _ShowAllGroupsCommand ??= new RelayCommand(async obj => {
                ClearMapsCommand.Execute(null);
                List<Models.NewPlacesGbrextensionBase> groups_places = await context.NewPlacesGbrextensionBase.Where(x => x.NewLatitude != null && x.NewLongitude != null).AsNoTracking().ToListAsync();
                if (!groups_places.Any())
                    return;
                foreach (Models.NewPlacesGbrextensionBase item in groups_places) {
                    PointLatLng point = new PointLatLng(Convert.ToDouble(item.NewLatitude.Replace('.', ',')), Convert.ToDouble(item.NewLongitude.Replace('.', ',')));
                    GMapMarker marker = new GMapMarker(point) {
                        Shape = new Ellipse {
                            Width = 12,
                            Height = 12,
                            Stroke = Brushes.DarkRed,
                            StrokeThickness = 7.5,
                            ToolTip = item.NewName,
                            AllowDrop = true
                        }
                    };
                    marker.Tag = item.NewPlacesGbrid.ToString();
                    marker.Shape.MouseRightButtonUp += Shape_MouseRightButtonUp;
                    gmaps_contol.Markers.Add(marker);
                }
            });
        }

        private bool _GuardObjectVisible;
        public bool GuardObjectVisible {
            get => _GuardObjectVisible;
            set {
                _GuardObjectVisible = value;
                OnPropertyChanged(nameof(GuardObjectVisible));
            }
        }

        private string _Address;
        public string Address {
            get => _Address;
            set {
                _Address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private RelayCommand _CalculateRoutesVisibleCommand;
        public RelayCommand CalculateRoutesVisibleCommand {
            get => _CalculateRoutesVisibleCommand ??= new RelayCommand(async obj => {
                GuardObjectVisible = !GuardObjectVisible;
            });
        }

        private RelayCommand _CheckAddressAndGetCoordinatesCommand;
        public RelayCommand CheckAddressAndGetCoordinatesCommand {
            get => _CheckAddressAndGetCoordinatesCommand ??= new RelayCommand(async obj => {
                if (string.IsNullOrEmpty(Address))
                    return;
                using (HttpClient client = new HttpClient()) {
                    Root geocoder;
                    HttpResponseMessage response = await client.GetAsync(@"https://geocode-maps.yandex.ru/1.x/?apikey=68d35a07-648c-49a3-9403-9f1ceb903ca7&format=json&geocode=" + Address);
                    if (response.IsSuccessStatusCode) {
                        geocoder = JsonConvert.DeserializeObject<Root>(response.Content.ReadAsStringAsync().Result);
                        string[] coords = geocoder.response.GeoObjectCollection.featureMember[0].GeoObject.Point.pos.Split(' ');
                        if (coords.Count() > 0) {
                            Latitude = double.TryParse(coords[1].Replace('.', ','), out _) ? coords[1].Replace('.', ',') : Latitude;
                            Longitude = double.TryParse(coords[0].Replace('.', ','), out _) ? coords[0].Replace('.', ',') : Longitude;
                            if (string.IsNullOrEmpty(Latitude) && string.IsNullOrEmpty(Longitude)) {
                                notificationManager.Show(new NotificationContent {
                                    Title = "Ошибка",
                                    Message = "Пустые координаты",
                                    Type = NotificationType.Error
                                });
                                return;
                            }
                            if (Latitude.Equals("-1000") || Longitude.Equals("-1000")) {
                                notificationManager.Show(new NotificationContent {
                                    Title = "Ошибка",
                                    Message = "Пустые координаты",
                                    Type = NotificationType.Error
                                });
                                return;
                            }
                        }
                        else {
                            notificationManager.Show(new NotificationContent {
                                Title = "Ошибка",
                                Message = "По указанному адресу не найдены координаты",
                                Type = NotificationType.Error
                            });
                            return;
                        }
                    }
                    else {
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "По указанному адресу не удалось получить координаты",
                            Type = NotificationType.Error
                        });
                        return;
                    }
                }
                GetRoutes.Execute(null);
            });
        }

        private ObservableCollection<RoutesTiming> _RoutesTimingList = new ObservableCollection<RoutesTiming>();
        public ObservableCollection<RoutesTiming> RoutesTimingList {
            get => _RoutesTimingList;
            set {
                _RoutesTimingList = value;
                OnPropertyChanged(nameof(RoutesTimingList));
            }
        }
        private bool _ResultRoutesTimingListVisible;
        public bool ResultRoutesTimingListVisible {
            get => _ResultRoutesTimingListVisible;
            set {
                _ResultRoutesTimingListVisible = value;
                OnPropertyChanged(nameof(ResultRoutesTimingListVisible));
            }
        }
        private RelayCommand _GetRoutes;
        public RelayCommand GetRoutes {
            get => _GetRoutes ??= new RelayCommand(async obj => {
                List<NewPlacesGbrextensionBase> groups_places = await context.NewPlacesGbrextensionBase.Where(x => x.NewLatitude != null && x.NewLongitude != null).AsNoTracking().ToListAsync();
                if (!groups_places.Any())
                    return;
                HttpClient client = new HttpClient();
                RoutesTimingList.Clear();
                foreach (NewPlacesGbrextensionBase item in groups_places) {
                    string resp = @"http://router.project-osrm.org/route/v1/driving/" + item.NewLongitude.ToString().Replace(',', '.') + "," + item.NewLatitude.ToString().Replace(',', '.') + ";" + Longitude.ToString().Replace(',', '.') + "," + Latitude.ToString().Replace(',', '.') + "?geometries=geojson";
                    HttpResponseMessage response = await client.GetAsync(resp);
                    if (response.IsSuccessStatusCode) {
                        var osrm = JsonConvert.DeserializeObject<OSRM>(response.Content.ReadAsStringAsync().Result);
                        if (osrm.code.Equals("Ok"))
                            if (osrm.routes.Count >= 1)
                                RoutesTimingList.Add(new RoutesTiming() {
                                    NameGroup = item.NewName,
                                    Duration = osrm.routes[0].duration,
                                    Distance = osrm.routes[0].distance / 1000,
                                    Coordinates = osrm.routes[0].geometry.coordinates
                                });

                            else
                                RoutesTimingList.Add(new RoutesTiming() {
                                    NameGroup = item.NewName,
                                    Duration = -1,
                                    Distance = -1,
                                    Coordinates = null
                                });

                        else
                            RoutesTimingList.Add(new RoutesTiming() {
                                NameGroup = item.NewName,
                                Duration = -1,
                                Distance = -1,
                                Coordinates = null
                            });
                    }
                    else {
                        notificationManager.Show(new NotificationContent {
                            Title = "Ошибка",
                            Message = "Маршрут не был построен",
                            Type = NotificationType.Error
                        });
                        return;
                    }
                }
                ResultRoutesTimingListVisible = true;
            });
        }
    }
}
