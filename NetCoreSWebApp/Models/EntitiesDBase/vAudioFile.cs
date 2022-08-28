using System.ComponentModel.DataAnnotations;

namespace NetCoreSWebApp.Models
{
    public class vAudioFile
    {
        //    SELECT dbo.StoreFiles.name, dbo.StoreFiles.path, dbo.StoreFiles.isDir, dbo.StoreFiles.filedate, dbo.StoreFiles.len, dbo.StoreFiles.del, dbo.audioParam.ID, dbo.audioParam.fileID, dbo.audioParam.artist,
        //                         dbo.audioParam.title, dbo.audioParam.album, dbo.audioParam.comment, dbo.audioParam.duration, dbo.audioParam.dekada, dbo.audioParam.bitrate, dbo.audioParam.channels, dbo.audioParam.sample, dbo.audioParam.vbr,
        //                         dbo.audioParam.year, dbo.audioParam.track
        //FROM            dbo.StoreFiles INNER JOIN
        //                         dbo.audioParam ON dbo.StoreFiles.ID = dbo.audioParam.fileID

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

        // audioParam
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

        public vAudioFile()
        {
            Name = "";
            Path = "";
            Filedate = "";

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
