using SunClouds.ViewModel;
using System.Windows.Controls;

namespace SunClouds.View
{
    /// <summary>
    /// Interaction logic for WeatherData.xaml
    /// </summary>
    public partial class WeatherData : Page
	{
		public WeatherData()
		{
			InitializeComponent();
			DataContext = new WeatherViewModel();
        }
	}
}
