using System;
using System.Collections.Generic;
using TamaChanBot.API.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace CoreModule
{
    internal class YouTubeCommand
    {
        private YouTubeService youTubeService;

        private const string PLAYLIST_KIND = "youtube#playlist";
        private const string PLAYLIST_URL_FORMAT = "https://www.youtube.com/playlist?list={0}";

        private const string VIDEO_KIND = "youtube#video";
        private const string VIDEO_URL_FORMAT = "https://www.youtube.com/watch?v={0}";

        private const string CHANNEL_KIND = "youtube#channel";
        private const string CHANNEL_URL_FORMAT = "https://www.youtube.com/channel/{0}";

        private const string NO_DESCRIPTION = "No Description.";

        internal YouTubeCommand(string apiKey)
        {
            youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "TamaChanBot Core Module"
            });
        }

        internal EmbedResponse Execute(string query)
        {
            SearchListResponse search = GetSearch(query);
            if (search == null || search.Items == null || search.Items.Count == 0)
                return GetFailureResponse(query);
            else
                return GetSuccessResponse(query, search.Items[0]);
        }

        private SearchListResponse GetSearch(string query)
        {
            SearchResource.ListRequest listRequest = youTubeService.Search.List("snippet");
            listRequest.Q = query;
            listRequest.MaxResults = 1;

            SearchListResponse search = listRequest.Execute();
            return search;
        }

        private EmbedResponse GetFailureResponse(string failedQuery)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Failure);
            builder.AddMessage("No videos found.", $"The query \"{failedQuery}\" yielded no results.");
            return builder.Build();
        }

        private EmbedResponse GetSuccessResponse(string query, SearchResult result)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Success);

            builder.SetTitle(result.Snippet.Title);

            string description = result.Snippet.Description;
            if (string.IsNullOrWhiteSpace(description))
                description = NO_DESCRIPTION;

            builder.AddMessage(result.Snippet.ChannelTitle, description);
            builder.SetThumbnailURL(result.Snippet.Thumbnails.Default__.Url);
            
            switch(result.Id.Kind)
            {
                case VIDEO_KIND:
                    builder.SetURL(string.Format(VIDEO_URL_FORMAT, result.Id.VideoId));
                    break;
                case CHANNEL_KIND:
                    builder.SetURL(string.Format(CHANNEL_URL_FORMAT, result.Id.ChannelId));
                    break;
                case PLAYLIST_KIND:
                    builder.SetURL(string.Format(PLAYLIST_URL_FORMAT, result.Id.PlaylistId));
                    break;
            }

            CoreModule.Logger.LogInfo($"Title: {result.Snippet.Title}\r\n" +
                $"Channel: {result.Snippet.ChannelTitle}\r\n" +
                $"Desc: {result.Snippet.Description}\r\n" +
                $"Thumb url: {result.Snippet.Thumbnails.Default__.Url}\r\n" +
                $"URL: {string.Format(PLAYLIST_URL_FORMAT, result.Id.PlaylistId)}");
            return builder.Build();
        }
    }
}
