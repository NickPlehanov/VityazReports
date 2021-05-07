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

            LoadingText = "Открыто окно отчёта. Для просмотра другого отчёта, закройте окно текущего отчёта";
        }
        /// <summary>
        /// Команда закрытия окна, 
        /// </summary>
        private RelayCommand _WindowCloseCommand;
        public RelayCommand WindowCloseCommand {
            get => _WindowCloseCommand ??= new RelayCommand(async obj => {
                Environment.Exit(0);
            });
        }

        private bool _Loading;
        public bool Loading {
            get => _Loading;
            set {
                _Loading = value;
                OnPropertyChanged(nameof(Loading));
            }
        }

        private string _LoadingText;
        public string LoadingText {
            get => _LoadingText;
            set {
                _LoadingText = value;
                OnPropertyChanged(nameof(LoadingText));
            }
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
                        GuardObjectsOnMap map = new GuardObjectsOnMap();
                        map.Show();
                        break;
                    case "B904A30B-16B1-4F59-A76D-BD981E18C930": //изм.абон.платы
                        ChangeCost changeCost = new ChangeCost();
                        changeCost.Show();
                        break;
                    case "FA4DD0A5-5B15-45B4-A55A-433267FA50FF": //Акты по тревогам
                        ActsByAlarm acts = new ActsByAlarm();
                        acts.Show();
                        break;
                    case "A35A2859-3E10-42F1-9E9B-5F29B5E953D9": //Опоздания пульта
                        LatePult latePult = new LatePult();
                        latePult.Show();
                        break;
                    case "8A7E33DF-E27D-413C-80D5-E3812B57853C": //Опоздания ГБР
                        LateGBR lateGBR = new LateGBR();
                        lateGBR.Show();
                        break;
                    case "7C9C1F49-6218-4C9A-8F17-126626E5D1D3": //Регламентные работы
                        ReglamentWorks rw = new ReglamentWorks();
                        rw.Show();
                        break;
                    case "B8441A22-FCBC-45E3-A767-4B593027018D": //Экипажи на карте
                        GuardsOnMap gom = new GuardsOnMap();
                        gom.Show();
                        break;
                    case "F8FC782C-AD41-4BE7-9D50-293F7E45D6AB": //оборудование с демонтажа
                        UnmountDevices ud = new UnmountDevices();
                        ud.Show();
                        break;
                    case "A7DCE7A5-D92E-4568-B531-94A888F4787A": //техники на карте
                        ServicemanOnMap som = new ServicemanOnMap();
                        som.Show();
                        break;
                    case "DD73D03F-CC93-49E5-AF8B-F49464B5EABA": //анализ техников
                        AnalyzeServicemans asm = new AnalyzeServicemans();
                        asm.Show();
                        break;
                    case "E56507EA-3FB3-49CF-9528-2634A4C0E21F": //корпоротивные клиенты
                        CorpClients cc = new CorpClients();
                        cc.Show();
                        break;
                }
            }, obj => obj != null);
        }
    }
}
