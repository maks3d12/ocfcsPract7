using SunClouds.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SunClouds
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AuthWindow authWindow;
        public static MainWindow mainWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            authWindow = new AuthWindow();
            authWindow.Show();

            // Подписываемся на изменение времени
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Устанавливаем тему при запуске
            SetAppThemeByTime();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SetAppThemeByTime();
        }

        private void SetAppThemeByTime()
        {
            int currentHour = DateTime.Now.Hour;

            var dictionary = new ResourceDictionary();

            if (currentHour >= 0 && currentHour <= 3)
            {                dictionary = new ResourceDictionary { Source = new Uri($"/Resources/NightTheme.xaml", UriKind.Relative) };
            }
            else if (currentHour >= 4 && currentHour <= 11)
            {
                dictionary = new ResourceDictionary { Source = new Uri($"/Resources/TwilightTheme.xaml", UriKind.Relative) };
            }
            else if (currentHour >= 12 && currentHour <= 16)
            {
                dictionary = new ResourceDictionary { Source = new Uri($"/Resources/DayTheme.xaml", UriKind.Relative) };
            }
            else if (currentHour >= 17 && currentHour <= 23)
            {
                dictionary = new ResourceDictionary { Source = new Uri($"/Resources/TwilightTheme.xaml", UriKind.Relative) };
            }

            Current.Resources.MergedDictionaries.RemoveAt(0);
            Current.Resources.MergedDictionaries.Insert(0, dictionary);
        }
    }
}
