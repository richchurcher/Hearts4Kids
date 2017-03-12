using System.Web;
using System.Web.Mvc;
using NWebsec.Mvc.HttpHeaders;
using NWebsec.Mvc.HttpHeaders.Csp;

namespace H4K.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());


            // Require HTTPS to be used across the whole site. System.Web.Mvc.RequireHttpsAttribute performs a
            // 302 Temporary redirect from a HTTP URL to a HTTPS URL. This filter gives you the option to perform a
            // 301 Permanent redirect or a 302 temporary redirect. You should perform a 301 permanent redirect if the
            // page can only ever be accessed by HTTPS and a 302 temporary redirect if the page can be accessed over
            // HTTP or HTTPS.
            // filters.Add(new RedirectToHttpsAttribute(true));

            filters.Add(new XContentTypeOptionsAttribute());
            filters.Add(new XFrameOptionsAttribute() { Policy = XFrameOptionsPolicy.Deny });

            // CSP
            filters.Add(new CspAttribute());
            filters.Add(new CspDefaultSrcAttribute() { None = true });
            filters.Add(new CspBaseUriAttribute() { Self = false });
            filters.Add(new CspChildSrcAttribute() { Self = false });
            filters.Add(new CspConnectSrcAttribute()
            {
                CustomSources = string.Join(" ", "localhost:*", "ws://localhost:*"),
                Self = true
            });
            filters.Add(new CspFontSrcAttribute()
            {
                CustomSources = string.Join(" ", "maxcdn.bootstrapcdn.com"),
                Self = true
            });
            filters.Add(new CspFormActionAttribute() { Self = true });
            filters.Add(new CspFrameAncestorsAttribute() { Self = false });
            filters.Add(new CspImgSrcAttribute()
            {
                CustomSources = "data:",
                Self = true
            });
            filters.Add(new CspScriptSrcAttribute()
            {
                CustomSources = string.Join(
                    " ",
                    "localhost:*",
                    "ajax.googleapis.com",
                    "ajax.aspnetcdn.com",
                    "cdnjs.cloudflare.com"),
                Self = true
            });
            filters.Add(new CspMediaSrcAttribute() { Self = false });
            filters.Add(new CspStyleSrcAttribute() {
                CustomSources = string.Join(" ", "maxcdn.bootstrapcdn.com"),
                Self = true,
                UnsafeInline = true
            });
        }
    }
}
