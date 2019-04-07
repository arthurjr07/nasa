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
using Microsoft.Extensions.Logging;
using NASA.Domain;
using NASA.Domain.RoverEntity;
using System.IO.Abstractions;

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
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageDownloaderService(HttpClient httpClient,
            Hosting.IHostingEnvironment environment,
            ILogger<ImageDownloaderService> logger,
            IFileSystem fileSystem)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <summary>
        /// Override method of IHostedService execute async
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var webRoot = _environment.WebRootPath;
            var imagesFolder = _fileSystem.Path.Combine(webRoot, Constants.ImageDirectory);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var dates = _fileSystem.File.ReadAllLines("img_dates.txt");
                    foreach (var date in dates)
                    {
                        var validDate = DateTime.Today;
                        var isValidDate = DateTime.TryParse(date, out validDate);
                        if(!isValidDate)
                        {
                            _logger.LogWarning("Invalid date found in the text file", new[] { date });
                            continue;
                        }

                        var earth_date = validDate.ToString("yyyy-MM-dd");
                        var albumFolder = $"{imagesFolder}/{earth_date}";
                        if (_fileSystem.Directory.Exists(albumFolder))
                        {
                            _logger.LogInformation($"Images for {earth_date} are already downloaded");
                            continue;
                        }

                        _fileSystem.Directory.CreateDirectory(albumFolder);

                        //TODO: Move this functionality to Rover Service class
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
                                var imageStream = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
                                
                                var filename = $"{albumFolder}/{id}.jpg";
                                using (var stream = _fileSystem.FileStream.Create(filename, FileMode.Create))
                                {
                                    imageStream.CopyTo(stream);
                                }
                            }

                        }
                    }
                }
                catch(Exception e)
                {
                    _logger.LogError(e.Message);
                }
                await Task.Delay(TimeSpan.FromMinutes(_noOfMinutesToWait), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
