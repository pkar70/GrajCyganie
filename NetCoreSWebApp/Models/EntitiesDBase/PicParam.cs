using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCoreSWebApp.Models
{
    public partial class PicParam
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int FileId { get; set; }

        [Required]
        public int Width { get; set; }

        [Required]
        public int Height { get; set; }

        public string? AllData { get; set; }

        [StringLength(64)]
        public string? MimeType { get; set; }
    }
}
