using System.Text;

namespace H4K.Core.Services.Robots
{
    public sealed class RobotsService : IRobotsService
    {
        public string GetRobotsText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("user-agent: *");

            // Tell all robots not to index any directories.
            // stringBuilder.AppendLine("disallow: /");

            // Tell all robots not to index everything under the following directory.
            // stringBuilder.AppendLine("disallow: /SomeRelativePath");

            // Tell all robots to to index any of the error pages.
            stringBuilder.AppendLine("disallow: /error/");

            // Tell all robots they can visit everything under the following sub-directory, even if the parent 
            // directory is disallowed.
            // stringBuilder.AppendLine("allow: /SomeRelativePath/SomeSubDirectory");

            // SECURITY ALERT - BE CAREFUL WHAT YOU ADD HERE
            // The line below stops all robots from indexing the following secret folder. For example, this could be 
            // your Elmah error logs. Very useful to any hacker. You should be securing these pages using some form of 
            // authentication but hiding where these things are can help through a bit of security through obscurity.
            // stringBuilder.AppendLine("disallow: /MySecretStuff");

            // Add a link to the sitemap. Unfortunately this must be an absolute URL. 
            //stringBuilder.Append("sitemap: ");
            //stringBuilder.AppendLine(this._urlHelper.AbsoluteRouteUrl(HomeControllerRoute.GetSitemapXml).TrimEnd('/'));

            return stringBuilder.ToString();
        }
    }
}