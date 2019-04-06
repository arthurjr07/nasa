using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NASA.Models;

namespace NASA.Controllers
{
    /// <summary>
    /// Home Controller class
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _environment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="environment"></param>
        public HomeController(IHostingEnvironment environment,
            ILogger<HomeController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        /// <summary>
        /// Home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                var webRoot = _environment.WebRootPath;
                var imagesFolder = Path.Combine(webRoot, "images");
                var directories = Directory.GetDirectories(imagesFolder);

                var albums = new List<Album>();
                foreach (string directory in directories)
                {
                    var folderName = Path.GetFileName(directory);
                    var url = $"./{folderName}";
                    var album = new Album() { Name = folderName, URL = url };
                    albums.Add(album);
                }

                var model = new PhotoModel();
                model.PhotoAlbum = albums;
                return View(model);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return View(new PhotoModel());
            }
        }

        /// <summary>
        /// Display images on the album
        /// </summary>
        /// <param name="id">The album name</param>
        /// <returns></returns>
        [Route("{id}")]
        public IActionResult Index(string id)
        {
            try
            {
                var webRoot = _environment.WebRootPath;
                var imagesFolder = Path.Combine(webRoot, "images", id);
                var files = Directory.GetFiles(imagesFolder);

                var photos = new List<Photo>();
                foreach (string file in files)
                {
                    var caption = Path.GetFileNameWithoutExtension(file);
                    var url = $"./images/{id}/{Path.GetFileName(file)}";
                    var photo = new Photo() { Caption = caption, URL = url };
                    photos.Add(photo);
                }

                var model = new PhotoModel();
                model.PhotoGallery = photos;
                return View(model);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return View(new PhotoModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
