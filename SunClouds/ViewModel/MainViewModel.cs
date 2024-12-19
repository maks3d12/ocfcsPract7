using SunClouds.ViewModel.Helpers;
using OpenMeteo;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SunClouds.ViewModel
{
    internal class MainViewModel : BindingHelper
    {
        private string weathercodeTemperature;
        private string weathercodeTemperature2;
        private string weathercodeTemperature3;

        private string apparentTemperature;
        private string apparentTemperature2;
        private string apparentTemperature3;

        private string time;
        private string time2;
        private string time3;

        private string icon;
        private string icon2;
        private string icon3;

        private string city;

        public string WeathercodeTemperature
        {
            get { return weathercodeTemperature; }
            set
            {
                weathercodeTemperature = value;
                OnPropertyChanged();
            }
        }

        public string WeathercodeTemperature2
        {
            get { return weathercodeTemperature2; }
            set
            {
                weathercodeTemperature2 = value;
                OnPropertyChanged();
            }
        }

        public string WeathercodeTemperature3
        {
            get { return weathercodeTemperature3; }
            set
            {
                weathercodeTemperature3 = value;
                OnPropertyChanged();
            }
        }

        public string ApparentTemperature
        {
            get { return apparentTemperature; }
            set
            {
                apparentTemperature = value;
                OnPropertyChanged();
            }
        }

        public string ApparentTemperature2
        {
            get { return apparentTemperature2; }
            set
            {
                apparentTemperature2 = value;
                OnPropertyChanged();
            }
        }

        public string ApparentTemperature3
        {
            get { return apparentTemperature3; }
            set
            {
                apparentTemperature3 = value;
                OnPropertyChanged();
            }
        }
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }

        public string Time2
        {
            get { return time2; }
            set
            {
                time2 = value;
                OnPropertyChanged();
            }
        }

        public string Time3
        {
            get { return time3; }
            set
            {
                time3 = value;
                OnPropertyChanged();
            }
        }

        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                OnPropertyChanged();
            }
        }

        public string Icon2
        {
            get { return icon2; }
            set
            {
                icon2 = value;
                OnPropertyChanged();
            }
        }

        public string Icon3
        {
            get { return icon3; }
            set
            {
                icon3 = value;
                OnPropertyChanged();
            }
        }
        public string City
        {
            get { return city; }
            set
            {
                city = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            Task.Run(async () => await RunAsync()).GetAwaiter().GetResult();
        }
        public string GetWeatherImage(int index)
        {
            switch (index)
            {
                case 0:
                    Icon = "/Resources/Sunny.png";
                    return Icon;
                case 1:
                    Icon = "/Resources/Sunny.png";
                    return Icon;
                case 2:
                    Icon = "/Resources/Cloudy.png";
                    return Icon;
                case 3:
                    Icon = "/Resources/Cloudy.png";
                    return Icon;
                case 45:
                    Icon = "/Resources/Blizzard.png";
                    return Icon;
                case 48:
                    Icon = "/Resources/Downpour.png";
                    return Icon;
                case 51:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 53:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 55:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 56:
                    Icon = "/Resources/Downpour.png";
                    return Icon;
                case 57:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 61:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 63:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 65:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 66:
                    Icon = "/Resources/Downpour.png";
                    return Icon;
                case 67:
                    Icon = "/Resources/Downpour.png";
                    return Icon;
                case 71:
                    Icon = "/Resources/Snow.png";
                    return Icon;
                case 73:
                    Icon = "/Resources/Snow.png";
                    return Icon;
                case 75:
                    Icon = "/Resources/Snow.png";
                    return Icon;
                case 77:
                    Icon = "/Resources/Snow.png";
                    return Icon;
                case 80:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 81:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 82:
                    Icon = "/Resources/Rainy.png";
                    return Icon;
                case 85:
                    Icon = "/Resources/Snow.png";
                    return Icon;
                case 86:
                    Icon = "/Resources/Snow.png";
                    return Icon;
                case 95:
                    Icon = "/Resources/Thunderstorm.png";
                    return Icon;
                case 96:
                    Icon = "/Resources/Thunderstorm.png";
                    return Icon;
                case 99:
                    Icon = "/Resources/Thunderstorm.png";
                    return Icon;
                default:
                    Icon = "/Resources/Sunny.png";
                    return Icon;
            }

        }

        public async Task RunAsync()
        {


        
            OpenMeteoClient client = new OpenMeteoClient();
            WeatherForecast weatherData = await client.QueryAsync(Properties.Settings.Default.CurrentCity);
            string weatherCode = client.WeathercodeToString((int)weatherData.Daily.Weathercode[0]) + "." + " " + weatherData.Hourly.Temperature_2m[0].ToString() + "°";
            string weatherCode2 = client.WeathercodeToString((int)weatherData.Daily.Weathercode[1]) + "." + " " + weatherData.Hourly.Temperature_2m[1].ToString() + "°";
            string weatherCode3 = client.WeathercodeToString((int)weatherData.Daily.Weathercode[2]) + "." + " " + weatherData.Hourly.Temperature_2m[2].ToString() + "°";

            string Apparent_temperature = "Ощущается как " + weatherData.Hourly.Apparent_temperature[0].ToString() + "°";
            string Apparent_temperature2 = "Ощущается как " + weatherData.Hourly.Apparent_temperature[1].ToString() + "°";
            string Apparent_temperature3 = "Ощущается как " + weatherData.Hourly.Apparent_temperature[2].ToString() + "°";

            DateTime dt = DateTime.Parse(weatherData.CurrentWeather.Time);
            string time = dt.ToString("HH:mm");
            string time2 = dt.AddHours(1).ToString("HH:mm");
            string time3 = dt.AddHours(2).ToString("HH:mm");

            Icon = GetWeatherImage((int)weatherData.Daily.Weathercode[0]);
            Icon2 = GetWeatherImage((int)weatherData.Daily.Weathercode[1]);
            Icon3 = GetWeatherImage((int)weatherData.Daily.Weathercode[2]);
            WeathercodeTemperature = weatherCode;
            WeathercodeTemperature2 = weatherCode2;
            WeathercodeTemperature3 = weatherCode3;
            ApparentTemperature = Apparent_temperature;
            ApparentTemperature2 = Apparent_temperature2;
            ApparentTemperature3 = Apparent_temperature3;
            Time = time;
            Time2 = time2;
            Time3 = time3;
            City = Properties.Settings.Default.CurrentCity;
        }
    }
}

