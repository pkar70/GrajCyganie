using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCoreSWebApp.Models
{
    public partial class VideoParam
    {
        [Required,Display(Name ="ID")]
        public int Id { get; set; }

        [Required, Display(Name = "FileId")]
        public int FileId { get; set; }

        [Required, StringLength(12), Display(Name = "ImageSize")]
        public string ImageSize { get; set; }

        [Required, Display(Name = "Duration")]
        public int Duration { get; set; }

        [Required, Display(Name = "Rok")]
        public int Rok { get; set; }

        [Required, StringLength(24), Display(Name = "MimeType")]
        public string MimeType { get; set; }

        [Required, StringLength(1024), Display(Name = "Video")]
        public string Video { get; set; }

        [Required, StringLength(1024), Display(Name = "Audio")]
        public string Audio { get; set; }

        [Required, StringLength(1024), Display(Name = "Subtitle")]
        public string Subtitle { get; set; } 

        [Required, StringLength(1024), Display(Name = "Other")]
        public string Other { get; set; } 

        public VideoParam()
        {
            ImageSize = "";
            MimeType = "";
            Video = "";
            Audio = "";
            Subtitle = "";
            Other = "";
        }
    }
}
