using System.ComponentModel.DataAnnotations;

namespace Hearts4Kids.Models
{
    public class BiosViewModel
    {
        public string Name { get; set; }
        [StringLength(128)]
        public string CitationDescription { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(4000)]
        public string Biography { get; set; }
        [StringLength(256)]
        public string BioPicUrl { get; set; }
    }
}