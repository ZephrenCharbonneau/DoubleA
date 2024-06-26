﻿using System;
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

            bool scoreExists = node.TryGetProperty("mean", out JsonElement score);
            toReturn.Score = scoreExists ? score.GetDouble().ToString() + "/10" : "-/10";

            StringBuilder animeStringBuilder = new StringBuilder();
            animeStringBuilder.Append(node.GetProperty("media_type").GetString()).Append(", ");
            int episodeNumber = node.GetProperty("num_episodes").GetInt32();
            animeStringBuilder.Append(episodeNumber).Append(episodeNumber != 1 ? " episodes" : " episode");
            toReturn.EpisodeCount = animeStringBuilder.ToString();

            animeStringBuilder.Length = 0;
            animeStringBuilder.Append(node.GetProperty("start_date").GetString()).Append(" - ");
            bool endDateExists = node.TryGetProperty("end_date", out JsonElement endDate);

            if (endDateExists)
                animeStringBuilder.Append(endDate.GetString());
            toReturn.StartEndDate = animeStringBuilder.ToString();

            return toReturn;
        }

        protected static readonly String[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public static Anime CreateFromAnilistJsonElement(JsonElement node)
        {
            Anime toReturn = new Anime();
            toReturn.Id = node.GetProperty("id").GetInt32();
            toReturn.Title = node.GetProperty("title").GetProperty("romaji").GetString();

            JsonElement meanScore = node.GetProperty("meanScore");

            if (meanScore.ValueKind != JsonValueKind.Null)
                toReturn.Score = node.GetProperty("meanScore").GetInt32().ToString() + "/100";
            else
                toReturn.Score = "";

            StringBuilder animeStringBuilder = new StringBuilder();
            animeStringBuilder.Append(node.GetProperty("format").GetString());
            JsonElement episodes = node.GetProperty("episodes");

            if (episodes.ValueKind != JsonValueKind.Null)
            {
                int episodeNumber = episodes.GetInt32();
                animeStringBuilder.Append(", ").Append(episodeNumber).Append(episodeNumber != 1 ? " episodes" : " episode");
            }

            toReturn.EpisodeCount = animeStringBuilder.ToString();

            animeStringBuilder.Length = 0;
            JsonElement dateComponent;

            dateComponent = node.GetProperty("startDate").GetProperty("month");
            if (dateComponent.ValueKind != JsonValueKind.Null) 
                animeStringBuilder.Append(months[dateComponent.GetInt32() - 1]);

            dateComponent = node.GetProperty("startDate").GetProperty("year");
            if (dateComponent.ValueKind != JsonValueKind.Null)
                animeStringBuilder.Append(" ").Append(dateComponent.GetInt32());

            dateComponent = node.GetProperty("endDate").GetProperty("month");
            if (dateComponent.ValueKind != JsonValueKind.Null)
                animeStringBuilder.Append(" - ").Append(months[dateComponent.GetInt32() - 1]);

            dateComponent = node.GetProperty("endDate").GetProperty("year");
            if (dateComponent.ValueKind != JsonValueKind.Null)
                animeStringBuilder.Append(" ").Append(dateComponent.GetInt32());

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

        public static AnimeDetailed CreateFromMALJsonElement(JsonElement node)
        {
            AnimeDetailed toReturn = new AnimeDetailed();
            toReturn.Id = node.GetProperty("id").GetInt32();
            toReturn.Title = node.GetProperty("title").GetString();

            bool scoreExists = node.TryGetProperty("mean", out JsonElement score);
            toReturn.Score = scoreExists ? score.GetDouble().ToString() + "/10" : "-/10";

            StringBuilder animeStringBuilder = new StringBuilder();
            animeStringBuilder.Append(node.GetProperty("media_type").GetString()).Append(", ");
            int episodeNumber = node.GetProperty("num_episodes").GetInt32();
            animeStringBuilder.Append(episodeNumber).Append(episodeNumber != 1 ? " episodes" : " episode");
            toReturn.EpisodeCount = animeStringBuilder.ToString();

            animeStringBuilder.Length = 0;
            animeStringBuilder.Append(node.GetProperty("start_date").GetString()).Append(" - ");
            bool endDateExists = node.TryGetProperty("end_date", out JsonElement endDate);

            if (endDateExists)
                animeStringBuilder.Append(endDate.GetString());
            toReturn.StartEndDate = animeStringBuilder.ToString();

            toReturn.Description = node.GetProperty("synopsis").GetString();
            toReturn.AdaptationSource = node.GetProperty("source").GetString();

            animeStringBuilder.Length = 0;
            foreach (JsonElement genre in node.GetProperty("genres").EnumerateArray())
                animeStringBuilder.Append(genre.GetProperty("name").GetString()).Append(", ");
            animeStringBuilder.Length -= 2;
            toReturn.Genres = animeStringBuilder.ToString();

            JsonElement duration = node.GetProperty("average_episode_duration");
            if (duration.ValueKind != JsonValueKind.Null)
                toReturn.EpisodeDuration = (duration.GetInt32() / 60).ToString() + " minutes";

            return toReturn;
        }

        public static AnimeDetailed CreateFromAnilistJsonElement(JsonElement node)
        {
            AnimeDetailed toReturn = new AnimeDetailed();
            toReturn.Id = node.GetProperty("id").GetInt32();
            toReturn.Title = node.GetProperty("title").GetProperty("romaji").GetString();

            JsonElement meanScore = node.GetProperty("meanScore");

            if (meanScore.ValueKind != JsonValueKind.Null)
                toReturn.Score = node.GetProperty("meanScore").GetInt32().ToString() + "/100";
            else
                toReturn.Score = "";

            StringBuilder animeStringBuilder = new StringBuilder();
            animeStringBuilder.Append(node.GetProperty("format").GetString());
            JsonElement episodes = node.GetProperty("episodes");

            if (episodes.ValueKind != JsonValueKind.Null)
            {
                int episodeNumber = episodes.GetInt32();
                animeStringBuilder.Append(", ").Append(episodeNumber).Append(episodeNumber != 1 ? " episodes" : " episode");
            }

            toReturn.EpisodeCount = animeStringBuilder.ToString();

            animeStringBuilder.Length = 0;
            JsonElement dateComponent;

            dateComponent = node.GetProperty("startDate").GetProperty("month");
            if (dateComponent.ValueKind != JsonValueKind.Null)
                animeStringBuilder.Append(months[dateComponent.GetInt32() - 1]);

            dateComponent = node.GetProperty("startDate").GetProperty("year");
            if (dateComponent.ValueKind != JsonValueKind.Null)
                animeStringBuilder.Append(" ").Append(dateComponent.GetInt32());

            dateComponent = node.GetProperty("endDate").GetProperty("month");
            if (dateComponent.ValueKind != JsonValueKind.Null)
                animeStringBuilder.Append(" - ").Append(months[dateComponent.GetInt32() - 1]);

            dateComponent = node.GetProperty("endDate").GetProperty("year");
            if (dateComponent.ValueKind != JsonValueKind.Null)
                animeStringBuilder.Append(" ").Append(dateComponent.GetInt32());

            toReturn.StartEndDate = animeStringBuilder.ToString();

            JsonElement description = node.GetProperty("description");
            if (description.ValueKind != JsonValueKind.Null)
                toReturn.Description = description.ToString();

            JsonElement duration = node.GetProperty("duration");
            if (duration.ValueKind != JsonValueKind.Null)
                toReturn.EpisodeDuration = duration.GetInt32().ToString() + " minutes";

            JsonElement source = node.GetProperty("source");
            if (source.ValueKind != JsonValueKind.Null)
                toReturn.AdaptationSource = source.GetString();

            animeStringBuilder.Length = 0;
            JsonElement genres = node.GetProperty("genres");
            if (genres.ValueKind != JsonValueKind.Null)
            {
                foreach (JsonElement genre in genres.EnumerateArray())
                    animeStringBuilder.Append(genre.GetString()).Append(", ");
                animeStringBuilder.Length -= 2;
                toReturn.Genres = animeStringBuilder.ToString();
            }

            return toReturn;
        }
    }

    public class AnimeListEntry
    {
        public int Id { get; set; }
        public int IdMal { get; set; }
        public string Title { get; set; }
        public int EpisodesSeen { get; set; }
        public int NumberOfEpisodes { get; set; }
        public string Status { get; set; }

        public static AnimeListEntry CreateFromMALJsonElement(JsonElement node)
        {
            AnimeListEntry toReturn = new AnimeListEntry();
            toReturn.Id = node.GetProperty("node").GetProperty("id").GetInt32();
            toReturn.IdMal = toReturn.Id;
            toReturn.Title = node.GetProperty("node").GetProperty("title").GetString();
            toReturn.EpisodesSeen = node.GetProperty("list_status").GetProperty("num_episodes_watched").GetInt32();
            toReturn.NumberOfEpisodes = node.GetProperty("node").GetProperty("num_episodes").GetInt32();
            toReturn.Status = node.GetProperty("list_status").GetProperty("status").GetString();
            return toReturn;
        }

        public static AnimeListEntry CreateFromAnilistJsonElement(JsonElement node)
        {
            AnimeListEntry toReturn = new AnimeListEntry();
            toReturn.Id = node.GetProperty("mediaId").GetInt32();
            toReturn.IdMal = node.GetProperty("media").GetProperty("idMal").GetInt32();
            toReturn.Title = node.GetProperty("media").GetProperty("title").GetProperty("romaji").GetString();
            toReturn.EpisodesSeen = node.GetProperty("progress").GetInt32();
            toReturn.NumberOfEpisodes = node.GetProperty("media").GetProperty("episodes").GetInt32();
            toReturn.Status = node.GetProperty("status").GetString();
            return toReturn;
        }
    }
}
