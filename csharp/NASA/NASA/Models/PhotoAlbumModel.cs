using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NASA.Models
{
    public class PhotoModel
    {
        public PhotoModel()
        {
            PhotoGallery = new List<Photo>();
            PhotoAlbum = new List<Album>();
        }
        public List<Photo> PhotoGallery { get; set; } 

        public List<Album> PhotoAlbum { get; set; }
    }
}
