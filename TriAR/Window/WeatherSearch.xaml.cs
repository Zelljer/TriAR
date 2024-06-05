using TriAR.ViewModel;
namespace TriAR.Window
{
    /// <summary>
    /// Логика взаимодействия для WeatherSearch.xaml
    /// </summary>
    public partial class WeatherSearch : System.Windows.Window
    {
        public WeatherSearch()
        {
            InitializeComponent();
            DataContext = new WeatherSearchViewModel();
        }
    }
}
