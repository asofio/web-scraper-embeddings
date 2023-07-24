using Azure.Search.Documents.Indexes;

namespace WebScraperEmbeddings.Models
{
    public class ScrapedPage
    {
        [SimpleField(IsKey = true)]
        public string Id { get; set; }
        public string title { get; set; }
        public IEnumerable<float> titleVector { get; set; }
        public string content { get; set; }
        public IEnumerable<float> contentVector { get; set; }
        public string url { get; set; }
    }
}