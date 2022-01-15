using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MathCompetition.Startup))]
namespace MathCompetition
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
