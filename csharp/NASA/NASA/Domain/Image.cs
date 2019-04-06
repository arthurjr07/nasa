namespace NASA.Domain
{
    /// <summary>
    /// Photo class
    /// </summary>
    public class Image
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
