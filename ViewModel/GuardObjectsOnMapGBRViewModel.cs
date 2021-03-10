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
    }
}
