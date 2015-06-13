using System.Collections.Generic;
using System.Web.Hosting;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using Hearts4Kids.Models;
using System.IO;
using System.Web;
using System.Threading;

namespace Hearts4Kids.Services
{
    public static class PhotoServices
    {
        public const string bioDir = "~/Content/Photos/Bios";
        public const int maxHeight = 696;
        public const int thumbHeight = 76;
        public const int bannerHeight = 232;
        public const long defaultQuality = 95;
        //screen * md-7 = 7/12
        public const int maxBannerWidth = 1600 * 7 / 12;
        public const string defaultDir = "~/Content/Photos/Slides";
        public const string bannerPath = "~/Content/Photos/Banner.jpg";
        internal class SiteImageSize
        {
            public SiteImageSize(string folderName, int height)
            {
                FolderName = folderName;
                Height = height;
                Quality = defaultQuality;
            }
            public string FolderName { get; private set; }
            public int Height { get; private set; }
            public ImageFormat ImgFmt { get; set; }
            public long Quality { get; set; }
            public string GetNewFileName(string originalFileName)
            {
                string newExt;
                if (ImgFmt == ImageFormat.Bmp)
                {
                    newExt = "bmp";
                }
                else if (ImgFmt == ImageFormat.Jpeg)
                {
                    newExt = "jpg";
                }
                else if (ImgFmt == ImageFormat.Png)
                {
                    newExt = "png";
                }
                else if (ImgFmt == ImageFormat.Tiff)
                {
                    newExt = "tiff";
                }
                else if (ImgFmt == ImageFormat.Icon)
                {
                    newExt = "icon";
                }
                else
                {
                    throw new Exception("unsupported type");
                }
                return Path.ChangeExtension(originalFileName, newExt);
            }
        }
        internal static SiteImageSize[] ImageSizes = new SiteImageSize[]
        {
            new SiteImageSize ( "lg", maxHeight) { ImgFmt=ImageFormat.Png }, //936
            new SiteImageSize ( "pv", bannerHeight) { ImgFmt=ImageFormat.Png }, //312
            new SiteImageSize ( "th", thumbHeight) { ImgFmt=ImageFormat.Png } //104
        };
        public static string GetDefaultDir()
        {
            return Path.Combine(HostingEnvironment.MapPath(defaultDir), ImageSizes[0].FolderName);
        }
        public static string processBioImage(HttpPostedFileBase file)
        {
            Thread reduceFurtherSizes = new Thread(processBioImg);
            reduceFurtherSizes.Start(file);
            var returnSize = ImageSizes[1];
            return defaultDir + '/' + returnSize.FolderName + '/' + GetBioFileName(returnSize.GetNewFileName(file.FileName));
        }
        static string GetBioFileName(string fileName)
        {
            return "bio_" + fileName;
        }
        static void processBioImg(object fileBase)
        {
            var file = (HttpPostedFileBase)fileBase;
            var newSize = ImageSizes[0];
            string basePath = HostingEnvironment.MapPath(defaultDir);
            string newFileName = GetBioFileName(newSize.GetNewFileName(file.FileName));
            string path = Path.Combine(basePath, newSize.FolderName, newFileName);
            using (var srcImage = Image.FromStream(file.InputStream))
            {
                Resize(srcImage, path, newSize.Height, newSize.ImgFmt, newSize.Quality);
            }
            processImage(newFileName);
        }
        /// <summary>
        /// makes various sizes, added to appropriate folders, and returns the full path to the thumbnail
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dir"></param>
        public static string processImage(string imageName, string dir= defaultDir)
        {
            string basePath = HostingEnvironment.MapPath(dir);
            string path = Path.Combine(basePath, ImageSizes[0].FolderName,imageName);
            SiteImageSize s=null;
            string newFileName = null;
            for(int i=1;i < ImageSizes.Length;i++)
            {
                s = ImageSizes[i];
                newFileName = s.GetNewFileName(imageName);
                Resize(path, Path.Combine(basePath, s.FolderName, newFileName), s.Height, s.ImgFmt, s.Quality);
            }
            return defaultDir + '/' + s.FolderName + '/' + newFileName;
        }
        public static IEnumerable<GalleryModel> GetImages()
        {
            string[] imgExt = new string[] { ".jpg", ".jpeg", ".png", ".bmp" };
            string baseUr = defaultDir.Substring(1) + '/';//hack
            string fullUr = baseUr + ImageSizes[0].FolderName +'/';
            string thumbUr = baseUr + ImageSizes[ImageSizes.Length - 1].FolderName +'/';
            return (from p in Directory.EnumerateFiles(Path.Combine(HostingEnvironment.MapPath(defaultDir), ImageSizes[0].FolderName),"*.*",SearchOption.TopDirectoryOnly)
                    where imgExt.Any(e=>p.EndsWith(e, StringComparison.InvariantCultureIgnoreCase))
                    let fn = Path.GetFileName(p)
                    select new GalleryModel
                    {
                        FullsizeUri = fullUr + fn,
                        ThumbUrl = thumbUr + fn
                    });
        }
        public static void Resize(string imageFile, string outputFile, int newHeight, ImageFormat fmt = null, long quality = defaultQuality)
        {
            using (var srcImage = Image.FromFile(imageFile))
            {
                Resize(srcImage, outputFile, newHeight, fmt, quality);
            }
        }
        public static void Resize(Image srcImage, string outputFile, int newHeight, ImageFormat fmt = null, long quality= defaultQuality)
        {
            int newWidth = newHeight * srcImage.Width / srcImage.Height;
            using (var newImage = new Bitmap(newWidth, newHeight))
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                if (fmt == null) { fmt = srcImage.RawFormat; }
                if (fmt != ImageFormat.Jpeg)
                {
                    newImage.Save(outputFile, fmt);
                }
                else
                {
                    // https://msdn.microsoft.com/en-us/library/bb882583%28v=vs.110%29.aspx
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID 
                    // for the Quality parameter category.
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object. 
                    // An EncoderParameters object has an array of EncoderParameter 
                    // objects. In this case, there is only one 
                    // EncoderParameter object in the array.
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    newImage.Save(outputFile, jpgEncoder, myEncoderParameters);
                }
                
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public static void DeleteImages(string fileName, string baseDir = defaultDir)
        {
            string dir = HostingEnvironment.MapPath(baseDir);
            fileName = fileName.Substring(fileName.LastIndexOf('/')+1);
            foreach(var c in ImageSizes)
            {
                string pathName = Path.Combine(dir, c.FolderName, fileName);
                try
                {
                    File.Delete(pathName);
                }
                catch (IOException) { }
            }
        }
        public static void makeBanner(IEnumerable<string> imageNames)
        {
            string dir = Path.Combine(HostingEnvironment.MapPath(defaultDir), ImageSizes[1].FolderName);
            IList<string> fns = imageNames.Select(i => dir + '\\' + i.Substring(i.LastIndexOf('/') + 1)).ToList();
            using (var bm = CombineBitmap(fns))
            {
                bm.Save(HostingEnvironment.MapPath(bannerPath), ImageFormat.Jpeg);
                //Resize(bm,bannerPath, bannerHeight,ImageFormat.Jpeg);
            }
                
        }
        public static System.Drawing.Bitmap CombineBitmap(IEnumerable<string> files)
        {
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }
                int imagesSupplied = images.Count;
                if (imagesSupplied == 0) { return null; }
                int minFinalWidth = width + maxBannerWidth;
                int i = 0;

                do
                {
                    images.Add(images[i]);
                    width += images[i].Width;
                    if (++i >= imagesSupplied) { i = 0; }
                } while (width < minFinalWidth);

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                throw;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }
    }
}