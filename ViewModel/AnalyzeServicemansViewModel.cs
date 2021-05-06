using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models;
using VityazReports.Models.AnalyzeServicemans;
using VityazReports.Models.ServicemanOnMapViewModel;

namespace VityazReports.ViewModel {
    public class AnalyzeServicemansViewModel : BaseViewModel {
        private A28Context a28Context;
        private MsCRMContext msCRMContext;
        public AnalyzeServicemansViewModel() {
            ServicemansList = new ObservableCollection<Servicemans>();
            GetServicemans.Execute(null);
            _gmaps_contol = new GMapControl();
            InitializeMapControl.Execute(null);
            //SqlDependency.Start("Data Source=sql-service;Initial Catalog=vityaz_MSCRM;Persist Security Info=True;User ID=admin;Password=111111");
            //SqlCommand command = new SqlCommand("Select soc_ID, soc_ServiceOrderID, soc_IncomeLatitude, soc_IncomeLongitude, soc_OutcomeLatitide, soc_OutcomeLongitude From ServiceOrderCoordinates");
            //SqlDependency dependency = new SqlDependency(command);
            //dependency.OnChange += Dependency_OnChange;
            //GetServiceordersOnDate.Execute(null);
            DateOrder = DateTime.Now;
            InitTimer.Execute(null);
            GetServiceordersOnDate.Execute(DateOrder);
        }
        private A28Context GetA28Context() {
            if (a28Context == null)
                return new A28Context();
            else
                return a28Context;
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
        private DispatcherTimer _Timer;
        public DispatcherTimer Timer {
            get => _Timer;
            set {
                _Timer = value;
                OnPropertyChanged(nameof(Timer));
            }
        }
        /// <summary>
        /// Свойство видимости для списка техников
        /// </summary>
        private bool _ServicemansListVisible;
        public bool ServicemansListVisible {
            get => _ServicemansListVisible;
            set {
                _ServicemansListVisible = value;
                OnPropertyChanged(nameof(ServicemansListVisible));
            }
        }
        /// <summary>
        /// Список техников
        /// </summary>
        private ObservableCollection<Servicemans> _ServicemansList;
        public ObservableCollection<Servicemans> ServicemansList {
            get => _ServicemansList;
            set {
                _ServicemansList = value;
                OnPropertyChanged(nameof(ServicemansList));
            }
        }
        /// <summary>
        /// Выбранный техник
        /// </summary>
        private Servicemans _SelectedServicemans;
        public Servicemans SelectedServicemans {
            get => _SelectedServicemans;
            set {
                _SelectedServicemans = value;
                OnPropertyChanged(nameof(SelectedServicemans));
            }
        }
        /// <summary>
        /// Храним дату заявки
        /// </summary>
        private DateTime _DateOrder;
        public DateTime DateOrder {
            get => _DateOrder;
            set {
                _DateOrder = value;
                OnPropertyChanged(nameof(DateOrder));
            }
        }
        /// <summary>
        /// Определяем свойства и настройки для контрола карты
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
                gmaps_contol.IgnoreMarkerOnMouseWheel = true;
            });
        }
        /// <summary>
        /// Команда выкл/вкл меню со списком техников
        /// </summary>
        private RelayCommand _ShowServicemansListCommand;
        public RelayCommand ShowServicemansListCommand {
            get => _ShowServicemansListCommand ??= new RelayCommand(async obj => {
                ServicemansListVisible = !ServicemansListVisible;
            });
        }
        /// <summary>
        /// Команда получения списка техников 
        /// </summary>
        private RelayCommand _GetServicemans;
        public RelayCommand GetServicemans {
            get => _GetServicemans ??= new RelayCommand(async obj => {
                var sm = GetMsCRMContext().NewServicemanExtensionBase.Where(x => x.NewIswork == true).AsNoTracking().ToList();
                if (sm == null)
                    return;
                if (sm.Count <= 0)
                    return;
                foreach (NewServicemanExtensionBase item in sm)
                    ServicemansList.Add(new Servicemans(item.NewName, item.NewCategory, item.NewServicemanId));
                ServicemansListVisible = ServicemansList.Count > 0;
            });
        }
        /// <summary>
        /// Команда выбора техника
        /// </summary>
        private RelayCommand _SelectServicemanCommand;
        public RelayCommand SelectServicemanCommand {
            get => _SelectServicemanCommand ??= new RelayCommand(async obj => {
                if (obj == null)
                    return;
                Guid? ID = obj as Guid?;
                if (ID == null)
                    return;
                SelectedServicemans = ServicemansList.FirstOrDefault(x => x.ServicemanID == ID.Value);
            });
        }
        /// <summary>
        /// Команда получения списка заявок технику по технику
        /// </summary>
        private RelayCommand _GetServiceordersByServiceman;
        public RelayCommand GetServiceordersByServiceman {
            get => _GetServiceordersByServiceman ??= new RelayCommand(async obj => {
                if (SelectedServicemans == null)
                    return;
                if (DateOrder == null)
                    return;
                msCRMContext = GetMsCRMContext();
                if (msCRMContext == null)
                    return;
                List<ServiceorderInfo> coords = (from soc in msCRMContext.ServiceOrderCoordinates
                                                 join soeb in msCRMContext.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                                                 join smeb in msCRMContext.NewServicemanExtensionBase on soeb.NewServicemanServiceorder equals smeb.NewServicemanId
                                                 where soeb.NewServicemanServiceorder == SelectedServicemans.ServicemanID
                                                 && soeb.NewDate.Value.Date == DateOrder.Date.AddHours(-5).Date
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                                   && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                                   && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                                 select new ServiceorderInfo(soc.SocIncomeLatitude,
                                                 soc.SocIncomeLongitude,
                                                 soc.SocOutcomeLatitide,
                                                 soc.SocOutcomeLongitude,
                                                 soeb.NewNumber,
                                                 soeb.NewCategory,
                                                 "наименование категории",
                                                 soeb.NewObjName,
                                                 soeb.NewName,
                                                 soeb.NewTechConclusion,
                                                 soeb.NewResult,
                                                 "результат работы по заявке",
                                                 soeb.NewIncome,
                                                 soeb.NewOutgone,
                                                 soeb.NewAddress,
                                                 soeb.NewOrderFrom,
                                                 soeb.NewWhoInit,
                                                 soeb.NewTime,
                                                 smeb.NewName
                                                     )
                                  ).AsNoTracking().ToList();
                if (coords.Count() <= 0)
                    return;
                //теперь необходимо заявки разобрать по типу того кто дал заявку и времени

            });
        }

        private RelayCommand _GetServiceordersOnDate;
        public RelayCommand GetServiceordersOnDate {
            get => _GetServiceordersOnDate ??= new RelayCommand(async obj => {
                //TODO: проверять на пустоту DateOrder
                Timer.Stop();
                gmaps_contol.Markers.Clear();
                msCRMContext = GetMsCRMContext();
                if (msCRMContext == null)
                    return;
                List<ServiceorderInfo> coords = (from soc in msCRMContext.ServiceOrderCoordinates
                                                 join soeb in msCRMContext.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                                                 join smeb in msCRMContext.NewServicemanExtensionBase on soeb.NewServicemanServiceorder equals smeb.NewServicemanId
                                                 join andr in msCRMContext.NewAndromedaExtensionBase on soeb.NewAndromedaServiceorder equals andr.NewAndromedaId
                                                 where soeb.NewDate.Value.Date == DateOrder.Date.AddHours(-5).Date
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                                   && string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                                   && string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                                   && soeb.NewOutgone == null
                                                 select new ServiceorderInfo(soc.SocIncomeLatitude,
                                                 soc.SocIncomeLongitude,
                                                 soc.SocOutcomeLatitide,
                                                 soc.SocOutcomeLongitude,
                                                 soeb.NewNumber,
                                                 soeb.NewCategory,
                                                 "наименование категории",
                                                 soeb.NewObjName,
                                                 soeb.NewName,
                                                 soeb.NewTechConclusion,
                                                 soeb.NewResult,
                                                 "результат работы по заявке",
                                                 soeb.NewIncome,
                                                 soeb.NewOutgone,
                                                 soeb.NewAddress,
                                                 soeb.NewOrderFrom,
                                                 soeb.NewWhoInit,
                                                 soeb.NewTime,
                                                 smeb.NewName
                                                     )
                                  ).AsNoTracking().ToList();
                if (coords.Count() <= 0) {
                    //TODO: добавить сообщение, что данных нет
                    return;
                }
                foreach (ServiceorderInfo item in coords) {
                    var d = (DateTime.Now.AddHours(-5) - item.Income).Value.Duration();
                    GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.IncomeLatitude), Convert.ToDouble(item.IncomeLongitude))) {
                        Shape = new Ellipse {
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.Purple,
                            StrokeThickness = 7.5,
                            //ToolTip = string.Format("{0} - {1} ({2})", item.Number, item.Name, item.Address),
                            ToolTip = string.Format("{0} ({1})"+Environment.NewLine+"{2} {3}"+Environment.NewLine+"{4}",item.TechName,string.Format("на объекте: {0}ч. {1}мин. {2}сек.",d.Hours,d.Minutes,d.Seconds), item.Number, item.Address, item.Name),
                            AllowDrop = false
                        }
                    };
                    gmaps_contol.Markers.Add(marker);
                }
                Timer.Start();
            });
        }
        private RelayCommand _InitTimer;
        public RelayCommand InitTimer {
            get => _InitTimer ??= new RelayCommand(async obj => {
                Timer = new DispatcherTimer();
                Timer.Tick += new EventHandler(timer_Tick);
                Timer.Interval = new TimeSpan(0, 1, 0);
            });
        }
        private void timer_Tick(object sender, EventArgs e) {
            GetServiceordersOnDate.Execute(DateOrder);
        }
    }
}
