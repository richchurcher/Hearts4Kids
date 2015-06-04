using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hearts4Kids.Startup))]
namespace Hearts4Kids
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
