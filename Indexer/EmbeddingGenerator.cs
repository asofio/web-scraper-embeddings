using WebScraperEmbeddings.Models;
using Azure.AI.OpenAI;
using Serilog;

namespace WebScraperEmbeddings.Indexer
{
    public class EmbeddingGenerator
    {
        private readonly OpenAIClient _oaiClient;
        private readonly string _aoaiDeploymentName;

        public EmbeddingGenerator(string aoaiKey, string aoaiEndpoint, string aoaiDeploymentName)
        {
            _oaiClient = new OpenAIClient(new Uri(aoaiEndpoint), new Azure.AzureKeyCredential(aoaiKey));
            _aoaiDeploymentName = aoaiDeploymentName;
        }

        public async Task<List<ScrapedPage>> GenerateEmeddingsAsync(List<ScrapedPage> scrapedPages) {
            foreach(var scrapedPage in scrapedPages) {
                scrapedPage.titleVector = await GetEmbeddingAsync(scrapedPage.title);
                scrapedPage.contentVector = await GetEmbeddingAsync(scrapedPage.content);

                Log.Information($"Embeddings generated for {scrapedPage.title}");
            }

            return scrapedPages;
        }

        private async Task<IEnumerable<float>> GetEmbeddingAsync(string text)
        {
            var embeddings = await _oaiClient.GetEmbeddingsAsync(_aoaiDeploymentName, new EmbeddingsOptions(text));
            return embeddings.Value.Data[0].Embedding;
        }
    }
}