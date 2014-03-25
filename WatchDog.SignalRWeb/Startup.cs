using Microsoft.Owin;
using Owin;
using MyWebApplication3;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(Startup))]
namespace MyWebApplication3
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HubConfiguration hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;

            app.MapSignalR(hubConfiguration);
        }
    }
}