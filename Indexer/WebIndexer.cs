using WebScraperEmbeddings.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Serilog;

namespace WebScraperEmbeddings.Indexer
{
    public class WebIndexer
    {
        private readonly SearchClient _searchClient;

        public WebIndexer(string cogSearchKey, string cogSearchEndpoint, string indexName)
        {
            Azure.AzureKeyCredential credential = new Azure.AzureKeyCredential(cogSearchKey);
            SearchIndexClient indexClient = new SearchIndexClient(new Uri(cogSearchEndpoint), credential);
            _searchClient = indexClient.GetSearchClient(indexName);

            Log.Information($"Created Cognitive Search client against the {indexName} index.");
        }

        public async Task IndexScrapedPagesAsync(List<ScrapedPage> scrapedPages)
        {
            SearchIndexingBufferedSenderOptions<ScrapedPage> options = new SearchIndexingBufferedSenderOptions<ScrapedPage>() { KeyFieldAccessor = scrapedPage => scrapedPage.Id };
            
            Log.Information($"Attempting to index {scrapedPages.Count} scraped pages...");

            await using SearchIndexingBufferedSender<ScrapedPage> indexer = new SearchIndexingBufferedSender<ScrapedPage>(_searchClient, options);
            await indexer.UploadDocumentsAsync(scrapedPages);

            Log.Information($"Indexing complete.");
        }
    }
}