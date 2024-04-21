using DoubleA.HTTP;
using DoubleA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DoubleA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnimeDetailsPage : ContentPage
    {
        private AnimeDetailed shownAnime;
        private UserSettings userSettings;
        private AnimeListEntry shownAnimeListEntry;
        public AnimeDetailsPage(AnimeDetailed anime, UserSettings settings)
        {
            InitializeComponent();
            shownAnime = anime;
            userSettings = settings;
            shownAnimeListEntry = null;
            lblAnimeTitle.Text = shownAnime.Title;

            if (OAuthAccessTokens.MalAnimeList == null && OAuthAccessTokens.AnilistAnimeList == null)
            {
                btnAddEp.IsVisible = false;
                lblProgress.IsVisible = false;
            }
            else
            {
                Console.WriteLine("HERE");
                List<AnimeListEntry> listToSearch = new List<AnimeListEntry>();

                if (userSettings.DefaultListSource == "MAL" || (OAuthAccessTokens.MalAccessToken != null &&
                    OAuthAccessTokens.AnilistAccessToken == null))
                    listToSearch = OAuthAccessTokens.MalAnimeList;
                else if (userSettings.DefaultListSource == "Anilist" || (OAuthAccessTokens.MalAccessToken == null &&
                    OAuthAccessTokens.AnilistAccessToken != null))
                    listToSearch = OAuthAccessTokens.AnilistAnimeList;

                foreach (AnimeListEntry entry in listToSearch)
                {
                    if (entry.Id == shownAnime.Id)
                    {
                        shownAnimeListEntry = entry;
                        break;
                    }
                }

                if (shownAnimeListEntry != null)
                    lblProgress.Text = shownAnimeListEntry.EpisodesSeen + "/" + shownAnimeListEntry.NumberOfEpisodes;
                else
                    lblProgress.Text = "0/" + shownAnime.EpisodeCount;
            }
        }

        private void btnAddEp_Clicked(object sender, EventArgs e)
        {
            if (OAuthAccessTokens.MalAccessToken != null)
                UpdateMALList();

            if (OAuthAccessTokens.AnilistAccessToken != null)
                UpdateAnilistList();
        }

        private async void UpdateMALList()
        {
            int newEpisodeCount = 1;
            bool alreadyCompleted = false;

            foreach (AnimeListEntry entry in OAuthAccessTokens.MalAnimeList)
            {
                if (entry.Id == shownAnime.Id)
                {
                    if (entry.EpisodesSeen == entry.NumberOfEpisodes)
                        alreadyCompleted = true;
                    else
                    {
                        entry.EpisodesSeen++;
                        newEpisodeCount = entry.EpisodesSeen;
                    }
                    break;
                }
            }

            if (!alreadyCompleted)
            {
                Dictionary<string, string> content = new Dictionary<string, string>();
                content.Add("num_watched_episodes", newEpisodeCount.ToString());
                await HTTPRequests.SendMALPatchRequestWithAuthAsync("https://api.myanimelist.net/v2/anime/" + shownAnime.Id
                    + "/my_list_status", content);
            }
        }

        private async void UpdateAnilistList()
        {
            int newEpisodeCount = 1;
            bool alreadyCompleted = false;

            foreach (AnimeListEntry entry in OAuthAccessTokens.AnilistAnimeList)
            {
                if (entry.Id == shownAnime.Id)
                {
                    if (entry.EpisodesSeen == entry.NumberOfEpisodes)
                        alreadyCompleted = true;
                    else
                    {
                        entry.EpisodesSeen++;
                        newEpisodeCount = entry.EpisodesSeen;
                    }
                    break;
                }
            }

            Console.WriteLine("HERE?");

            if (!alreadyCompleted)
            {
                if (newEpisodeCount == 1)
                {
                    Console.WriteLine("NEW EP IS 1");
                    Dictionary<string, object> variables = new Dictionary<string, object>();
                    variables.Add("mediaId", shownAnime.Id);
                    await HTTPRequests.SendAnilistPostRequestAsyncWithAuth("mutation ($mediaId: Int) {SaveMediaListEntry " +
                        "(mediaId: $mediaId, progress: 1, status: CURRENT) {id,status}}", variables);
                }
                else
                {
                    Dictionary<string, object> variables = new Dictionary<string, object>();
                    variables.Add("mediaId", shownAnime.Id);
                    variables.Add("progress", newEpisodeCount);
                    await HTTPRequests.SendAnilistPostRequestAsyncWithAuth("mutation ($mediaId: Int, $progress: Int) {SaveMediaListEntry " +
                        "(mediaId: $mediaId, progress: $progress, status: CURRENT) {id,status}}", variables);
                }
            }
        }
    }
}