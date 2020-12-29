using Wangkanai.Detection.Services;

namespace BlazorPrerenderingForSEO.Server.Services
{
    public class CrawlerPrerenderedManagerService : IPrerenderedManagerService
    {
        private readonly IDetectionService detectionService;

        public CrawlerPrerenderedManagerService(IDetectionService detectionService)
        {
            this.detectionService = detectionService;
        }

        public bool ShouldUsePrerendering()
        {
            return detectionService.Crawler.IsCrawler;
        }
    }
}
