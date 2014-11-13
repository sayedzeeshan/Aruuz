using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Aruuz.Website.Startup))]
namespace Aruuz.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
