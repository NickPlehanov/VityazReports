using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using VityazReports.Data;
using VityazReports.Helpers;
using VityazReports.Models.MainWindow;
using VityazReports.Views;

namespace VityazReports.ViewModel {
    public class MainWindowViewModel : BaseViewModel {

        private readonly ReportBaseContext reportBaseContext;
        
        public MainWindowViewModel() {
            reportBaseContext = new ReportBaseContext();

            string login = Environment.UserName;
            if (string.IsNullOrEmpty(login))
                throw new Exception("Логин пользователя не может быть пустым!");
            var _rpt_lst = (from r in reportBaseContext.Reports
                           join ur in reportBaseContext.UsersReports on r.RptId equals ur.RptId
                           where ur.UsrLogin.Equals(login)
                           select r).ToList();
            if (_rpt_lst.Count <= 0)
                throw new Exception("Для указанного пользователя нет доступных отчётов");
            foreach (var item in _rpt_lst) 
                ReportList.Add(item);            

        }

        private ObservableCollection<Reports> _ReportList = new ObservableCollection<Reports>();
        public ObservableCollection<Reports> ReportList {
            get => _ReportList;
            set {
                _ReportList = value;
                OnPropertyChanged(nameof(ReportList));
            }
        }

        private RelayCommand _SelectedReport;
        public RelayCommand SelectedReport {
            get => _SelectedReport ??= new RelayCommand(async obj => {
                Button btn = obj is Button ? obj as Button : null;

                if (btn == null)
                    throw new Exception("Не выбран отчёт");

                switch (btn.Tag.ToString().ToUpper()) {
                    case "23F71A51-F909-417C-9B09-69534715C689": //охр. объекты на карте
                        GuardObjectsOnMapGBRViewModel vm = new GuardObjectsOnMapGBRViewModel();
                        GuardObjectsOnMap map = new GuardObjectsOnMap();
                        map.ShowDialog();
                        break;
                }
            }, obj=>obj!=null);
        }
    }
}
