using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(H4K.Web.Startup))]
namespace H4K.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureContainer(app);
        }
    }
}
