using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using TriAR.Model;

namespace TriAR.ViewModel
{
    public class WeatherSearchViewModel
    {
        private string _townName;
        public string TownName
        {
            get => _townName;
            set
            {
                _townName = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SearchButton_Click => new RelayCommand(obj =>
        {
            SearchWeather(TownName);
        });

        private event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void SearchWeather(string townName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + townName + "&units=metric&appid=9c06ee257d644d659503518a39b99b25";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                MessageBox.Show("Темперантура в городе " + weatherResponse.Name + " на данный момент: " + weatherResponse.Main.Temp + " °C");
            }
            catch
            {
                MessageBox.Show("Произошла ошибка, проверьте вводимые данные или подключение к интернету.");
            }
        }
    }
}
