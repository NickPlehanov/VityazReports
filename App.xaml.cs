using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Windows;

namespace VityazReports {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            AppCenter.Start("4f8515cc-9fc7-4296-8389-b9d8cd76148b",
                   typeof(Analytics), typeof(Crashes));
        }
    }
}
