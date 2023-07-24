using System;
using System.Threading.Tasks;
using Abot2;
using Abot2.Crawler;
using Abot2.Poco;
using WebScraperEmbeddings.Crawler;
using WebScraperEmbeddings.Indexer;
using WebScraperEmbeddings.Models;
using AngleSharp.Html.Parser;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace WebScraperEmbeddings
{
    class Program
    {
        private static IConfiguration? _config;
        private static Settings? _settings;

        static async Task Main(string[] args)
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            _settings = _config.GetRequiredSection("Settings").Get<Settings>()!;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            if(_settings.UrlsToCrawl.Count == 0) {
                Log.Error("No URLs to crawl. Please provide at least one URL to crawl in the appsettings.json file.");
                return;
            }

            // Crawl all of the configured Urls
            var uris = _settings.UrlsToCrawl.Select(x => new Uri(x)).ToList();
            var crawler = new GenericWebCrawler(uris, _settings);
            var scrapedPages = await crawler.CrawlAsync();

            // Generate embeddings for each page
            var embeddingsGenerator = new EmbeddingGenerator(_settings.AzureOpenAIKey, _settings.AzureOpenAIEndpoint, _settings.AzureOpenAIModelDeploymentName);
            scrapedPages = await embeddingsGenerator.GenerateEmbeddingsAsync(scrapedPages);

            // Persist the index to Azure Cognitive Search
            var indexer = new WebIndexer(_settings.AzureCognitiveSearchKey, _settings.AzureCognitiveSearchEndpoint, _settings.AzureCognitiveSearchIndexName);
            await indexer.IndexScrapedPagesAsync(scrapedPages);
        }        
    }
}