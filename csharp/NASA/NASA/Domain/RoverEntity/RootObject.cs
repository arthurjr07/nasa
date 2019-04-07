using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NASA.Domain.RoverEntity
{
    /// <summary>
    /// Rover root object class
    /// </summary>
    public class RootObject
    {
        /// <summary>
        /// List of Photo objects
        /// </summary>
        public List<Photo> photos { get; set; }
    }
}
