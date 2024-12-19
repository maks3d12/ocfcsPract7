using OpenMeteo;
using SunClouds.Model;
using SunClouds.ViewModel.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SunClouds.ViewModel
{
    internal class WeatherViewModel : BindingHelper
    {
        private string _feelsLike;
        private string _minTemperature;
        private string _maxTemperature;
        private string _pressure;
        private string _humidity;
        private string _windSpeed;
        private string _windDirection;
        private string _reallyTemperature;
        private bool NeedUpdate = true;
        private DateTime dateTimeNow;
        private bool Cilisus;

        private ObservableCollection<WithAlldayBlock> _allday;
        public ObservableCollection<WithAlldayBlock> Allday
        {
            get { return _allday; }
            set { _allday = value; OnPropertyChanged(); }
        }

        #region Методы для выгрузки
        public string GetTemperature(int value)
        {
            return $"{value}°";
        }

        public string GetPressure(int value)
        {
            return $"{value} мм рт. ст.";
        }

        public string GetHumidity(int value)
        {
            return $"{value}%";
        }

        public string GetWindSpeed(int value)
        {
            return $"{value} м/с";
        }

        public string GetWindDirection(int value)
        {
            return $"{value}";
        }
        public string GetImg(int index)
        {
            string Icon;
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
                    Icon = "/Resources/Rainy,png";
                    return Icon;
                case 63:
                    Icon = "/Resources/Rainy,png";
                    return Icon;
                case 65:
                    Icon = "/Resources/Rainy,png";
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
                    Icon = "/Resources/Rainy,png";
                    return Icon;
                case 81:
                    Icon = "/Resources/Rainy,png";
                    return Icon;
                case 82:
                    Icon = "/Resources/Rainy,png";
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
                    Icon = "/Resources/Sunny..png";
                    return Icon;
            }
        }
        public int GetFarengeit(int Cilius)
        {
            return Convert.ToInt32((Cilius * 9 / 5) + 32);
        }

        #endregion
        #region Конструктор
        public WeatherViewModel()
        {
            Cilisus = SunClouds.Properties.Settings.Default.degreesCelsius;
            Allday = new ObservableCollection<WithAlldayBlock>();
            Task.Run(async () => await RunAsync()).GetAwaiter().GetResult();
        }
        #endregion
        #region Подвязка
        public string ReallyTemperature
        {
            get { return _reallyTemperature; }
            set { _reallyTemperature = value; OnPropertyChanged(); }
        }

        public string FeelsLike
        {
            get { return _feelsLike; }
            set { _feelsLike = value; OnPropertyChanged(); }
        }

        public string MinTemperature
        {
            get { return _minTemperature; }
            set { _minTemperature = value; OnPropertyChanged(); }
        }
        public string MaxTemperature
        {
            get { return _maxTemperature; }
            set { _maxTemperature = value; OnPropertyChanged(); }
        }

        public string Pressure
        {
            get { return _pressure; }
            set { _pressure = value; OnPropertyChanged(); }
        }

        public string Humidity
        {
            get { return _humidity; }
            set { _humidity = value; OnPropertyChanged(); }
        }

        public string WindSpeed
        {
            get { return _windSpeed; }
            set { _windSpeed = value; OnPropertyChanged(); }
        }

        public string WindDirection
        {
            get { return _windDirection; }
            set { _windDirection = value; OnPropertyChanged(); }
        }
        #endregion
        public async Task RunAsync()
        {
            OpenMeteoClient client = new OpenMeteoClient();
            WeatherForecast weatherData = await client.QueryAsync(SunClouds.Properties.Settings.Default.CurrentCity);


            Pressure = GetPressure(Convert.ToInt32(weatherData.Hourly.Surface_pressure[0] * 0.750064));
            Humidity = GetHumidity(Convert.ToInt32(weatherData.Hourly.Relativehumidity_2m[0]));
            WindSpeed = GetWindSpeed(Convert.ToInt32(weatherData.CurrentWeather.Windspeed));
            WindDirection = GetWindDirection(Convert.ToInt32(weatherData.CurrentWeather.WindDirection));

            DateTime dt = DateTime.Parse(weatherData.CurrentWeather.Time);
            if (dt.Hour!= dateTimeNow.Hour) {
                NeedUpdate = true;
            }

            if (Cilisus)
            {
                ReallyTemperature = GetTemperature(Convert.ToInt32(weatherData.Hourly.Temperature_2m[0]));
                FeelsLike = GetTemperature(Convert.ToInt32(weatherData.Hourly.Apparent_temperature[0]));
                MinTemperature = GetTemperature(Convert.ToInt32(weatherData.Daily.Apparent_temperature_min[0]));
                MaxTemperature = GetTemperature(Convert.ToInt32(weatherData.Daily.Apparent_temperature_max[0]));
                if (Allday.Count < 11 && NeedUpdate)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        Allday.Add(new WithAlldayBlock(dt.AddHours(i).ToString("HH:mm"),
                            GetTemperature(Convert.ToInt32(weatherData.Hourly.Temperature_2m[i])),
                            GetTemperature(Convert.ToInt32(weatherData.Hourly.Apparent_temperature[i])),
                            GetHumidity(Convert.ToInt32(weatherData.Hourly.Relativehumidity_2m[i])),
                            GetImg((int)weatherData.Hourly.Weathercode[i])));
                    }
                    NeedUpdate = false;
                    dateTimeNow = dt;
                }
            }
            else
            {
                ReallyTemperature = GetTemperature(GetFarengeit(Convert.ToInt32(weatherData.Hourly.Temperature_2m[0])));
                FeelsLike = GetTemperature(GetFarengeit(Convert.ToInt32(weatherData.Hourly.Apparent_temperature[0])));
                MinTemperature = GetTemperature(GetFarengeit(Convert.ToInt32(weatherData.Daily.Apparent_temperature_min[0])));
                MaxTemperature = GetTemperature(GetFarengeit(Convert.ToInt32(weatherData.Daily.Apparent_temperature_max[0])));
                if (Allday.Count < 11 && NeedUpdate)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        Allday.Add(new WithAlldayBlock(dt.AddHours(i).ToString("HH:mm"),
                            GetTemperature(GetFarengeit(Convert.ToInt32(weatherData.Hourly.Temperature_2m[i]))),
                            GetTemperature(GetFarengeit(Convert.ToInt32(weatherData.Hourly.Apparent_temperature[i]))),
                            GetHumidity(GetFarengeit(Convert.ToInt32(weatherData.Hourly.Relativehumidity_2m[i]))),
                            GetImg((int)weatherData.Hourly.Weathercode[i])));
                    }
                    NeedUpdate = false;
                    dateTimeNow = dt;
                }
            }
            

            
        }
    }
}