using System;
using TamaChanBot.API.Responses;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;

namespace CoreModule
{
    internal class GoogleCommand
    {
        private const int MAX_RESULTS = 5;

        private CustomsearchService customsearchService;
        private string searchId;

        internal GoogleCommand(CoreModuleSettings.GoogleSettings googleSettings)
        {
            try
            {
                customsearchService = new CustomsearchService(new BaseClientService.Initializer() { ApiKey = googleSettings.apiKey, ApplicationName = "TamaChanBot Core Module" });
                this.searchId = googleSettings.searchId;
            }
            catch (Exception ex)
            {
                CoreModule.Logger.LogError("An error occured! " + ex.ToString());
            }
        }

        internal EmbedResponse Execute(string query)
        {
            Search search = GetSearch(query);
            if (search == null || search.Items == null || search.Items.Count == 0)
                return GetFailureResponse(query);
            else
                return GetSuccessResponse(query, search);
        }

        private Search GetSearch(string query)
        {
            CseResource.ListRequest listRequest = customsearchService.Cse.List(query);
            listRequest.Cx = searchId;
            return listRequest.Execute();
        }

        private EmbedResponse GetFailureResponse(string failedQuery)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Failure);
            builder.AddMessage("No results found.", $"The query \"{failedQuery}\" yielded no results.");
            return builder.Build();
        }

        private EmbedResponse GetSuccessResponse(string query, Search results)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Success);

            int resultCount = results.Items.Count > MAX_RESULTS ? MAX_RESULTS : results.Items.Count;

            builder.SetTitle($"Query \"{query}\" yielded {resultCount} results");

            for(int i = 0; i < resultCount; i++)
                builder.AddMessage(results.Items[i].Title, results.Items[i].Link);
            return builder.Build();
        }
    }
}
