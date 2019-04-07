using NASA.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NASA.Insfrastructures.Services
{
    /// <summary>
    /// Interface for Rover Service class
    /// </summary>
    public interface IRoverService
    {
        /// <summary>
        /// Retrieve images on specific date
        /// </summary>
        /// <param name="date">The date</param>
        /// <returns></returns>
        Task<IEnumerable<Image>> GetImages(DateTime date);

        /// <summary>
        /// Download image from NASA
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<Stream> DownloadImage(string url);
    }
}
