using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCoreSWebApp.Models
{
    public partial class StoreFile
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(255), Display(Name="filename")]
        public string Name { get; set; }

        [Required, StringLength(1024), Display(Name = "file path")]
        public string Path { get; set; }

        [Required]
        public bool IsDir { get; set; }

        // [filedate][nchar] (24) NOT NULL,
        [Required, StringLength(24)]
        public string Filedate { get; set; }

        //	[len] [bigint] NOT NULL,
        [Required]
        public long Len { get; set; }

        //	[del] [bit] NOT NULL
        // [Required]
        public bool Del { get; set; }

        public StoreFile()
        {
            Name = "";
            Path = "";
            Filedate = "";
        }
    }
}
    