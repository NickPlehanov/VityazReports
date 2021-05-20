using GeoCoordinatePortable;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models;
using VityazReports.Models.AnalyzeServicemans;

namespace VityazReports.ViewModel {
    public class AnalyzeServicemansViewModel : BaseViewModel {
        private A28Context a28Context;
        private MsCRMContext msCRMContext;
        public AnalyzeServicemansViewModel() {
            ServicemansList = new ObservableCollection<Servicemans>();
            NotCompletedOrdersList = new ObservableCollection<ClientsOrdersNotCompleted>();
            GetServicemans.Execute(null);
            _gmaps_contol = new GMapControl();
            InitializeMapControl.Execute(null);
            LatesOrders = new ObservableCollection<ServiceorderInfo>();
            Intervals = new ObservableCollection<IntervalsModel>();
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
                if (_DateOrder != null) {
                    FillIntervals.Execute(null);
                    GetInfoByServiceman.Execute(null);
                }
                OnPropertyChanged(nameof(DateOrder));
            }
        }
        /// <summary>
        /// Определяет видимость плавающей области с анализом техников за выбранный день
        /// </summary>
        private bool _AnalyzeSelectedServicemanVisibleFlyout;
        public bool AnalyzeSelectedServicemanVisibleFlyout {
            get => _AnalyzeSelectedServicemanVisibleFlyout;
            set {
                _AnalyzeSelectedServicemanVisibleFlyout = value;
                OnPropertyChanged(nameof(AnalyzeSelectedServicemanVisibleFlyout));
            }
        }

        private ObservableCollection<ServiceorderInfo> _LatesOrders;
        public ObservableCollection<ServiceorderInfo> LatesOrders {
            get => _LatesOrders;
            set {
                _LatesOrders = value;
                OnPropertyChanged(nameof(LatesOrders));
            }
        }

        private ObservableCollection<IntervalsModel> _Intervals;
        public ObservableCollection<IntervalsModel> Intervals {
            get => _Intervals;
            set {
                _Intervals = value;
                OnPropertyChanged(nameof(Intervals));
            }
        }

        private int _AllCountOrders;
        public int AllCountOrders {
            get => _AllCountOrders;
            set {
                _AllCountOrders = value;
                OnPropertyChanged(nameof(AllCountOrders));
            }
        }

        private int _AllCountComplete;
        public int AllCountComplete {
            get => _AllCountComplete;
            set {
                _AllCountComplete = value;
                OnPropertyChanged(nameof(AllCountComplete));
            }
        }

        private int _AllCountTransfer;
        public int AllCountTransfer {
            get => _AllCountTransfer;
            set {
                _AllCountTransfer = value;
                OnPropertyChanged(nameof(AllCountTransfer));
            }
        }

        private int _AllCountCancel;
        public int AllCountCancel {
            get => _AllCountCancel;
            set {
                _AllCountCancel = value;
                OnPropertyChanged(nameof(AllCountCancel));
            }
        }

        private ObservableCollection<ClientsOrdersNotCompleted> _NotCompletedOrdersList;
        public ObservableCollection<ClientsOrdersNotCompleted> NotCompletedOrdersList {
            get => _NotCompletedOrdersList;
            set {
                _NotCompletedOrdersList = value;
                OnPropertyChanged(nameof(NotCompletedOrdersList));
            }
        }

        private RelayCommand _FillIntervals;
        public RelayCommand FillIntervals {
            get => _FillIntervals ??= new RelayCommand(async obj => {
                LatesOrders.Clear();
                Intervals.Clear();
                AnalyzeSelectedServicemanVisibleFlyout = Intervals.Count > 0 || LatesOrders.Count > 0;
                if (SelectedServicemans == null || DateOrder == null)
                    return;
                App.Current.Dispatcher.Invoke((Action)delegate {
                    var coords = (from soc in msCRMContext.ServiceOrderCoordinates
                                  join soeb in msCRMContext.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                                  join sob in msCRMContext.NewServiceorderBase on soeb.NewServiceorderId equals sob.NewServiceorderId
                                  join smeb in msCRMContext.NewServicemanExtensionBase on soeb.NewServicemanServiceorder equals smeb.NewServicemanId
                                  join andr in msCRMContext.NewAndromedaExtensionBase on soeb.NewAndromedaServiceorder equals andr.NewAndromedaId
                                  where soeb.NewServicemanServiceorder == SelectedServicemans.ServicemanID
                                  && soeb.NewDate.Value.Date == DateOrder.Date.AddHours(-5).Date
                                    && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                    && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                    && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                    && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                    && sob.DeletionStateCode == 0
                                    && sob.Statecode == 0
                                    && sob.Statuscode == 1
                                  select new { soeb.NewIncome, soeb.NewOutgone }
                                  ).AsNoTracking().ToList();
                    if (coords.Count() <= 0)
                        return;
                    coords = coords.OrderBy(x => x.NewIncome).ToList();
                    for (int i = 0; i < coords.Count - 1; i++) {
                        var d = (coords[i + 1].NewIncome - coords[i].NewOutgone).Value.Duration();
                        //Intervals.Add(string.Format("({0}) {1}-{2}", d.Minutes, coords[i].NewOutgone.Value.ToShortTimeString(), coords[i + 1].NewIncome.Value.ToShortTimeString()));
                        if (coords[i + 1].NewIncome.Value >= coords[i].NewOutgone.Value)
                            if (d.TotalMinutes >= 40)
                                Intervals.Add(new IntervalsModel(coords[i].NewOutgone.Value.ToShortTimeString() + " - " + coords[i + 1].NewIncome.Value.ToShortTimeString(), d.Hours.ToString() + ":" + d.Minutes.ToString()));
                    }
                });
                AnalyzeSelectedServicemanVisibleFlyout = Intervals.Count > 0 || LatesOrders.Count > 0;
            });
        }

        private RelayCommand _GetInfoByServiceman;
        public RelayCommand GetInfoByServiceman {
            get => _GetInfoByServiceman ??= new RelayCommand(async obj => {
                if (SelectedServicemans == null || DateOrder == null)
                    return;
                NotCompletedOrdersList.Clear();
                //Требуется получить количество заявок , сколько выполнено, перенесено, отменено
                //Сколько клиентских заявок всего - сделано/перенесено и отменено
                App.Current.Dispatcher.Invoke((Action)delegate {
                    msCRMContext = GetMsCRMContext();


                    List<NewServiceorderExtensionBase> soebs = new List<NewServiceorderExtensionBase>();
                    soebs = msCRMContext.NewServiceorderExtensionBase.Where(x => x.NewDate.Value.AddHours(-5).Date == DateOrder.Date && x.NewServicemanServiceorder == SelectedServicemans.ServicemanID).AsNoTracking().ToList();
                    if (soebs.Count <= 0)
                        return;
                    AllCountOrders = soebs.Count;
                    AllCountComplete = soebs.Count(x => x.NewResult == 1);

                    List<NewServiceorderExtensionBase> soebs_transfer = new List<NewServiceorderExtensionBase>();
                    soebs_transfer = soebs.Where(x => x.NewResult == 2).ToList();
                    AllCountTransfer = soebs_transfer.Count;

                    List<NewServiceorderExtensionBase> soebs_cancel = new List<NewServiceorderExtensionBase>();
                    soebs_cancel = soebs.Where(x => x.NewResult == 3).ToList();
                    AllCountCancel = soebs_cancel.Count;

                    List<NewServiceorderExtensionBase> soebs_clients = new List<NewServiceorderExtensionBase>();
                    soebs_clients = soebs.Where(x => x.NewOrderFrom != 3).ToList();

                    List<NewServiceorderExtensionBase> soebs_clients_transfer = new List<NewServiceorderExtensionBase>();
                    soebs_clients_transfer = soebs_clients.Where(x => x.NewResult == 2).ToList();
                    int ClientCountTransfer = soebs_clients_transfer.Count();

                    List<NewServiceorderExtensionBase> soebs_clients_cancel = new List<NewServiceorderExtensionBase>();
                    soebs_clients_cancel = soebs_clients.Where(x => x.NewResult == 3).ToList();
                    int ClientCountCancel = soebs_clients_cancel.Count();

                    //получаем всю информацию по заявкам, которые были с результатом отмена/перенос
                    foreach (var st in soebs_transfer) {
                        var transfers = (from soeb in msCRMContext.NewServiceorderExtensionBase
                                         join sob in msCRMContext.NewServiceorderBase on soeb.NewServiceorderId equals sob.NewServiceorderId
                                         join soc in msCRMContext.ServiceOrderCoordinates on sob.NewServiceorderId equals soc.SocServiceOrderId into lj_soc
                                         from x in lj_soc.DefaultIfEmpty()
                                         join andr_e in msCRMContext.NewAndromedaExtensionBase on soeb.NewNumber equals andr_e.NewNumber
                                         join andr in msCRMContext.NewAndromedaBase on andr_e.NewAndromedaId equals andr.NewAndromedaId
                                         where soeb.NewServiceorderId == st.NewServiceorderId
                                         && andr.DeletionStateCode == 0
                                         && andr.Statecode == 0
                                         && andr.Statuscode == 1
                                         select new ClientsOrdersNotCompleted(soeb.NewNumber, soeb.NewObjName, soeb.NewAddress, soeb.NewOrderFrom, null,
                                         soeb.NewName, soeb.NewIncome, soeb.NewOutgone, andr_e.NewLatitude, andr_e.NewLonitude, null, null, soeb.NewResult, null, soeb.NewResultId, null, soeb.NewTransferReason, soeb.NewTechConclusion
                                         , x.SocIncomeLatitude, x.SocIncomeLongitude, x.SocOutcomeLatitide, x.SocOutcomeLongitude)
                                ).AsNoTracking().ToList();
                        //вероятно это лишнее, но на всякий случай сделать стоит, так как кто его знает, вдруг решат менять порядок причин или в целом наименование причин.
                        foreach (var tr in transfers) {
                            var result = (from apv in msCRMContext.AttributePicklistValue
                                          join ll in msCRMContext.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                                          join a in msCRMContext.Attribute on apv.AttributeId equals a.AttributeId
                                          join e in msCRMContext.Entity on a.EntityId equals e.EntityId
                                          where a.PhysicalName == "New_Result"
                                          && e.Name == "New_serviceorder"
                                          && apv.Value == tr.Result
                                          select new { label = ll.Label, order = apv.DisplayOrder, value = apv.Value }).AsNoTracking().ToList();
                            if (result != null)
                                tr.ResultString = result.FirstOrDefault().label;
                            var resultID = (from apv in msCRMContext.AttributePicklistValue
                                            join ll in msCRMContext.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                                            join a in msCRMContext.Attribute on apv.AttributeId equals a.AttributeId
                                            join e in msCRMContext.Entity on a.EntityId equals e.EntityId
                                            where a.PhysicalName == "New_ResultId"
                                            && e.Name == "New_serviceorder"
                                            && apv.Value == tr.ReasonResult
                                            select new { label = ll.Label, order = apv.DisplayOrder, value = apv.Value }).AsNoTracking().ToList();
                            if (resultID != null)
                                tr.ReasonResultString = resultID.FirstOrDefault().label;
                            var ord_type = (from apv in msCRMContext.AttributePicklistValue
                                            join ll in msCRMContext.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                                            join a in msCRMContext.Attribute on apv.AttributeId equals a.AttributeId
                                            join e in msCRMContext.Entity on a.EntityId equals e.EntityId
                                            where a.PhysicalName == "New_order_from"
                                            && e.Name == "New_serviceorder"
                                            && apv.Value == tr.OrderType
                                            select new { label = ll.Label, order = apv.DisplayOrder, value = apv.Value }).AsNoTracking().ToList();
                            if (ord_type != null)
                                tr.OrderTypeString = ord_type.FirstOrDefault().label;

                            if (!string.IsNullOrEmpty(tr.LatitudeObject) && !string.IsNullOrEmpty(tr.LongitudeObject)
                            && !string.IsNullOrEmpty(tr.IncomeLatitude) && !string.IsNullOrEmpty(tr.IncomeLongitude)
                            && !string.IsNullOrEmpty(tr.OutgoneLatitude) && !string.IsNullOrEmpty(tr.OutgoneLongitude)) {
                                GeoCoordinate c_object = new GeoCoordinate(double.Parse(tr.LatitudeObject), double.Parse(tr.LongitudeObject));
                                GeoCoordinate c_income = new GeoCoordinate(double.Parse(tr.IncomeLatitude), double.Parse(tr.IncomeLongitude));
                                GeoCoordinate c_outgone = new GeoCoordinate(double.Parse(tr.OutgoneLatitude), double.Parse(tr.OutgoneLongitude));

                                tr.DistanceIncome = c_income.GetDistanceTo(c_object);
                                tr.DistanceOutgone = c_outgone.GetDistanceTo(c_object);
                            }
                            NotCompletedOrdersList.Add(tr);
                        }
                    }

                    foreach (var sc in soebs_cancel) {
                        var cancels = (from soeb in msCRMContext.NewServiceorderExtensionBase
                                       join sob in msCRMContext.NewServiceorderBase on soeb.NewServiceorderId equals sob.NewServiceorderId
                                       join soc in msCRMContext.ServiceOrderCoordinates on sob.NewServiceorderId equals soc.SocServiceOrderId into lj_soc
                                       from x in lj_soc.DefaultIfEmpty()
                                       join andr_e in msCRMContext.NewAndromedaExtensionBase on soeb.NewNumber equals andr_e.NewNumber
                                       join andr in msCRMContext.NewAndromedaBase on andr_e.NewAndromedaId equals andr.NewAndromedaId
                                       where soeb.NewServiceorderId == sc.NewServiceorderId
                                       && andr.DeletionStateCode == 0
                                       && andr.Statecode == 0
                                       && andr.Statuscode == 1
                                       select new ClientsOrdersNotCompleted(soeb.NewNumber, soeb.NewObjName, soeb.NewAddress, soeb.NewOrderFrom, null,
                                       soeb.NewName, soeb.NewIncome, soeb.NewOutgone, andr_e.NewLatitude, andr_e.NewLonitude, null, null, soeb.NewResult, null, soeb.NewResultId, null, soeb.NewTransferReason, soeb.NewTechConclusion
                                       , x.SocIncomeLatitude, x.SocIncomeLongitude, x.SocOutcomeLatitide, x.SocOutcomeLongitude)
                                ).AsNoTracking().ToList();
                        //вероятно это лишнее, но на всякий случай сделать стоит, так как кто его знает, вдруг решат менять порядок причин или в целом наименование причин.
                        foreach (var c in cancels) {
                            var result = (from apv in msCRMContext.AttributePicklistValue
                                          join ll in msCRMContext.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                                          join a in msCRMContext.Attribute on apv.AttributeId equals a.AttributeId
                                          join e in msCRMContext.Entity on a.EntityId equals e.EntityId
                                          where a.PhysicalName == "New_Result"
                                          && e.Name == "New_serviceorder"
                                          && apv.Value == c.Result
                                          select new { label = ll.Label, order = apv.DisplayOrder, value = apv.Value }).AsNoTracking().ToList();
                            if (result != null)
                                c.ResultString = result.FirstOrDefault().label;
                            var resultID = (from apv in msCRMContext.AttributePicklistValue
                                            join ll in msCRMContext.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                                            join a in msCRMContext.Attribute on apv.AttributeId equals a.AttributeId
                                            join e in msCRMContext.Entity on a.EntityId equals e.EntityId
                                            where a.PhysicalName == "New_ResultId"
                                            && e.Name == "New_serviceorder"
                                            && apv.Value == c.ReasonResult
                                            select new { label = ll.Label, order = apv.DisplayOrder, value = apv.Value }).AsNoTracking().ToList();
                            if (resultID != null)
                                c.ReasonResultString = resultID.FirstOrDefault().label;
                            var ord_type = (from apv in msCRMContext.AttributePicklistValue
                                            join ll in msCRMContext.LocalizedLabel on apv.AttributePicklistValueId equals ll.ObjectId
                                            join a in msCRMContext.Attribute on apv.AttributeId equals a.AttributeId
                                            join e in msCRMContext.Entity on a.EntityId equals e.EntityId
                                            where a.PhysicalName == "New_order_from"
                                            && e.Name == "New_serviceorder"
                                            && apv.Value == c.OrderType
                                            select new { label = ll.Label, order = apv.DisplayOrder, value = apv.Value }).AsNoTracking().ToList();
                            if (ord_type != null)
                                c.OrderTypeString = ord_type.FirstOrDefault().label;

                            if (!string.IsNullOrEmpty(c.LatitudeObject) && !string.IsNullOrEmpty(c.LongitudeObject)
                            && !string.IsNullOrEmpty(c.IncomeLatitude) && !string.IsNullOrEmpty(c.IncomeLongitude)
                            && !string.IsNullOrEmpty(c.OutgoneLatitude) && !string.IsNullOrEmpty(c.OutgoneLongitude)) {
                                GeoCoordinate c_object = new GeoCoordinate(double.Parse(c.LatitudeObject), double.Parse(c.LongitudeObject));
                                GeoCoordinate c_income = new GeoCoordinate(double.Parse(c.IncomeLatitude), double.Parse(c.IncomeLongitude));
                                GeoCoordinate c_outgone = new GeoCoordinate(double.Parse(c.OutgoneLatitude), double.Parse(c.OutgoneLongitude));

                                c.DistanceIncome = c_income.GetDistanceTo(c_object);
                                c.DistanceOutgone = c_outgone.GetDistanceTo(c_object);
                            }

                            NotCompletedOrdersList.Add(c);
                        }
                    }
                });
            });
        }

        private RelayCommand _FillLatesOrders;
        public RelayCommand FillLatesOrders {
            get => _FillLatesOrders ??= new RelayCommand(async obj => {
                if (SelectedServicemans == null || DateOrder == null)
                    return;
                LatesOrders.Clear();
                List<ServiceorderInfo> coords = (from soc in msCRMContext.ServiceOrderCoordinates
                                                 join soeb in msCRMContext.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                                                 join smeb in msCRMContext.NewServicemanExtensionBase on soeb.NewServicemanServiceorder equals smeb.NewServicemanId
                                                 join andr in msCRMContext.NewAndromedaExtensionBase on soeb.NewAndromedaServiceorder equals andr.NewAndromedaId
                                                 where soeb.NewServicemanServiceorder == SelectedServicemans.ServicemanID
                                                 && soeb.NewDate.Value.Date == DateOrder.Date.AddHours(-5).Date
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                                   && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                                   && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                                 select new ServiceorderInfo(soeb.NewNumber, soeb.NewAddress, soeb.NewObjName, soeb.NewName, soeb.NewTime, (DateTime)soeb.NewIncome, (DateTime)soeb.NewOutgone, soeb.NewTechConclusion)
                                  ).AsNoTracking().ToList();
                if (coords.Count() <= 0)
                    return;
                //каким образом парсить столбец времени
                AnalyzeSelectedServicemanVisibleFlyout = true;
            });
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
                if (!(obj is ToggleButton tb))
                    return;
                Guid? ServicemanID = Guid.Parse(tb.Tag.ToString());
                if (!ServicemanID.HasValue)
                    return;
                SelectedServicemans = tb.IsChecked == true ? ServicemansList.FirstOrDefault(x => x.ServicemanID == ServicemanID.Value) : null;
                FillIntervals.Execute(null);
                GetInfoByServiceman.Execute(null);
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
                                                 join sob in msCRMContext.NewServiceorderBase on soeb.NewServiceorderId equals sob.NewServiceorderId
                                                 join smeb in msCRMContext.NewServicemanExtensionBase on soeb.NewServicemanServiceorder equals smeb.NewServicemanId
                                                 join andr in msCRMContext.NewAndromedaExtensionBase on soeb.NewAndromedaServiceorder equals andr.NewAndromedaId
                                                 where soeb.NewServicemanServiceorder == SelectedServicemans.ServicemanID
                                                 && soeb.NewDate.Value.Date == DateOrder.Date.AddHours(-5).Date
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                                   && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                                   && !string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                                   && !string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                                   && sob.DeletionStateCode == 0
                                                   && sob.Statecode == 0
                                                   && sob.Statuscode == 1
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
                                                 smeb.NewName,
                                                 andr.NewLatitude,
                                                 andr.NewLonitude
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
                App.Current.Dispatcher.Invoke((Action)delegate {
                    List<ServiceorderInfo> coords = (from soc in msCRMContext.ServiceOrderCoordinates
                                                     join soeb in msCRMContext.NewServiceorderExtensionBase on soc.SocServiceOrderId equals soeb.NewServiceorderId
                                                     join sob in msCRMContext.NewServiceorderBase on soeb.NewServiceorderId equals sob.NewServiceorderId
                                                     join smeb in msCRMContext.NewServicemanExtensionBase on soeb.NewServicemanServiceorder equals smeb.NewServicemanId
                                                     join andr in msCRMContext.NewAndromedaExtensionBase on soeb.NewAndromedaServiceorder equals andr.NewAndromedaId
                                                     //where soeb.NewDate.Value.Date == DateOrder.Date.AddHours(-5).Date
                                                     where soeb.NewDate.Value.Date == DateTime.Now.AddDays(-1).Date
                                                       && !string.IsNullOrEmpty(soc.SocIncomeLatitude)
                                                       && !string.IsNullOrEmpty(soc.SocIncomeLongitude)
                                                       && string.IsNullOrEmpty(soc.SocOutcomeLatitide)
                                                       && string.IsNullOrEmpty(soc.SocOutcomeLongitude)
                                                       && soeb.NewOutgone == null
                                                       && sob.DeletionStateCode == 0
                                                       && sob.Statecode == 0
                                                       && sob.Statuscode == 1
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
                                                     smeb.NewName,
                                                     andr.NewLatitude,
                                                     andr.NewLonitude
                                                         )
                                  ).AsNoTracking().ToList();
                    if (coords.Count() <= 0) {
                        //TODO: добавить сообщение, что данных нет
                        return;
                    }
                    foreach (ServiceorderInfo item in coords) {
                        var d = (DateTime.Now.AddHours(-5) - item.Income).Value.Duration();
                        GeoCoordinate c1 = new GeoCoordinate(Convert.ToDouble(item.IncomeLatitude), Convert.ToDouble(item.IncomeLongitude));
                        GeoCoordinate c2 = new GeoCoordinate(Convert.ToDouble(item.Andr_lat), Convert.ToDouble(item.Andr_lon));
                        GMapMarker marker = new GMapMarker(new PointLatLng(Convert.ToDouble(item.IncomeLatitude), Convert.ToDouble(item.IncomeLongitude))) {
                            Shape = new Ellipse {
                                Width = 20,
                                Height = 20,
                                //Stroke = Brushes.Purple,
                                Stroke = c1.GetDistanceTo(c2) > 150.0 ? Brushes.Red : Brushes.Purple,
                                StrokeThickness = 7.5,
                                //ToolTip = string.Format("{0} - {1} ({2})", item.Number, item.Name, item.Address),
                                ToolTip = string.Format("{0} ({1})" + Environment.NewLine + "{2} {3}" + Environment.NewLine + "{4} {5}",
                                item.TechName,
                                string.Format("на объекте: {0}ч. {1}мин. {2}сек.", d.Hours, d.Minutes, d.Seconds),
                                item.Number,
                                item.ObjectName + " " + item.Address,
                                item.Name,
                                c1.GetDistanceTo(c2) > 150.0 ? Environment.NewLine + string.Format("расстояние от объекта: {0}", c1.GetDistanceTo(c2).ToString()) : null
                                ),
                                AllowDrop = false
                            }
                        };
                        gmaps_contol.Markers.Add(marker);
                    }
                    Timer.Start();
                });
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
