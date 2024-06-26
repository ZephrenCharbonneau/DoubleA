﻿using DoubleA.HTTP;
using DoubleA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DoubleA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnilistLoginPage : ContentPage
    {
        private readonly string clientId = "18146";
        private readonly string clientSecret = "KKdWNtjytPELQFIRCESxb9IELkDybveK4zK8wH1O";
        private readonly string redirectUrl = "http://localhost/oauth";
        public AnilistLoginPage()
        {
            InitializeComponent();

            StringBuilder anilistAuthUrlBuilder = new StringBuilder();
            anilistAuthUrlBuilder.Append("https://anilist.co/api/v2/oauth/authorize?");
            anilistAuthUrlBuilder.Append("&client_id=").Append(clientId);
            anilistAuthUrlBuilder.Append("&redirect_uri=").Append(redirectUrl);
            anilistAuthUrlBuilder.Append("&response_type=code");

            webView.Source = anilistAuthUrlBuilder.ToString();
            webView.Navigating += webView_Navigating;
        }

        private async void webView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith("http://localhost/oauth"))
            {
                string authorizationCode = e.Url.Split('?')[1].Split('=')[1];
                Console.WriteLine("NAV EVENT URL: " + e.Url);

                Dictionary<string, string> accessTokenRequestContent = new Dictionary<string, string>();
                accessTokenRequestContent.Add("grant_type", "authorization_code");
                accessTokenRequestContent.Add("client_id", clientId);
                accessTokenRequestContent.Add("client_secret", clientSecret);
                accessTokenRequestContent.Add("redirect_uri", redirectUrl);
                accessTokenRequestContent.Add("code", authorizationCode);

                string authorizationResult = await HTTPRequests.SendPostRequestAsync("https://anilist.co/api/v2/oauth/token",
                    accessTokenRequestContent);
                JsonDocument authorizationJson = JsonDocument.Parse(authorizationResult);
                OAuthAccessTokens.AnilistAccessToken = authorizationJson.RootElement.GetProperty("access_token").GetString();
                OAuthAccessTokens.AnilistRefreshToken = authorizationJson.RootElement.GetProperty("refresh_token").GetString();

                string userIdRequest = await HTTPRequests.SendAnilistPostRequestAsyncWithAuth("query {Viewer {id}}",
                    new Dictionary<string, object>());
                JsonDocument userIdJson = JsonDocument.Parse(userIdRequest);
                OAuthAccessTokens.AnilistUserId = userIdJson.RootElement.GetProperty("data").GetProperty("Viewer")
                    .GetProperty("id").GetInt32();

                Dictionary<string, object> variables = new Dictionary<string, object>();
                variables.Add("userId", OAuthAccessTokens.AnilistUserId);
                variables.Add("page", 1);
                string userListRequest = await HTTPRequests.SendAnilistPostRequestAsyncWithAuth
                    ("query($userId: Int, $page: Int) {Page (page: $page, perPage: 50) {mediaList(userId: $userId) {mediaId,progress," +
                    "status,media{title {romaji},episodes,idMal}} pageInfo {currentPage,hasNextPage}}}", variables);
                Console.WriteLine(userListRequest);
                bool hasNextPage = true;
                JsonDocument userListJson;
                List<AnimeListEntry> userAnimeList = new List<AnimeListEntry>();

                do
                {
                    userListJson = JsonDocument.Parse(userListRequest);

                    JsonElement pageNode = userListJson.RootElement.GetProperty("data").GetProperty("Page");
                    foreach (JsonElement node in pageNode.GetProperty("mediaList").EnumerateArray())
                        userAnimeList.Add(AnimeListEntry.CreateFromAnilistJsonElement(node));

                    if (pageNode.GetProperty("pageInfo").GetProperty("hasNextPage").GetBoolean())
                    {
                        int page = (int)variables["page"];
                        variables["page"] = page + 1;
                        userListRequest = await HTTPRequests.SendAnilistPostRequestAsyncWithAuth
                            ("query($userId: Int, $page: Int) {Page (page: $page, perPage: 50) {mediaList(userId: $userId) {mediaId," +
                            "progress,status,media{title {romaji},episodes,idMal}} pageInfo {currentPage,hasNextPage}}}", variables);
                    }
                    else
                        hasNextPage = false;
                } while (hasNextPage);

                OAuthAccessTokens.AnilistAnimeList = userAnimeList;

                await DisplayAlert("Success", "You have successfully logged into Anilist", "OK");
                await Navigation.PopAsync();
            }
            else if (!e.Url.StartsWith("https://anilist.co"))
            {
                await Navigation.PopAsync();
            }
        }
    }
}