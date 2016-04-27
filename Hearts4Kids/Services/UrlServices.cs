
using System.Web;

namespace Hearts4Kids.Services
{
    public static class UrlServices
    {
        public static string GetBaseUrl()
        {
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;
            if (!string.IsNullOrWhiteSpace(appUrl)) { appUrl += "/"; }
            var request = HttpContext.Current.Request;
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);
        }
    }
}