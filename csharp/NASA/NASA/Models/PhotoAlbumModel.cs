using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NASA.Models
{
    /// <summary>
    /// Photo Model for Home Controller
    /// </summary>
    public class PhotoModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PhotoModel()
        {
            PhotoGallery = new List<Photo>();
            PhotoAlbum = new List<Album>();
        }

        /// <summary>
        /// List of images
        /// </summary>
        public List<Photo> PhotoGallery { get; set; } 

        /// <summary>
        /// List of album
        /// </summary>
        public List<Album> PhotoAlbum { get; set; }
    }
}
