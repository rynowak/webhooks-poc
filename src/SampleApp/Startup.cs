using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            RouteTable = new FunctionsRouteTable();
        }

        public IConfiguration Configuration { get; }

        public FunctionsRouteTable RouteTable { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<FunctionsRouteTable>(RouteTable);
            services.AddSingleton<IActionDescriptorChangeProvider>(RouteTable);

            services.AddMvc(options =>
            {
                options.Conventions.Add(new DuplicateConvention(RouteTable));
            })
            .AddGitHubWebHooks();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
