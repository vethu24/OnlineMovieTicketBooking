using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieTicketBooking.Startup))]
namespace MovieTicketBooking
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
