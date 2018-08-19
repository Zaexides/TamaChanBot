using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using TamaChanBot.API.Responses;

namespace CoreModule
{
    internal class WikipediaSearchCommand
    {
        private const int MAX_RESULTS = 5;
        private const string WIKIPEDIA_API_URL_FORMAT = "https://en.wikipedia.org/w/api.php?action=query&titles={0}&prop=info&redirects={1}&inprop=url&formatversion=2&format=json";

        internal EmbedResponse Execute(string query)
        {
            APIResponse apiResponse = GetAPIResponse(query);
            if (apiResponse == null || apiResponse.query == null || apiResponse.query.pages == null || apiResponse.query.pages.Length == 0)
                return GetFailureResponse(query);
            else
                return GetSuccessResponse(apiResponse);
        }

        private APIResponse GetAPIResponse(string query)
        {
            WebRequest webRequest = WebRequest.Create(string.Format(WIKIPEDIA_API_URL_FORMAT, query, MAX_RESULTS));
            WebResponse response = webRequest.GetResponse();

            APIResponse apiResponse = null;
            using (Stream contentStream = response.GetResponseStream())
            {
                string jsonContent;

                using (StreamReader reader = new StreamReader(contentStream))
                    jsonContent = reader.ReadToEnd();

                apiResponse = JsonConvert.DeserializeObject<APIResponse>(jsonContent);
            }
            response.Close();
            return apiResponse;
        }
        
        private EmbedResponse GetFailureResponse(string query)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Failure);
            builder.AddMessage("No results found.", $"Query {query} yielded no results.");
            return builder.Build();
        }

        private EmbedResponse GetSuccessResponse(APIResponse response)
        {
            if (response.query.pages[0].missing)
                return GetFailureResponse(response.query.pages[0].title);
            else
            {
                EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Success);

                int resultCount = response.query.pages.Length;
                resultCount = resultCount > MAX_RESULTS ? MAX_RESULTS : resultCount;
                builder.SetTitle($"Found {resultCount} article(s)");

                for(int i = 0; i < resultCount; i++)
                {
                    string articleName = response.query.pages[i].title;
                    APIResponse.Query.Redirect redirect = GetRedirectInfo(response.query, articleName);

                    string messageTitle = articleName;
                    if (redirect != null)
                        messageTitle += $" (from \"{redirect.from}\")";

                    builder.AddMessage(messageTitle, response.query.pages[0].fullurl);
                }

                return builder.Build();
            }
        }

        private APIResponse.Query.Redirect GetRedirectInfo(APIResponse.Query query, string articleName)
        {
            if (query.redirects == null)
                return null;

            foreach(APIResponse.Query.Redirect redirect in query.redirects)
            {
                if (redirect.to.Equals(articleName))
                    return redirect;
            }
            return null;
        }

        private class APIResponse
        {
            public Query query;

            public class Query
            {
                public Page[] pages;
                public Redirect[] redirects;

                public class Page
                {
                    public string title;
                    public bool missing;
                    public string fullurl;
                }

                public class Redirect
                {
                    public string from;
                    public string to;
                }
            }
        }
    }
}
