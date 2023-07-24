using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScraperEmbeddings.Models
{
    public class Settings
    {
        public string AzureOpenAIKey { get; set; }
        public string AzureOpenAIEndpoint { get; set; }
        public string AzureOpenAIModelDeploymentName { get; set; }
        public string AzureCognitiveSearchKey { get; set; }
        public string AzureCognitiveSearchEndpoint { get; set; }
        public string AzureCognitiveSearchIndexName { get; set; }
        public int MaxWebCrawlDepth { get; set; }
        public List<string> UrlsToCrawl { get; set; }

        public Settings()
        {
            this.AzureOpenAIKey = "[Provide an Azure Open AI Key]";
            this.AzureOpenAIEndpoint = "[Provide an Azure Open AI Endpoint]";
            this.AzureOpenAIModelDeploymentName = "[Provide an Azure Open AI Model Deployment Name]";
            this.AzureCognitiveSearchKey = "[Provide an Azure Cognitive Search Key]";
            this.AzureCognitiveSearchEndpoint = "[Provide an Azure Cognitive Search Endpoint]";
            this.AzureCognitiveSearchIndexName = "[Provide an Azure Cognitive Search Index Name]";
            this.MaxWebCrawlDepth = 3;
            this.UrlsToCrawl = new List<string>();
        }
    }
}