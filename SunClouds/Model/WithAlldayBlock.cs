using SunClouds.ViewModel.Helpers;

namespace SunClouds.Model
{
    internal class WithAlldayBlock: BindingHelper
    {

        private string _time;
        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        private string _temperature;
        public string Temperature
        {
            get { return _temperature; }
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        private string _fellslike;
        public string Fellslike
        {
            get { return _fellslike; }
            set
            {
                _fellslike = value;
                OnPropertyChanged();
            }
        }

        private string _humidity;
        public string Humidity
        {
            get { return _humidity; }
            set
            {
                _humidity = value;
                OnPropertyChanged();
            }
        }

        private string _imgSource;
        public string ImgSource
        {
            get { return _imgSource; }
            set
            {
                _imgSource = value;
                OnPropertyChanged();
            }
        }
        public  WithAlldayBlock(string Time, string Temperature, string Fellslike, string Humidity, string ImgSource)
        {
            this.Time = Time;
            this.Temperature = Temperature;
            this.Fellslike = Fellslike;
            this.Humidity = Humidity;
            this.ImgSource = ImgSource;
        }
    }
}
