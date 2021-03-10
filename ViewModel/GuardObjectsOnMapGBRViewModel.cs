using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using VityazReports.Helpers;

namespace VityazReports.ViewModel {
    public class GuardObjectsOnMapGBRViewModel : BaseViewModel {
        public GuardObjectsOnMapGBRViewModel() {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gmaps_contol.MapProvider = GMap.NET.MapProviders.YandexMapProvider.Instance;
            gmaps_contol.MinZoom = 5;
            gmaps_contol.MaxZoom = 17;
            gmaps_contol.Zoom = 5;
            gmaps_contol.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            gmaps_contol.CanDragMap = true;
            gmaps_contol.DragButton = MouseButton.Left;
            gmaps_contol.CenterPosition = new PointLatLng(55.159904, 61.401919);
        }
        public GMapControl gmaps_contol { get; set; } = new GMapControl();

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

        private RelayCommand _ShowGroups;
        public RelayCommand ShowGroups {
            get => _ShowGroups ??= new RelayCommand(async obj => {
                FlyoutShowGroupsVisibleState = !FlyoutShowGroupsVisibleState;
            });
        }
    }
}
