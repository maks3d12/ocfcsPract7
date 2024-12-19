using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;


namespace OpenMeteo
{
    /// <summary>
    /// Handles GET Requests and performs API Calls.
    /// </summary>
    public class OpenMeteoClient
    {
        private readonly string _weatherApiUrl = "https://api.open-meteo.com/v1/forecast";
        private readonly string _geocodeApiUrl = "https://geocoding-api.open-meteo.com/v1/search";
        private readonly string _airQualityApiUrl = "https://air-quality-api.open-meteo.com/v1/air-quality";
        private readonly HttpController httpController;
        (float latitude, float longitude) moscow = (55.7558f, 37.6173f);


        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object and sets the neccessary variables (httpController, CultureInfo)
        /// </summary>
        public OpenMeteoClient()
        {
            httpController = new HttpController();
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="location">Name of city</param>
        /// <returns>If successful returns an awaitable Task containing WeatherForecast or NULL if request failed</returns>
        public async Task<WeatherForecast> QueryAsync(string location)
        {
            GeocodingOptions geocodingOptions = new GeocodingOptions(location);

            // Get location Information
            GeocodingApiResponse response = await GetGeocodingDataAsync(geocodingOptions);
            if (response == null || response.Locations == null)
                return null;

            WeatherForecastOptions options = new WeatherForecastOptions
            {
                Latitude = response.Locations[0].Latitude,
                Longitude = response.Locations[0].Longitude,
                Current_Weather = true   
            };

            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="options">Geocoding options</param>
        /// <returns>If successful awaitable <see cref="Task"/> or NULL</returns>
        public async Task<WeatherForecast> QueryAsync(GeocodingOptions options)
        {
            // Get City Information
            GeocodingApiResponse response = await GetLocationDataAsync(options);
            if (response == null || response.Locations == null)
                return null;

            WeatherForecastOptions weatherForecastOptions = new WeatherForecastOptions
            {
                Latitude = response.Locations[0].Latitude,
                Longitude = response.Locations[0].Longitude,
                Current_Weather = true
            };

            return await GetWeatherForecastAsync(weatherForecastOptions);
        }

        /// <summary>
        /// Performs one GET-Request
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast> QueryAsync(WeatherForecastOptions options)
        {
            try
            {
                return await GetWeatherForecastAsync(options);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Performs one GET-Request to get weather information
        /// </summary>
        /// <param name="latitude">City latitude</param>
        /// <param name="longitude">City longitude</param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast> QueryAsync(float latitude, float longitude)
        {
            WeatherForecastOptions options = new WeatherForecastOptions
            {
                Latitude = latitude,
                Longitude = longitude,
                Current_Weather = false
            };
            return await QueryAsync(options);
        }

        /// <summary>
        /// Gets Weather Forecast for a given location with individual options
        /// </summary>
        /// <param name="location"></param>
        /// <param name="options"></param>
        /// <returns><see cref="WeatherForecast"/> for the FIRST found result for <paramref name="location"/></returns>
        public async Task<WeatherForecast> QueryAsync(string location, WeatherForecastOptions options)
        {
            GeocodingApiResponse geocodingApiResponse = await GetLocationDataAsync(location);
            if (geocodingApiResponse == null || geocodingApiResponse.Locations == null)
                return null;
            
            options.Longitude = geocodingApiResponse.Locations[0].Longitude;
            options.Latitude = geocodingApiResponse.Locations[0].Latitude;

            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Gets air quality data for a given location with individual options
        /// </summary>
        /// <param name="options">options for air quality request</param>
        /// <returns><see cref="AirQuality"/> if successfull or <see cref="null"/> if failed</returns>
        public async Task<AirQuality> QueryAsync(AirQualityOptions options)
        {
            return await GetAirQualityAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to Open-Meteo Geocoding API 
        /// </summary>
        /// <param name="location">Name of a location or city</param>
        /// <returns></returns>
        public async Task<GeocodingApiResponse> GetLocationDataAsync(string location)
        {
            GeocodingOptions geocodingOptions = new GeocodingOptions(location);

            return await GetLocationDataAsync(geocodingOptions);
        }

        public async Task<GeocodingApiResponse> GetLocationDataAsync(GeocodingOptions options)
        {
            return await GetGeocodingDataAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to get a (float, float) tuple
        /// </summary>
        /// <param name="location">Name of a city or location</param>
        /// <returns>(latitude, longitude) tuple of first found location or null if no location was found</returns>
        public async Task<(float latitude, float longitude)> GetLocationLatitudeLongitudeAsync(string location)
        {
            GeocodingApiResponse response = await GetLocationDataAsync(location);
            if (response == null || response.Locations == null)

                return moscow;
            return (response.Locations[0].Latitude, response.Locations[0].Longitude);
        }

        public WeatherForecast Query(WeatherForecastOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }

        public WeatherForecast Query(float latitude, float longitude)
        {
            return QueryAsync(latitude, longitude).GetAwaiter().GetResult();
        }

        public WeatherForecast Query(string location, WeatherForecastOptions options)
        {
            return QueryAsync(location, options).GetAwaiter().GetResult();
        }

        public WeatherForecast Query(GeocodingOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }

        public WeatherForecast Query(string location)
        {
            return QueryAsync(location).GetAwaiter().GetResult();
        }

        public AirQuality Query(AirQualityOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }

        private async Task<AirQuality> GetAirQualityAsync(AirQualityOptions options)
        {
            try
            {
                HttpResponseMessage response = await httpController.Client.GetAsync(MergeUrlWithOptions(_airQualityApiUrl, options));
                response.EnsureSuccessStatusCode();

                AirQuality airQuality = await JsonSerializer.DeserializeAsync<AirQuality>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return airQuality;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Converts a given weathercode to it's string representation
        /// </summary>
        /// <param name="weathercode"></param>
        /// <returns><see cref="string"/> Weathercode string representation</returns>
        public string WeathercodeToString(int weathercode)
        {
            switch (weathercode)
            {
                case 0:
                    return "Чистое небо";
                case 1:
                    return "В основном ясно";
                case 2:
                    return "Переменная облачность";
                case 3:
                    return "Пасмурно";
                case 45:
                    return "Туман";
                case 48:
                    return "Осаждающий иней туман";
                case 51:
                    return "Легкий моросящий дождь";
                case 53:
                    return "Умеренный моросящий дождь";
                case 55:
                    return "Густой моросящий дождь";
                case 56:
                    return "Легкая моросящая изморось";
                case 57:
                    return "Густой моросящий дождь";
                case 61:
                    return "Небольшой дождь";
                case 63:
                    return "Умеренный дождь";
                case 65:
                    return "Ливень";
                case 66:
                    return "Легкий ледяной дождь";
                case 67:
                    return "Сильный ледяной дождь";
                case 71:
                    return "Выпал небольшой снег";
                case 73:
                    return "Умеренный снегопад";
                case 75:
                    return "Выпадает сильный снег";
                case 77:
                    return "Снежные зерна";
                case 80:
                    return "Небольшой ливень";
                case 81:
                    return "Умеренные ливневые дожди";
                case 82:
                    return "Сильные ливневые дожди";
                case 85:
                    return "Небольшой снегопад";
                case 86:
                    return "Сильные снегопады";
                case 95:
                    return "Гроза";
                case 96:
                    return "Гроза с небольшим градом";
                case 99:
                    return "Гроза с сильным градом";
                default:
                    return "Неверный код погоды";
            }
        }

        private async Task<WeatherForecast> GetWeatherForecastAsync(WeatherForecastOptions options)
        {
            try
            {
                HttpResponseMessage response = await httpController.Client.GetAsync(MergeUrlWithOptions(_weatherApiUrl, options));
                response.EnsureSuccessStatusCode();

                WeatherForecast weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return weatherForecast;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }

        }

        private async Task<GeocodingApiResponse> GetGeocodingDataAsync(GeocodingOptions options)
        {
          
            try
            {
                HttpResponseMessage response =  await httpController.Client.GetAsync(MergeUrlWithOptions(_geocodeApiUrl, options));

                response.EnsureSuccessStatusCode();

                GeocodingApiResponse geocodingData = await JsonSerializer.DeserializeAsync<GeocodingApiResponse>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                
                return geocodingData;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Can't find " + options.Name + ". Please make sure that the name is valid.");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private string MergeUrlWithOptions(string url, WeatherForecastOptions options)
        {
            string qieru = "";
            if (options == null) return url;

            UriBuilder uri = new UriBuilder(url);
            bool isFirstParam = false;

            // If no query given, add '' to start the query string
            if (qieru == string.Empty)
            {
                qieru = "";

                // isFirstParam becomes true because the query string is new
                isFirstParam = true;
            }

            // Add the properties
            
            // Begin with Latitude and Longitude since they're required
            if (isFirstParam)
                qieru += "latitude=" +  options.Latitude.ToString(CultureInfo.InvariantCulture);
            else
                qieru += "&latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);

            qieru += "&longitude=" + options.Longitude.ToString(CultureInfo.InvariantCulture);

            qieru += "&temperature_unit=" + options.Temperature_Unit.ToString();
            qieru += "&windspeed_unit=" + options.Windspeed_Unit.ToString();
            qieru += "&precipitation_unit=" + options.Precipitation_Unit.ToString();
            if (options.Timezone != string.Empty)
                qieru += "&timezone=" + options.Timezone;
            qieru += "&hourly=temperature_2m,relativehumidity_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,snowfall,snow_depth,weathercode,pressure_msl,surface_pressure,cloudcover,cloudcover_low,cloudcover_mid,cloudcover_high,visibility,is_day";
            qieru += "&daily=weathercode,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min,sunrise,sunset,precipitation_sum,rain_sum,showers_sum,snowfall_sum,precipitation_hours,precipitation_probability_max,windspeed_10m_max,windgusts_10m_max,winddirection_10m_dominant";
            qieru += "&current_weather=" + options.Current_Weather;

            qieru += "&timeformat=" + options.Timeformat.ToString();

            qieru += "&past_days=" + options.Past_Days;


            if (options.Start_date != string.Empty)
                qieru += "&start_date=" + options.Start_date;
            if (options.End_date != string.Empty)
                qieru += "&end_date=" + options.End_date;

            // Now we iterate through hourly and daily

            // Hourly
            if (options.Hourly.Count > 0)
            {
                bool firstHourlyElement = true;
                qieru += "&hourly=";

                foreach (var option in options.Hourly)
                {
                    if (firstHourlyElement)
                    {
                        qieru += option.ToString();
                        firstHourlyElement = false;
                    }
                    else
                    {
                        qieru += "," + option.ToString();
                    }
                }
            }

            // Daily
            if (options.Daily.Count > 0)
            {
                bool firstDailyElement = true;
                qieru += "&daily=";
                foreach (var option in options.Daily)
                {
                    if (firstDailyElement)
                    {
                        qieru += option.ToString();
                        firstDailyElement = false;
                    }
                    else
                    {
                        qieru += "," + option.ToString();
                    }
                }
            }

            // 0.2.0 Weather models
            // cell_selection
            qieru += "&cell_selection=" + options.Cell_Selection;

            // Models
            if (options.Models.Count > 0)
            {
                bool firstModelsElement = true;
                qieru += "&models=";
                foreach (var option in options.Models)
                {
                    if (firstModelsElement)
                    {
                        qieru += option.ToString();
                        firstModelsElement = false;
                    }
                    else
                    {
                        qieru += "," + option.ToString();
                    }
                }
            }

            uri.Query = qieru;
            return uri.ToString();
        }

        /// <summary>
        /// Combines a given url with an options object to create a url for GET requests
        /// </summary>
        /// <returns>url+queryString</returns>
        private string MergeUrlWithOptions(string url, GeocodingOptions options)
        {
            string qieru = "";
            if (options == null) return url;

            UriBuilder uri = new UriBuilder(url);
            bool isFirstParam = false;

            // If no query given, add '' to start the query string
            if (qieru == string.Empty)
            {
                qieru = "";

                // isFirstParam becomes true because the query string is new
                isFirstParam = true;
            }

            // Now we check every property and set the value, if neccessary
          
            if (isFirstParam)
            {
                qieru += "name=" + options.Name;
            }
            else
            { qieru += "&name=" + options.Name; }
                

            if(options.Count >0)
                qieru += "&count=" + options.Count;
            
            if (options.Format != string.Empty)
                qieru += "&format=" + options.Format;

            if (options.Language != string.Empty)
                qieru += "&language=" + options.Language;

            uri.Query = qieru;
            return uri.ToString();
        }

        /// <summary>
        /// Combines a given url with an options object to create a url for GET requests
        /// </summary>
        /// <returns>url+queryString</returns>
        private string MergeUrlWithOptions(string url, AirQualityOptions options)
        {
            string qieru = "";
            if (options == null) return url;

            UriBuilder uri = new UriBuilder(url);
            bool isFirstParam = false;

            // If no query given, add '' to start the query string
            if (qieru == string.Empty)
            {
                qieru = "";

                // isFirstParam becomes true because the query string is new
                isFirstParam = true;
            }

            // Now we check every property and set the value, if neccessary
            if (isFirstParam)
                qieru += "latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);
            else
                qieru += "&latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);

            qieru += "&longitude=" + options.Longitude.ToString(CultureInfo.InvariantCulture);

            if (options.Domains != string.Empty)
                qieru += "&domains=" + options.Domains;

            if (options.Timeformat != string.Empty)
                qieru += "&timeformat=" + options.Timeformat;

            if (options.Timezone != string.Empty)
                qieru += "&timezone=" + options.Timezone;

            // Finally add hourly array
            if (options.Hourly.Count >= 0)
            {
                bool firstHourlyElement = true;
                qieru += "&hourly=" + "surface_pressure";

                foreach (var option in options.Hourly)
                {
                    if (firstHourlyElement)
                    {
                        qieru += option.ToString();
                        firstHourlyElement = false;
                    }
                    else
                    {
                        qieru += option.ToString();
                    }
                }
            }
            uri.Query = qieru;
            return uri.ToString();
        }
    }
}

