using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DoubleA.Models
{
    public class Anime
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Score { get; set; }
        public string EpisodeCount { get; set; }
        public string StartEndDate { get; set; }

        public static Anime CreateFromMALJsonElement(JsonElement node)
        {
            Anime toReturn = new Anime();
            toReturn.Id = node.GetProperty("id").GetInt32();
            toReturn.Title = node.GetProperty("title").GetString();
            toReturn.Score = node.GetProperty("mean").GetDouble().ToString() + "/10";

            StringBuilder animeStringBuilder = new StringBuilder();
            animeStringBuilder.Append(node.GetProperty("media_type").GetString()).Append(", ");
            int episodeNumber = node.GetProperty("num_episodes").GetInt32();
            animeStringBuilder.Append(episodeNumber).Append(episodeNumber != 1 ? " episodes" : " episode");
            toReturn.EpisodeCount = animeStringBuilder.ToString();

            animeStringBuilder.Length = 0;
            animeStringBuilder.Append(node.GetProperty("start_date").GetString());
            animeStringBuilder.Append(" - ").Append(node.GetProperty("end_date").GetString());
            toReturn.StartEndDate = animeStringBuilder.ToString();

            return toReturn;
        }
    }

    public class AnimeDetailed : Anime
    {
        public string Description { get; set; }
        public string EpisodeDuration { get; set; }
        public string AdaptationSource { get; set; }
        public string Genres { get; set; }
    }
}
