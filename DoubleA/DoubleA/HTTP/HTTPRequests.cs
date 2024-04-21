using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;
using System.Threading.Tasks;

namespace DoubleA.HTTP
{
    public static class HTTPRequests
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public static async Task<string> SendPostRequestAsync(string url, Dictionary<string, string> properties)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(properties);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> SendMALGetRequestAsync(string url, Dictionary<string, string> properties)
        {
            StringBuilder fullUrlBuilder = new StringBuilder(url);
            fullUrlBuilder.Append("?");

            foreach (KeyValuePair<string, string> kvp in properties)
            {
                fullUrlBuilder.Append(kvp.Key).Append("=").Append(kvp.Value).Append("&");
            }

            fullUrlBuilder.Length--;
            Console.WriteLine(fullUrlBuilder.ToString());
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, fullUrlBuilder.ToString());
            getRequest.Headers.Add("X-MAL-CLIENT-ID", "6fc21232be4f66ad247267c08942ad75");
            HttpResponseMessage response = await httpClient.SendAsync(getRequest);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> SendMALGetRequestWithAuthAsync(string url, Dictionary<string, string> properties)
        {
            StringBuilder fullUrlBuilder = new StringBuilder(url);
            fullUrlBuilder.Append("?");

            foreach (KeyValuePair<string, string> kvp in properties)
            {
                fullUrlBuilder.Append(kvp.Key).Append("=").Append(kvp.Value).Append("&");
            }

            fullUrlBuilder.Length--;
            Console.WriteLine(fullUrlBuilder.ToString());
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, fullUrlBuilder.ToString());
            getRequest.Headers.Add("Authorization", "Bearer " + OAuthAccessTokens.MalAccessToken);
            HttpResponseMessage response = await httpClient.SendAsync(getRequest);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> SendMALPatchRequestWithAuthAsync(string url, Dictionary<string, string> properties)
        {
            HttpRequestMessage patchRequest = new HttpRequestMessage(HttpMethod.Put, url);
            patchRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", 
                OAuthAccessTokens.MalAccessToken);
            patchRequest.Content = new FormUrlEncodedContent(properties);
            HttpResponseMessage response = await httpClient.SendAsync(patchRequest);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> SendAnilistPostRequestAsync(string graphQLQueryText, Dictionary<string, object> vars)
        {
            GraphQLQuery query = new GraphQLQuery
            {
                query = graphQLQueryText.Replace("{", "{\n").Replace("}", "}\n").Replace(',', '\n'),
                variables = vars
            };
            JsonContent content = JsonContent.Create(query);
            // content.Headers.Add("Access", "application-json");
            HttpResponseMessage response = await httpClient.PostAsync("https://graphql.anilist.co", content);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> SendAnilistPostRequestAsyncWithAuth(string graphQLQueryText, Dictionary<string, object> vars)
        {
            GraphQLQuery query = new GraphQLQuery
            {
                query = graphQLQueryText.Replace("{", "{\n").Replace("}", "}\n").Replace(',', '\n'),
                variables = vars
            };
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "https://graphql.anilist.co");
            Console.WriteLine("PRENULL");
            postRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                OAuthAccessTokens.AnilistAccessToken);
            Console.WriteLine("POSTNULL");
            postRequest.Content = JsonContent.Create(query);
            HttpResponseMessage response = await httpClient.SendAsync(postRequest);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
