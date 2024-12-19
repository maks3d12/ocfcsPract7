using OpenMeteo;
using SunClouds.ViewModel.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SunClouds.ViewModel
{
    internal class AuthViewModel : BindingHelper
    {
        private bool flag = false;

        private string _city;
        public string City
        {
            get { return _city; }
            set { _city = value; OnPropertyChanged(); }
        }

        public BindableCommand AuthorizationCommand { get; set; }
        public BindableCommand ClearCityTextBoxCommand { get; set; }
        public AuthViewModel()
        {
            AuthorizationCommand = new BindableCommand(_ => Authorization());
            ClearCityTextBoxCommand = new BindableCommand(_ => ClearCityTextBox());
        }

        private void ClearCityTextBox()
        {
            City = string.Empty;
        }

        private void Authorization()
        {
            Task.Run(async () => await RunAsync()).GetAwaiter().GetResult();

            if (City != null && City.Length > 0)
            {
                if (flag)
                {
                    Properties.Settings.Default.CurrentCity = City;
                    App.mainWindow = new MainWindow();
                    App.mainWindow.Show();
                    App.authWindow.Close();
                }
                else
                {
                    MessageBox.Show("Введено некорректное название города.", "Город не найден!", MessageBoxButton.OK, MessageBoxImage.Warning); 
                }
            }
            else
            {
                MessageBox.Show("Поле ввода название города не заполнено.", "Пустое поле!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public async Task RunAsync()
        {
            OpenMeteoClient client = new OpenMeteoClient();
            WeatherForecast weatherData = await client.QueryAsync(City);

            if (weatherData != null)
            {
                flag = true;
            }
        }
    }
}