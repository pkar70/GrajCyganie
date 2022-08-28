using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace NetCoreSWebApp.Models
{
    public partial class AudioParam
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int FileId { get; set; }

        [Required, StringLength(255)]
        public string Artist { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; }

        [Required, StringLength(255)]
        public string Album { get; set; }

        [Required, StringLength(3900)]
        public string Comment { get; set; }

        [Required]
        public short Duration { get; set; }

        [Required, StringLength(8)]
        public string Dekada { get; set; }

        [Required]
        public int Bitrate { get; set; }

        [Required, StringLength(32)]
        public string Channels { get; set; }

        [Required]
        public int Sample { get; set; }

        [Required]
        public int Vbr { get; set; }

        [Required, StringLength(10)]
        public string Year { get; set; }

        [Required, StringLength(16)]
        public string Track { get; set; }

        public AudioParam()
        {
            Artist = "";
            Title = "";
            Album = "";
            Comment = "";
            Dekada = "";
            Channels = "";
            Year = "";
            Track = "";
        }
    }
}
