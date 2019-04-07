using NASA.Domain;
using NASA.Domain.RoverEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NASA.Insfrastructures.Services
{
    /// <summary>
    /// Rover Service class for accessing the NASA API
    /// </summary>
    public class RoverService : IRoverService 
    {

        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "Oau7uZZxM7OcAeFFKqUpQovjDpHDN1xHRS3QZSGx";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_httpClient"></param>
        public RoverService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Download the image on the specified url.
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns></returns>
        public async Task<Stream> DownloadImage(string url)
        {
            var imageStream = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            return imageStream;
        }

        /// <summary>
        /// Retrieve all the images on the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Image>> GetImages(DateTime date)
        {
            if(date > DateTime.Today )
            {
                throw new ApplicationException("Date should not be greater than date today");
            }

            var images = new List<Image>();
            var earth_date = date.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync(
                            new Uri($"https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?earth_date={earth_date}&api_key={_apiKey}"))
                            .ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rootObject = JsonConvert.DeserializeObject<RootObject>(stringResponse);
                var photos = rootObject.photos;
                foreach (var photo in photos)
                {
                    var id = photo.id.ToString();
                    var url = photo.img_src;
                    var image = new Image() { Caption = id, URL = url };
                    images.Add(image);
                }
            }

            return images;
        }
    }
}
