using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models;
using VityazReports.Models.GuardObjectsOnMapGBR;

namespace VityazReports.ViewModel {
    public class GuardsOnMapViewModel : BaseViewModel {
        private readonly MsCRMContext context = new MsCRMContext();
        public GuardsOnMapViewModel() {
            InitializeMapControl.Execute(null);
            InitializeColorList.Execute(null);
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
                _GroupInfoFlyoutVisible = value;
                OnPropertyChanged(nameof(GroupInfoFlyoutVisible));
            }
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
                Guid NewPlacesGbrbaseID = Guid.NewGuid();
                SystemUserBase user = context.SystemUserBase.FirstOrDefault(x => x.DomainName.Contains(Environment.UserName));
                context.NewPlacesGbrbase.Add(new NewPlacesGbrbase() { 
                    NewPlacesGbrid = NewPlacesGbrbaseID, 
                    CreatedOn = DateTime.Now.AddHours(-5), 
                    ModifiedOn= DateTime.Now.AddHours(-5), 
                    CreatedBy =  user.SystemUserId,
                    ModifiedBy = user.SystemUserId
                    });
                await context.SaveChangesAsync();
                context.NewPlacesGbrextensionBase.Add(new NewPlacesGbrextensionBase() { NewPlacesGbrid = NewPlacesGbrbaseID, NewName = GroupName, NewLatitude = Latitude, NewLongitude = Longitude });
                await context.SaveChangesAsync();
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
                gmaps_contol.MapProvider = GMap.NET.MapProviders.YandexMapProvider.Instance;
                gmaps_contol.MinZoom = 5;
                gmaps_contol.MaxZoom = 17;
                gmaps_contol.Zoom = 5;
                gmaps_contol.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
                gmaps_contol.CanDragMap = true;
                gmaps_contol.DragButton = MouseButton.Left;
                gmaps_contol.CenterPosition = new PointLatLng(55.159904, 61.401919);
            });
        }

        private RelayCommand _ClearMapsCommand;
        public RelayCommand ClearMapsCommand {
            get => _ClearMapsCommand ??= new RelayCommand(async obj => {
                List<GMapMarker> markers = gmaps_contol.Markers.ToList();
                if (!markers.Any())
                    return;
                foreach (GMapMarker item in markers)
                    gmaps_contol.Markers.Remove(item);
            });
        }

        private RelayCommand _CreateLabelGBRCommand;
        public RelayCommand CreateLabelGBRCommand {
            get => _CreateLabelGBRCommand ??= new RelayCommand(async obj => {
                GroupInfoFlyoutVisible = true;
                //if (string.IsNullOrEmpty(GroupName)) {
                //    System.Windows.Forms.MessageBox.Show("Наименование экипажа не может быть пустым");
                //    return;
                //}
                var u = obj as System.Windows.Controls.Button;
                if (u == null)
                    return;
                //if (ObjectTypeList.Count(x => x.IsShowOnMap == true) == 1) {
                //if (gmaps_contol.Markers.Count(x => x.ZIndex.ToString() == "-1") == 0) {
                Point point = u.TranslatePoint(new Point(), gmaps_contol);
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
                PointLatLng p = gmaps_contol.FromLocalToLatLng((int)point.X - 35, (int)point.Y - 15);
                Latitude = p.Lat.ToString().Replace('.', ',');
                Longitude = p.Lng.ToString().Replace('.', ',');

                //ChartSeries = null;
                //}
                //else
                //    System.Windows.Forms.MessageBox.Show("На карте уже есть экипаж", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                //else {
                //    System.Windows.Forms.MessageBox.Show("Необходимо выбрать хоть один маршрут", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    //FlyoutShowGroupsVisibleState = true;
                //}
                ContextMenuIsOpen = false;
                GroupName = null;
            });
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
            Guid id = Guid.TryParse(marker.Tag.ToString(), out id) ? id : Guid.Empty;
            if (id == Guid.Empty)
                return;
            NewPlacesGbrextensionBase gbrextensionBase = context.NewPlacesGbrextensionBase.FirstOrDefault(x => x.NewPlacesGbrid == id);
            if (gbrextensionBase == null)
                return;
            Latitude = gbrextensionBase.NewLatitude;
            Longitude = gbrextensionBase.NewLongitude;
            GroupName = gbrextensionBase.NewName;
        }

        private RelayCommand _EditMarkerCommand;
        public RelayCommand EditMarkerCommand {
            get => _EditMarkerCommand ??= new RelayCommand(async obj => {
                var t = 0;
            }, obj=>!string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude) && !string.IsNullOrEmpty(GroupName));
        }
        /// <summary>
        /// Команда. Показать все группы
        /// </summary>
        private RelayCommand _ShowAllGroupsCommand;
        public RelayCommand ShowAllGroupsCommand {
            get => _ShowAllGroupsCommand ??= new RelayCommand(async obj => {
                ClearMapsCommand.Execute(null);
                List<NewPlacesGbrextensionBase> groups_places = await context.NewPlacesGbrextensionBase.Where(x => x.NewLatitude != null && x.NewLongitude != null).AsNoTracking().ToListAsync();
                if (!groups_places.Any())
                    return;
                foreach (NewPlacesGbrextensionBase item in groups_places) {
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
    }
}
