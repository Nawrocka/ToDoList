using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ToDoList1.Startup))]
namespace ToDoList1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
