namespace MusicHub
{
    using System;

    using Data;
    using Initializer;
    using MusicHub.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);
            Console.WriteLine(ExportSongsAboveDuration(context, 4));

            //Test your solutions here
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Producers
                .Include(x => x.Albums)
                .ThenInclude(x => x.Songs)
                .ThenInclude(x => x.Writer)
                .First(x => x.Id == producerId)
                .Albums.Select(a => new
                {
                    albumName = a.Name,
                    a.ReleaseDate,
                    producerName = a.Producer.Name,
                    albumSongs = a.Songs.Select(s => new
                    {
                        s.Name,
                        s.Price,
                        writerName = s.Writer.Name,
                    }).OrderByDescending(s => s.Name)
                       .ThenBy(s => s.writerName),
                    totalAlbumPrice = a.Price
                }).OrderByDescending(x => x.totalAlbumPrice)
                .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.albumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyy")}");
                sb.AppendLine($"-ProducerName: {album.producerName}");
                sb.AppendLine("-Songs:");
                if (album.albumSongs.Any())
                {
                    int counter = 1;
                    foreach (var item in album.albumSongs)
                    {
                        sb.AppendLine($"---#{counter++}");
                        sb.AppendLine($"---SongName: {item.Name}");
                        sb.AppendLine($"---Price: {item.Price:f2}");
                        sb.AppendLine($"---Writer: {item.writerName}");
                    }

                }

                sb.AppendLine($"-AlbumPrice: {album.totalAlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();

        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Include(x => x.SongPerformers)
                .ThenInclude(x => x.Performer)
                .Include(x => x.Writer)
                .Include(x => x.Album)
                .ThenInclude(x => x.Producer)
                .ToList()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                {
                    x.Name,
                    writerName = x.Writer.Name,
                    performer = x.SongPerformers
                    .Select(x => x.Performer.FirstName + " " + x.Performer.LastName).ToList().OrderBy(x => x),
                    albumProducerName = x.Album.Producer.Name,
                    duration = x.Duration.ToString("c")



                }).OrderBy(x => x.Name)
                .ThenBy(x => x.writerName)
                .ToList();
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{counter++}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.writerName}");
                //if (song.performer.Any())
                //{
                //    sb.AppendLine($"---Performer: {string.Join(", ", song.performer)}");
                //}
                foreach (var item in song .performer)
                {
                    sb.AppendLine ($"---Performer: {item}");
                }

                sb.AppendLine($"---AlbumProducer: {song.albumProducerName}");
                sb.AppendLine($"---Duration: {song.duration}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
