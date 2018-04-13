using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using WhisperAPI.Registries;
using WhisperAPI.Services;
using WhisperAPI.Settings;

namespace WhisperAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
            services.AddDbContext<Contexts>(options => options.UseInMemoryDatabase("contextDB"));
            services.AddMvc();

            var applicationSettings = new ApplicationSettings();
            this.Configuration.Bind(applicationSettings);

            var container = new Container();

            container.Configure(config =>
            {
                config.AddRegistry(new WhisperApiRegistry(applicationSettings.ApiKey, applicationSettings.IrrelevantsIntents, applicationSettings.NlpApiBaseAddress, applicationSettings.ContextLifeSpan));
                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            loggerFactory.AddLog4Net();

            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
