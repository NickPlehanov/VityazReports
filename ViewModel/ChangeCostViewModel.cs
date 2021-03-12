using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Helpers;

namespace VityazReports.ViewModel {
    public class ChangeCostViewModel : BaseViewModel {
        public ChangeCostViewModel() {
            FilterFlyoutVisible = true;
        }

        private DateTime _DateStart;
        public DateTime DateStart {
            get {
                if (_DateStart == DateTime.MinValue)
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                else
                    return _DateStart;
            }
            set {
                _DateStart = value;
                OnPropertyChanged("DateStart");
            }
        }

        private DateTime _DateEnd;
        public DateTime DateEnd {
            get {
                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
                if (_DateEnd == DateTime.MinValue)
                    return DateTime.Now;
                else
                    return DateTime.Parse(_DateEnd.ToShortDateString());
            }
            set {
                _DateEnd = value;
                OnPropertyChanged("DateEnd");
            }
        }

        private bool _FilterFlyoutVisible;
        public bool FilterFlyoutVisible {
            get => _FilterFlyoutVisible;
            set {
                _FilterFlyoutVisible = value;
                OnPropertyChanged(nameof(FilterFlyoutVisible));
            }
        }

        private RelayCommand _OpenFlyoutFilterCommand;
        public RelayCommand OpenFlyoutFilterCommand {
            get => _OpenFlyoutFilterCommand ??= new RelayCommand(async obj => {
                FilterFlyoutVisible = !FilterFlyoutVisible;
            });
        }

        private RelayCommand _RefreshDataCommand;
        public RelayCommand RefreshDataCommand {
            get => _RefreshDataCommand ??= new RelayCommand(async obj => {

            });
        }
    }
}
