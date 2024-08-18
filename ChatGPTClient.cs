using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YoutubeScraper.Models;

namespace YoutubeScraper
{
    public class ChatGPTClient
    {
        private readonly HttpClient _httpClient;

        public ChatGPTClient(string apiKey)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<string> GetChannelCategoryAsync(ChannelData channel)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are an assistant that predicts YouTube channel categories based on their title, description, and country." },
                new { role = "user", content = $"Title: {channel.Title}, Description: {channel.Description}, Country: {channel.Country}" }
            }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(responseBody);
            var completion = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return completion.Trim();
        }
    }
}
