using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using YoutubeScraper.Models;
using static System.Net.Mime.MediaTypeNames;

namespace YoutubeScraper
{
    public class GeminiAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiAIClient(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<string> GenerateContentAsync(ChannelData channel)
        {
            var textContent = "You are an assistant that predicts YouTube channel categories based on their title, description, and country. Only response category name in csv format! (give me maximum 3 category). "
                + "Channel Id: " + channel.Id + ", Channel Country: " + channel.Country + ", Channel Title: " + channel.Title + ", Channel Description: " + channel.Description;
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = textContent }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUri = $"https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key={_apiKey}";
            var contentText = "No response";

            int maxRetries = 5;
            int retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    var response = await _httpClient.PostAsync(requestUri, content);

                    if (response.StatusCode == (System.Net.HttpStatusCode)429) 
                    {
                        retryCount++;
                        var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(10);
                        await Task.Delay(retryAfter);
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var jsonDoc = JsonDocument.Parse(responseBody);

                    if (jsonDoc.RootElement.TryGetProperty("candidates", out var candidatesArray) &&
                        candidatesArray[0].TryGetProperty("content", out var contentElement) &&
                        contentElement.TryGetProperty("parts", out var partsArray) &&
                        partsArray[0].TryGetProperty("text", out var textElement))
                    {
                        contentText = textElement.GetString();
                        break;
                    }
                    else
                    {
                        contentText = "Response format is incorrect";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    contentText = $"Error: {ex.Message}";
                    break;
                }
            }

            return contentText;
        }


    }
}
