using LogServer.Report;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LogServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<Statistics>()
                .AddHostedService<Printer>()
                .Configure<KestrelServerOptions>(options =>
                    {
                        options.Limits.MaxRequestBodySize = int.MaxValue;
                    })
                .Configure<IISServerOptions>(options =>
                    {
                        options.MaxRequestBodySize = int.MaxValue;
                    })
                .AddControllers(configure =>
                    {
                        configure.InputFormatters.Insert(0, new RawJsonBodyInputFormatter());
                    });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
