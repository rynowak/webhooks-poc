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
        }

        public IConfiguration Configuration { get; }

        public FunctionsRouteTable RouteTable { get; } = new FunctionsRouteTable();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(RouteTable);
            services.AddSingleton<IActionDescriptorChangeProvider>(RouteTable);

            services
                .AddMvc(options =>
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
            app.UseMvcWithDefaultRoute();
        }
    }
}
