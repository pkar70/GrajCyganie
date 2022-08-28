using System.ComponentModel.DataAnnotations;

namespace NetCoreSWebApp.Models
{
    public class vVideoFile
    {
        //        SELECT dbo.StoreFiles.ID, dbo.StoreFiles.name, dbo.StoreFiles.path, dbo.StoreFiles.isDir, dbo.StoreFiles.filedate, dbo.StoreFiles.len, dbo.StoreFiles.del, dbo.videoParam.ID AS Expr1, dbo.videoParam.fileID, dbo.videoParam.imageSize,
        //                         dbo.videoParam.duration, dbo.videoParam.rok, dbo.videoParam.mimeType, dbo.videoParam.video, dbo.videoParam.audio, dbo.videoParam.subtitle, dbo.videoParam.other
        //FROM            dbo.StoreFiles INNER JOIN
        //                         dbo.videoParam ON dbo.StoreFiles.ID = dbo.videoParam.fileID

        // StoreFile
        // public int Id { get; set; }

        [Required, StringLength(255), Display(Name = "filename")]
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


        // videoParam
        [Required, Display(Name = "ID")]
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



        public vVideoFile()
        {
            Name = "";
            Path = "";
            Filedate = "";

            ImageSize = "";
            MimeType = "";
            Video = "";
            Audio = "";
            Subtitle = "";
            Other = "";
        }
    }
}
