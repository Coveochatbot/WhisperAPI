using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        public void ConfigureServices(IServiceCollection services)
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

            var applicationSettings = new ApplicationSettings();
            this.Configuration.Bind(applicationSettings);

            ConfigureDependancy(services, applicationSettings);
        }

        private static void ConfigureDependancy(IServiceCollection services, ApplicationSettings applicationSettings)
        {
            services.AddTransient<ISuggestionsService>(
                x => new SuggestionsService(
                    x.GetService<IIndexSearch>(),
                    x.GetService<INlpCall>(),
                    applicationSettings.IrrelevantIntents));

            services.AddTransient<INlpCall>(
                x => new NlpCall(
                    x.GetService<HttpClient>(),
                    applicationSettings.NlpApiBaseAddress));

            services.AddTransient<IIndexSearch>(
                x => new IndexSearch(
                    applicationSettings.ApiKey,
                    x.GetService<HttpClient>()));

            services.AddTransient<HttpClient, HttpClient>();

            services.AddSingleton<IContexts>(
                x => new InMemoryContexts(
                    TimeSpan.Parse(applicationSettings.ContextLifeSpan)));
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
