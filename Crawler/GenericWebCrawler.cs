using Abot2.Crawler;
using Abot2.Poco;
using WebScraperEmbeddings.Models;
using AngleSharp.Html.Parser;
using Serilog;
using System.Web;

namespace WebScraperEmbeddings.Crawler
{
    public class GenericWebCrawler
    {
        private readonly List<Uri> _seedUris;
        private readonly Settings _settings;
        private List<ScrapedPage> _scrapedPages = new List<ScrapedPage>();

        public List<ScrapedPage> ScrapedPages { 
            get {
                return _scrapedPages;
            }
        }

        public GenericWebCrawler(List<Uri> seedUris, Settings settings)
        {
            _seedUris = seedUris;
            _settings = settings;
        }

        public async Task CrawlAsync()
        {
            await SimpleCrawlerAsync();
        }

        private async Task SimpleCrawlerAsync()
        {
            var config = new CrawlConfiguration
            {
                MaxConcurrentThreads = 8,
                MaxCrawlDepth = _settings.MaxWebCrawlDepth,
                MinCrawlDelayPerDomainMilliSeconds = 500 //Wait this many millisecs between requests
            };
            var crawler = new PoliteWebCrawler(config);

            crawler.ShouldCrawlPageDecisionMaker = (pageToCrawl, crawlContext) => 
            {
                var decision = new CrawlDecision{ Allow = true };
                if(pageToCrawl.Uri.AbsoluteUri.Contains("login"))
                    return new CrawlDecision{ Allow = false, Reason = "Skip login pages." };
                
                return decision;
            };

            crawler.PageCrawlCompleted += PageCrawlCompleted;

            // Need to check crawl result here...
            foreach(var uri in _seedUris) {
                await crawler.CrawlAsync(uri);
            }
        }

        public void PageCrawlCompleted(object? sender, PageCrawlCompletedArgs e)
        {
            var httpStatus = e.CrawledPage.HttpResponseMessage.StatusCode;
            var rawPageText = e.CrawledPage.Content.Text;
            
            var parser = new HtmlParser();
            var htmldoc = parser.ParseDocument(rawPageText);

            var scriptElements = htmldoc.QuerySelectorAll("script");
            foreach (var scriptElement in scriptElements)
            {
                scriptElement.Remove();
            }

            var content = htmldoc.DocumentElement.TextContent;
            var trimmedContent = content.Replace("\n", String.Empty).Replace("\t", String.Empty);

            if(!String.IsNullOrEmpty(trimmedContent)) {
                _scrapedPages.Add(new ScrapedPage
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = e.CrawledPage.Uri.ToString(),
                    Content = trimmedContent,
                    Url = e.CrawledPage.Uri.ToString()
                });
            }
        }
    }    
}