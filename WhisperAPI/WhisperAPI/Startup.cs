using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using WhisperAPI.Registries;
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
            services.AddMvc();

            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v2", new Info { Title = "WhisperAPI", Version = "v2" }));

            var applicationSettings = new ApplicationSettings();
            this.Configuration.Bind(applicationSettings);

            var container = new Container();

            container.Configure(config =>
            {
                config.AddRegistry(new WhisperApiRegistry(applicationSettings.ApiKey, applicationSettings.IrrelevantIntents, applicationSettings.NlpApiBaseAddress, applicationSettings.ContextLifeSpan));
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
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "WhisperAPI v2"));
            app.UseMvc();
        }
    }
}
