using Microsoft.AspNetCore.Http;

namespace BlazorPrerenderingForSEO.Server.Services
{
    public class SpecificQueryPrerenderedManagerService : IPrerenderedManagerService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public SpecificQueryPrerenderedManagerService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public bool ShouldUsePrerendering()
        {
            return httpContextAccessor.HttpContext.Request.Query.ContainsKey("prerender");
        }
    }
}
