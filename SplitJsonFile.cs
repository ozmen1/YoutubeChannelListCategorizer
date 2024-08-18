using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace YoutubeScraper
{
    public class JsonWorker
    {
        public static void SplitJsonFile(string inputFilePath, string outputFilePathPrefix, int batchSize)
        {
            var jsonString = File.ReadAllText(inputFilePath);
            var jsonDocument = JsonDocument.Parse(jsonString);

            var items = jsonDocument.RootElement.GetProperty("Items").EnumerateArray().ToList();
            var totalItems = items.Count;

            for (int i = 0; i < totalItems; i += batchSize)
            {
                var batchItems = items.Skip(i).Take(batchSize).ToList();
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var batchJson = JsonSerializer.Serialize(new { Items = batchItems }, jsonOptions);
                var outputFilePath = $"{outputFilePathPrefix}{(i / batchSize) + 1}.json";

                File.WriteAllText(outputFilePath, batchJson, Encoding.UTF8);

                Console.WriteLine($"Dosya kaydedildi: {outputFilePath}");
            }
        }
    }
}
