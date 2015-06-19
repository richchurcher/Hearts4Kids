using System;
using System.IO;

namespace Hearts4Kids.Models
{
    public class GalleryModel
    {
        public string thumbnailUrl { get; set; }
        public string url { get; set; }
    }

    public class AdminGalleryModel : GalleryModel
    {
        public string name { get;set; }
        public long size { get; set; }

        public string fileSize
        {
            get
            {
                if (size < 1000)
                {
                    return size.ToString() + 'B';
                }
                if (size < 1000000)
                {
                    return (size/1000).ToString() + "KB";
                }
                return (size / 1000000).ToString() + "MB";
            }
        }
        /*
        public string type { get; set; }
        public string deleteUrl { get; set; }
        public string deleteType { get { return "DELETE"; } }
        */
    }

}