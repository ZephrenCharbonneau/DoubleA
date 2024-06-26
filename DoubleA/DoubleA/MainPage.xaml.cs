﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using DoubleA.HTTP;
using System.Collections.ObjectModel;
using DoubleA.Models;
using System.Text.Json;
using System.Net.Http.Headers;

namespace DoubleA
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Anime> animeList = new ObservableCollection<Anime>();
        UserSettings userSettings;

        public UserSettings UserSettings {
            get { return userSettings; }
            set { userSettings = value; }
        }
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            userSettings = await App.SettingsDatabase.GetSettingsAsync();
            PopulateAnimeList();
            animeListView.ItemsSource = animeList;
        }

        private async void PopulateAnimeList()
        {
            if ((OAuthAccessTokens.MalAccessToken != null && OAuthAccessTokens.AnilistAccessToken == null) ||
                userSettings.DefaultListSource == "MAL")
            {
                Dictionary<string, string> requestParams = new Dictionary<string, string>();
                requestParams.Add("ranking_type", "all");
                requestParams.Add("limit", "50");
                requestParams.Add("fields", "mean,start_date,end_date,media_type,num_episodes");
                String response = await HTTPRequests.SendMALGetRequestAsync("https://api.myanimelist.net/v2/anime/ranking",
                    requestParams);

                JsonDocument jsonResponse = JsonDocument.Parse(response);
                JsonElement dataArray = jsonResponse.RootElement.GetProperty("data");

                foreach (JsonElement element in dataArray.EnumerateArray())
                {
                    animeList.Add(Anime.CreateFromMALJsonElement(element.GetProperty("node")));
                }
            }
            else if ((OAuthAccessTokens.MalAccessToken == null && OAuthAccessTokens.AnilistAccessToken != null) ||
                userSettings.DefaultListSource == "Anilist")
            {
                string queryText = "query { Page { media (type: ANIME, sort: SCORE_DESC) { id,title { romaji },format,startDate {year,month},endDate {year,month},episodes,meanScore}}}";
                string response = await HTTPRequests.SendAnilistPostRequestAsync(queryText, new Dictionary<string, object>());

                JsonDocument jsonResponse = JsonDocument.Parse(response);
                JsonElement dataArray = jsonResponse.RootElement.GetProperty("data").GetProperty("Page").GetProperty("media");

                foreach (JsonElement element in dataArray.EnumerateArray())
                {
                    animeList.Add(Anime.CreateFromAnilistJsonElement(element));
                }
            }
        }

        private async void btnSearch_Clicked(object sender, EventArgs e)
        {
            animeList.Clear();

            if ((OAuthAccessTokens.MalAccessToken != null && OAuthAccessTokens.AnilistAccessToken == null) ||
                userSettings.DefaultListSource == "MAL")
            {
                Dictionary<string, string> requestParams = new Dictionary<string, string>();
                requestParams.Add("q", searchBar.Text);
                requestParams.Add("limit", "50");
                requestParams.Add("fields", "mean,start_date,end_date,media_type,num_episodes");
                String response = await HTTPRequests.SendMALGetRequestAsync("https://api.myanimelist.net/v2/anime",
                    requestParams);

                Console.WriteLine(response);

                JsonDocument jsonResponse = JsonDocument.Parse(response);
                Console.WriteLine("p1");
                JsonElement dataArray = jsonResponse.RootElement.GetProperty("data");
                Console.WriteLine("p2");

                foreach (JsonElement element in dataArray.EnumerateArray())
                {
                    animeList.Add(Anime.CreateFromMALJsonElement(element.GetProperty("node")));
                    Console.WriteLine("p3");
                }
            }
            else if ((OAuthAccessTokens.MalAccessToken == null && OAuthAccessTokens.AnilistAccessToken != null) ||
                userSettings.DefaultListSource == "Anilist")
            {
                string queryText = "query { Page { media (type: ANIME, sort: SCORE_DESC, search: \"" + searchBar.Text + "\") { id,title { romaji },format,startDate {year,month},endDate {year,month},episodes,meanScore}}}";
                string response = await HTTPRequests.SendAnilistPostRequestAsync(queryText, new Dictionary<string, object>());

                Console.WriteLine(response);

                JsonDocument jsonResponse = JsonDocument.Parse(response);
                JsonElement dataArray = jsonResponse.RootElement.GetProperty("data").GetProperty("Page").GetProperty("media");

                foreach (JsonElement element in dataArray.EnumerateArray())
                {
                    animeList.Add(Anime.CreateFromAnilistJsonElement(element));
                }
            }
        }

        private void btnAdvancedSearch_Clicked(object sender, EventArgs e)
        {
            searchBar.Text = OAuthAccessTokens.MalAnimeList.Count.ToString();
        }

        private async void btnMALLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MALLoginPage());
        }

        private async void btnAnilistLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AnilistLoginPage());
        }

        private async void btnSettings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage(this));
        }

        private async void animeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Anime selectedAnime = animeListView.SelectedItem as Anime;
            int selectedAnimeId = selectedAnime.Id;

            if ((OAuthAccessTokens.MalAccessToken != null && OAuthAccessTokens.AnilistAccessToken == null) || 
                userSettings.DefaultListSource == "MAL")
            {
                Console.WriteLine("IS THIS RUNNING");
                Dictionary<string, string> requestParams = new Dictionary<string, string>();
                requestParams.Add("fields", "mean,start_date,end_date,media_type,num_episodes,synopsis,average_episode_duration," +
                    "source,genres,alternative_titles,num_list_users");
                String response = await HTTPRequests.SendMALGetRequestAsync("https://api.myanimelist.net/v2/anime/" + selectedAnimeId,
                    requestParams);

                Console.WriteLine(response);

                JsonDocument jsonResponse = JsonDocument.Parse(response);
                AnimeDetailed detailed = AnimeDetailed.CreateFromMALJsonElement(jsonResponse.RootElement);
                await Navigation.PushAsync(new AnimeDetailsPage(detailed, userSettings));
            }
            else if ((OAuthAccessTokens.MalAccessToken == null && OAuthAccessTokens.AnilistAccessToken != null) || 
                userSettings.DefaultListSource == "Anilist")
            {
                string queryText = "query { Media (type: ANIME, id: " + selectedAnimeId + ") { id,title { romaji },format,startDate " +
                    "{year,month},endDate {year,month},episodes,meanScore,description,duration,source,genres,synonyms,popularity}}";
                string response = await HTTPRequests.SendAnilistPostRequestAsync(queryText, new Dictionary<string, object>());

                Console.WriteLine(response);

                JsonDocument jsonResponse = JsonDocument.Parse(response);
                JsonElement mediaElement = jsonResponse.RootElement.GetProperty("data").GetProperty("Media");
                AnimeDetailed detailed = AnimeDetailed.CreateFromAnilistJsonElement(mediaElement);
                await Navigation.PushAsync(new AnimeDetailsPage(detailed, userSettings));
            }
        }
    }
}
