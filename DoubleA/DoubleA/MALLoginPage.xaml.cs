using DoubleA.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DoubleA
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MALLoginPage : ContentPage
	{
		private readonly string clientId = "6fc21232be4f66ad247267c08942ad75";
		private string pkceCode;
		public MALLoginPage ()
		{
			InitializeComponent ();

			pkceCode = GeneratePKCECode();

			StringBuilder malAuthUrlBuilder = new StringBuilder();
			malAuthUrlBuilder.Append("https://myanimelist.net/v1/oauth2/authorize?response_type=code");
			malAuthUrlBuilder.Append("&client_id=").Append(clientId);
			malAuthUrlBuilder.Append("&code_challenge=").Append(pkceCode);

			webView.Source = malAuthUrlBuilder.ToString();
            webView.Navigating += webView_Navigating;
		}

        private async void webView_Navigating(object sender, WebNavigatingEventArgs e)
        {
			if (e.Url.StartsWith("http://localhost/oauth"))
			{
				string authorizationCode = e.Url.Split('?')[1].Split('=')[1];

				Dictionary<string, string> accessTokenRequestContent = new Dictionary<string, string>();
				accessTokenRequestContent.Add("client_id", clientId);
				accessTokenRequestContent.Add("code", authorizationCode);
				accessTokenRequestContent.Add("code_verifier", pkceCode);
				accessTokenRequestContent.Add("grant_type", "authorization_code");

				string authorizationResult = await HTTPRequests.SendPostRequestAsync("https://myanimelist.net/v1/oauth2/token",
					accessTokenRequestContent);
				JsonDocument authorizationJson = JsonDocument.Parse(authorizationResult);
				OAuthAccessTokens.MalAccessToken = authorizationJson.RootElement.GetProperty("access_token").GetString();
				OAuthAccessTokens.MalRefreshToken = authorizationJson.RootElement.GetProperty("refresh_token").GetString();

				await DisplayAlert("Success", "You have successfully logged into MyAnimeList", "OK");
                await Navigation.PopAsync();
            }
			else if (!e.Url.StartsWith("https://myanimelist.net"))
			{
				await Navigation.PopAsync();
			}
        }

        private readonly string pkceAllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
		private string GeneratePKCECode()
		{
			StringBuilder pkceBuilder = new StringBuilder();
			Random rand = new Random();

			for (int i = 0; i < 127; i++)
			{
				// gets random character from above string, appends it to pkceBuilder
				pkceBuilder.Append(pkceAllowedCharacters[rand.Next(pkceAllowedCharacters.Length)]);
			}

			return pkceBuilder.ToString();
		}
	}
}