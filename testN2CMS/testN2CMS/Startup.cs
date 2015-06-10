using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(testN2CMS.Startup))]
namespace testN2CMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
