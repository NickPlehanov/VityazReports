using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VityazReports.Helpers {
    /// <summary>
    /// Логика взаимодействия для FlatControl.xaml
    /// </summary>
    public partial class FlatControl : UserControl {
        public FlatControl() {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public ImageSource ImageSource { get => (ImageSource)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value); }
        public ICommand ClickCommand { get => (ICommand)GetValue(ClickCommandProperty); set => SetValue(ClickCommandProperty, value); }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(FlatControl), new PropertyMetadata(defaultValue: ""));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(string), typeof(FlatControl), new PropertyMetadata(defaultValue: ""));
        public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(FlatControl), new PropertyMetadata(defaultValue: null));
    }
}
