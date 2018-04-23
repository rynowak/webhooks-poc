using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleApp
{
    public class Startup
    {
        private readonly FunctionsRouteTable _routeTable = new FunctionsRouteTable();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_routeTable);
            services.AddSingleton<IActionDescriptorChangeProvider>(_routeTable);

            services
                .AddMvc(options =>
                {
                    options.Conventions.Add(new DuplicateConvention(_routeTable));
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
            app.UseMvcWithDefaultRoute();
        }
    }
}
