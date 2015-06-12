using System.Collections.Generic;
using ImageMagick;
using System.Web.Hosting;
using System.Linq;

namespace Hearts4Kids.Services
{
    public static class PhotoServices
    {
        public const string bioDir = "~/Content/Photos/Bios";
        public const int quality = 95;
        public const int maxHeight = 696;
        public const int thumbHeight = 76;
        public const string defaultDir = "~/Content/Photos/Slides";
        internal class SiteImageCompression
        {
            public SiteImageCompression(string folderName, int height, int width = 0)
            {
                FolderName = folderName;
                Geometry = new MagickGeometry(width, height);
                //Geometry.FillArea = true;
                //Geometry.IgnoreAspectRatio = false;
            }
            public string FolderName { get; private set; }
            public MagickGeometry Geometry { get; private set; }
        }
        internal static SiteImageCompression[] Compressions = new SiteImageCompression[]
        {
            new SiteImageCompression ( "lg", maxHeight), //936
            new SiteImageCompression ( "pv", 232), //312
            new SiteImageCompression ( "th", thumbHeight) //104
        };
        public static string GetDefaultDir()
        {
            return System.IO.Path.Combine(HostingEnvironment.MapPath(defaultDir), Compressions[0].FolderName);
        }
        public static void processImage(string imageName, string dir= defaultDir)
        {
            // This will resize the image to a fixed size without maintaining the aspect ratio.
            // Normally an image will be resized to fit inside the specified size.
            string basePath = HostingEnvironment.MapPath(dir);
            string path = System.IO.Path.Combine(basePath, Compressions[0].FolderName,imageName);
            using (var m = new MagickImage(path))
            {
                foreach(var c in Compressions.Skip(1))
                {
                    MagickGeometry size = new MagickGeometry(100, 100);
                    size.IgnoreAspectRatio = true;
                    m.Resize(100,100);//c.Geometry);
                    path = System.IO.Path.Combine(basePath, c.FolderName, imageName);
                    m.Write(path);
                }
            }

        }
        public static void makeBanner(IEnumerable<string> imageNames)
        {
            using (MagickImageCollection images = new MagickImageCollection())
            {

                foreach (string fn in imageNames)
                {
                    MagickImage i = new MagickImage(fn);
                    images.Add(i);
                }

                // Create a mosaic from all images
                using (MagickImage result = images.Mosaic())
                {
                    // Save the result
                    result.Write("Mosaic.png");
                }
            }
        }
    }
}