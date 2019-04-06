using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hosting = Microsoft.AspNetCore.Hosting;

namespace NASA.BackgroundServices
{
    /// <summary>
    /// Background service that will download all images from NASA Mars Rover API
    /// </summary>
    public class ImageDownloaderService : BackgroundService
    {
        private readonly int _noOfMinutesToWait = 60;
        private readonly string _apiKey = "Oau7uZZxM7OcAeFFKqUpQovjDpHDN1xHRS3QZSGx";
        private readonly HttpClient _httpClient;
        private readonly Hosting.IHostingEnvironment _environment;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageDownloaderService(HttpClient httpClient,
            Hosting.IHostingEnvironment environment)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        /// <summary>
        /// Override method of IHostedService execute async
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var webRoot = _environment.WebRootPath;
            var imagesFolder = Path.Combine(webRoot, "images");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var dates = File.ReadAllLines("img_dates.txt");
                    foreach (var date in dates)
                    {
                        var earth_date = DateTime.Parse(date).ToString("yyyy-MM-dd");
                        var response = await _httpClient.GetAsync(
                            new Uri($"https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?earth_date={earth_date}&api_key={_apiKey}"))
                            .ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            dynamic photoListObject = JObject.Parse(stringResponse);
                            var photos = photoListObject.photos;
                            foreach (var photoObj in photos)
                            {
                                var id = ((dynamic)photoObj).id;
                                var url = ((dynamic)photoObj).img_src;
                                var imageStream = await _httpClient.GetStreamAsync(url.Value).ConfigureAwait(false);
                                var albumFolder = $"{imagesFolder}/{earth_date}";
                                if(!Directory.Exists(albumFolder))
                                {
                                    Directory.CreateDirectory(albumFolder);
                                }
                                var filename = $"{albumFolder}/{id.Value}.jpg";
                                using (var stream = new FileStream(filename, FileMode.Create))
                                {
                                    imageStream.CopyTo(stream);
                                }
                            }

                        }
                    }
                }
                catch(Exception e)
                {
                    //do nothing
                }
                await Task.Delay(TimeSpan.FromMinutes(_noOfMinutesToWait), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
