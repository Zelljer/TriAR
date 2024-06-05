using TriAR.Model;
using TriAR.Window;

namespace TriAR.ViewModel
{
    public class MainWindowViewModel
    {
        public RelayCommand ParseButton_Click => new RelayCommand(obj =>
        {
            DatabaseTable table = new DatabaseTable();
            table.ShowDialog();
        });

        public RelayCommand WeatherButton_Click => new RelayCommand(obj =>
        {
            WeatherSearch weather = new WeatherSearch();
            weather.ShowDialog();
        });
    }
}
