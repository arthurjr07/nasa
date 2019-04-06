using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NASA.Models
{
    /// <summary>
    /// Photo class
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Caption that will be displayed when image is not available
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Url of the image
        /// </summary>
        public string URL { get; set; }
    }
}
