using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using YoutubeScraper.Models;

namespace YoutubeScraper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            var configuration = builder.Build();
            string geminiApiKey = configuration["GeminiApiKey"];
            string youtubeApiKey = configuration["YoutubeApiKey"];

            #region Get Youtube Metadata
            //string inputFilePath = "channels.txt";
            //string outputFilePath = "channel_metadata.json";
            //FetchChannelList.ProcessChannelList(inputFilePath, outputFilePath, youtubeApiKey); 
            #endregion
            
            #region Split Json File
            //string inputFilePath = "channel_metadata.json";
            //string outputFilePathPrefix = "channel_metadata_part_";
            //int batchSize = 45;
            //JsonWorker.SplitJsonFile(inputFilePath, outputFilePathPrefix, batchSize); 
            #endregion

            #region Predict Categories with ChatGPT
            //string filePath = "channel_metadata.json";
            //string json = File.ReadAllText(filePath);
            //List<ChannelData> youtubeChannels = JsonConvert.DeserializeObject<List<ChannelData>>(json);
            //var client = new ChatGPTClient(youtubeApiKey);
            //var csvFilePath = "youtube_channels.csv";
            //using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
            //{
            //    writer.WriteLine("Kanal ID,Kanal Title,Kanal Kategori");
            //    foreach (var channel in youtubeChannels)
            //    {
            //        var category = await client.GetChannelCategoryAsync(channel);
            //        var csvLine = $"{channel.Id},{channel.Title},{category}";
            //        writer.WriteLine(csvLine);
            //    }
            //}
            //Console.WriteLine($"Sonuçlar {csvFilePath} dosyasına kaydedildi."); 
            #endregion

            #region Predict Categories with Gemini
            string filePath = "channel_metadata.json";
            string json = File.ReadAllText(filePath);
            List<ChannelData> youtubeChannels = JsonConvert.DeserializeObject<List<ChannelData>>(json);
            var client = new GeminiAIClient(geminiApiKey);
            var csvFilePath = "youtube_channels_gemini.csv";
            using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
            {
                writer.WriteLine("Kanal ID,Kanal Title,Kanal Kategori");
                foreach (var channel in youtubeChannels)
                {
                    var category = await client.GenerateContentAsync(channel);
                    var csvLine = $"{channel.Id},{channel.Title},{category}";
                    writer.WriteLine(csvLine);
                }
            }
            Console.WriteLine($"Sonuçlar {csvFilePath} dosyasına kaydedildi."); 
            #endregion
        }
    }
}
