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
using NASA.Insfrastructures.Services;

namespace NASA.BackgroundServices
{
    /// <summary>
    /// Background service that will download all images from NASA Mars Rover API
    /// </summary>
    public class ImageDownloaderService : BackgroundService
    {
        private readonly int _noOfMinutesToWait = 60;
        private readonly string _dateConfigFile = "img_dates.txt";
        private readonly Hosting.IHostingEnvironment _environment;

        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IRoverService _roverService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageDownloaderService(IRoverService roverService,
            Hosting.IHostingEnvironment environment,
            ILogger<ImageDownloaderService> logger,
            IFileSystem fileSystem)
        {
            _roverService = roverService ?? throw new ArgumentNullException(nameof(roverService));
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
                    if(!_fileSystem.File.Exists(_dateConfigFile))
                    {
                        _logger.LogWarning("File does not exists.", new[] { _dateConfigFile });
                        continue;
                    }

                    var dates = _fileSystem.File.ReadAllLines(_dateConfigFile);
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

                        var images = await _roverService.GetImages(validDate).ConfigureAwait(false);
                        foreach(var image in images)
                        {
                            var imageStream = await _roverService.DownloadImage(image.URL).ConfigureAwait(false);
                            var filename = $"{albumFolder}/{image.Caption}.jpg";
                            using (var stream = _fileSystem.FileStream.Create(filename, FileMode.Create))
                            {
                                imageStream.CopyTo(stream);
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
