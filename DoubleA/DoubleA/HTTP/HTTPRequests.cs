﻿using System;
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
    }
}
