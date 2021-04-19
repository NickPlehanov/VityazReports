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
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models;

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
                              select new { soc, soeb }
                ).AsNoTracking().ToList();
                if (coords.Count() <= 0) {
                    notificationManager.Show(new NotificationContent() { Type = NotificationType.Error, Title = "Ошибка", Message = "Для выбранного техника не удалось найти координаты" });
                    return;
                }
                var l_dates = coords.Select(x => x.soeb.NewDate).OrderBy(x => x.Value.Date).Distinct();
                if (l_dates.Count() <= 0)
                    return;
                foreach (var item in l_dates) {
                    ListDates.Add(item.Value.Date.AddHours(5).Date);
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
                              && soeb.NewDate.Value.Date == SelectedDate.Value.Date
                              select new { soc, soeb }
                              ).AsNoTracking().ToList();
            }, obj => SelectedDate != null);
        }
    }
}
