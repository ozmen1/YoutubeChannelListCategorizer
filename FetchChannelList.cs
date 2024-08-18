using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YoutubeScraper.Models;

namespace YoutubeScraper
{
    public class FetchChannelList
    {
        static string ytApiKey; 

        public static void ProcessChannelList(string filePath, string outputFilePath, string apiKey)
        {
            ytApiKey = apiKey;
            var urls = File.ReadAllLines(filePath);
            var results = new List<ChannelData>();

            foreach (var url in urls)
            {
                var cleanedUrl = url.Trim();
                if (string.IsNullOrEmpty(cleanedUrl))
                {
                    continue;
                }

                try
                {
                    var channelId = GetChannelIdFromUrl(cleanedUrl);
                    var metadata = GetChannelMetadata(channelId);
                    results.Add(metadata);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Bir hata oluştu: {e.Message}");
                }
            }

            var json = JsonSerializer.Serialize(new { Items = results }, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputFilePath, json, Encoding.UTF8);
        }

        static string GetChannelIdFromUrl(string url)
        {
            if (url.Contains("youtube.com/channel/"))
            {
                return url.Split(new[] { "youtube.com/channel/" }, StringSplitOptions.None)[1];
            }
            else
            {
                throw new ArgumentException("Desteklenmeyen URL formatı.");
            }
        }

        static ChannelData GetChannelMetadata(string channelId)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = ytApiKey,
                ApplicationName = "YouTube Metadata Scraper"
            });

            var request = youtubeService.Channels.List("snippet");
            request.Id = channelId;

            var response = request.Execute();
            var item = response.Items.FirstOrDefault();

            if (item != null)
            {
                return new ChannelData
                {
                    Id = item.Id,
                    Country = item.Snippet.Country,
                    Description = item.Snippet.Description,
                    Title = item.Snippet.Title
                };
            }
            else
            {
                throw new Exception("Kanal bulunamadı.");
            }
        }
    }
}
