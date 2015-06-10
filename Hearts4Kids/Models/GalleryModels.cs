using Hearts4Kids.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hearts4Kids.Models
{
    public class GalleryModels
    {
        [PostedFileSize(10000000)]
        [PostedFileTypes(PhotoServices.AcceptedFileTypes)]
        public HttpPostedFileWrapper File { get; set; }
    }
}