﻿using NASA.Domain;
using System.Collections.Generic;

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
            PhotoGallery = new List<Image>();
            PhotoAlbum = new List<Album>();
        }

        /// <summary>
        /// List of images
        /// </summary>
        public List<Image> PhotoGallery { get; set; } 

        /// <summary>
        /// List of album
        /// </summary>
        public List<Album> PhotoAlbum { get; set; }
    }
}
