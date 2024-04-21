using DoubleA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleA.HTTP
{
    public static class OAuthAccessTokens
    {
        public static string MalAccessToken { get; set; }
        public static string MalRefreshToken { get; set; }
        public static string AnilistAccessToken { get; set; }
        public static string AnilistRefreshToken { get; set; }

        public static List<AnimeListEntry> MalAnimeList { get; set; }
        public static List<AnimeListEntry> AnilistAnimeList { get; set; }
        public static int AnilistUserId { get; set; }
    }
}
