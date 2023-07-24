using Azure.Search.Documents.Indexes;

namespace WebScraperEmbeddings.Models
{
    public class ScrapedPage
    {
        [SimpleField(IsKey = true)]
        public string Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<float> TitleVector { get; set; }
        public string Content { get; set; }
        public IEnumerable<float> ContentVector { get; set; }
        public string Url { get; set; }

        public ScrapedPage()
        {
            Id = Guid.NewGuid().ToString();
            Title = String.Empty;
            TitleVector = new List<float>();
            Content = String.Empty;
            ContentVector = new List<float>();
            Url = String.Empty;
        }
    }
}