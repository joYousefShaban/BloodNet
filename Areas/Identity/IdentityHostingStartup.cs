[assembly: HostingStartup(typeof(BloodNet.Areas.Identity.IdentityHostingStartup))]
namespace BloodNet.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}