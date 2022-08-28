using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// w szkoleniu jest Startup.cs
// https://github.com/OdeToCode/OdeToFood/blob/master/OdeToFood/OdeToFood/Startup.cs
//  services.AddScoped<IRestaurantData, SqlRestaurantData>();

//             services.AddDbContextPool<OdeToFoodDbContext>(options =>
//{
//    options.UseSqlServer(Configuration.GetConnectionString("OdeToFoodDb"));
//});


// z package command line
// https://docs.microsoft.com/en-us/ef/core/cli/powershell
// Scaffold-DbContext "Server=.;Database=PKARweb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model


namespace NetCoreSWebApp.Models
{
    public partial class LocalSQLContext : DbContext
    {
        public LocalSQLContext()
        {
        }

        public LocalSQLContext(DbContextOptions<LocalSQLContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActorFilm> ActorFilms { get; set; } = null!;
        public virtual DbSet<ActorName> ActorNames { get; set; } = null!;
        public virtual DbSet<AudioParam> AudioParams { get; set; } = null!;
        public virtual DbSet<PicParam> PicParams { get; set; } = null!;
        public virtual DbSet<StoreFile> StoreFiles { get; set; } = null!;
        public virtual DbSet<VideoParam> VideoParams { get; set; } = null!;
        public virtual DbSet<vAktorzyFilmu> vAktorzyFilmus { get; set; } = null!;
        public virtual DbSet<vAudioFile> vAudioFiles { get; set; } = null!;
        public virtual DbSet<vVideoFile> vVideoFiles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(CONN_STRING);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Polish_CI_AI");

            modelBuilder.Entity<ActorFilm>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("actorFilm");

                entity.Property(e => e.ActorId)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("actorId")
                    .IsFixedLength();

                entity.Property(e => e.FilmId)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("filmId")
                    .IsFixedLength();

                entity.Property(e => e.Postac)
                    .HasMaxLength(256)
                    .HasColumnName("postac");
            });

            modelBuilder.Entity<ActorName>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("actorNames");

                entity.Property(e => e.Id)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("id")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<AudioParam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("audioParam");

                entity.Property(e => e.Album)
                    .HasMaxLength(255)
                    .HasColumnName("album");

                entity.Property(e => e.Artist)
                    .HasMaxLength(255)
                    .HasColumnName("artist");

                entity.Property(e => e.Bitrate).HasColumnName("bitrate");

                entity.Property(e => e.Channels)
                    .HasMaxLength(32)
                    .HasColumnName("channels")
                    .IsFixedLength();

                entity.Property(e => e.Comment)
                    .HasMaxLength(3900)
                    .HasColumnName("comment");

                entity.Property(e => e.Dekada)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dekada")
                    .IsFixedLength();

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.FileId).HasColumnName("fileID");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Sample).HasColumnName("sample");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.Track)
                    .HasMaxLength(16)
                    .HasColumnName("track")
                    .IsFixedLength();

                entity.Property(e => e.Vbr).HasColumnName("vbr");

                entity.Property(e => e.Year)
                    .HasMaxLength(10)
                    .HasColumnName("year")
                    .IsFixedLength();
            });

            modelBuilder.Entity<PicParam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("picParam");

                entity.Property(e => e.AllData).HasColumnName("allData");

                entity.Property(e => e.FileId).HasColumnName("fileID");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.MimeType)
                    .HasMaxLength(64)
                    .HasColumnName("mimeType");

                entity.Property(e => e.Width).HasColumnName("width");
            });

            modelBuilder.Entity<StoreFile>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Del).HasColumnName("del");

                entity.Property(e => e.Filedate)
                    .HasMaxLength(24)
                    .HasColumnName("filedate")
                    .IsFixedLength();

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.IsDir).HasColumnName("isDir");

                entity.Property(e => e.Len).HasColumnName("len");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Path)
                    .HasMaxLength(1024)
                    .HasColumnName("path");
            });

            modelBuilder.Entity<VideoParam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("videoParam");

                entity.Property(e => e.Audio)
                    .HasMaxLength(1024)
                    .HasColumnName("audio");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.FileId).HasColumnName("fileID");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.ImageSize)
                    .HasMaxLength(12)
                    .HasColumnName("imageSize")
                    .IsFixedLength();

                entity.Property(e => e.MimeType)
                    .HasMaxLength(24)
                    .HasColumnName("mimeType")
                    .IsFixedLength();

                entity.Property(e => e.Other)
                    .HasMaxLength(1024)
                    .HasColumnName("other");

                entity.Property(e => e.Rok).HasColumnName("rok");

                entity.Property(e => e.Subtitle)
                    .HasMaxLength(1024)
                    .HasColumnName("subtitle");

                entity.Property(e => e.Video)
                    .HasMaxLength(1024)
                    .HasColumnName("video");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
