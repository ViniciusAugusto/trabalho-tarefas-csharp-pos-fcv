using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tarefas.Startup))]
namespace Tarefas
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // fazemos o registro do signalR
            app.MapSignalR();

            ConfigureAuth(app);
        }
    }
}
