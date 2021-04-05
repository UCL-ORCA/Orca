using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orca.Services;
using Orca.Scheduling;
using Orca.Tools;
using Orca.Services.Adapters;
using Orca.Database;
using Microsoft.Extensions.Options;
using Orca.Controllers;

namespace Orca
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SharepointSettings>(Configuration.GetSection("Orca:Sharepoint"));
            services.Configure<MSGraphSettings>(Configuration.GetSection("Orca:MsGraph"));
            services.Configure<CaliperSettings>(Configuration.GetSection("Orca:Caliper"));
            services.Configure<DatabaseFields>(Configuration.GetSection("Orca:Database"));
            services.AddSingleton<GraphHelper>();
            services.AddSingleton<IGraphHelper>(serviceFactory => serviceFactory.GetRequiredService<GraphHelper>());

            // Register the sharepoint manager
            services.AddSingleton<ISharepointManager, SharepointManager>();
            services.AddTransient<DatabaseConnect>();
            // Register the event aggregator as a service.
            services.AddSingleton<IEventAggregator, EventAggregator>();
            // Register the course catalog and the course catalog updater background task
            services.AddSingleton<SharepointCourseCatalog>(); // directly register as SharepointCourseCatalog for the CourseCatalogUpdater 
            // asking for an ICourseCatalog will give us the same registered SharepointCourseCatalog above
            services.AddSingleton<ICourseCatalog>(serviceFactory => serviceFactory.GetRequiredService<SharepointCourseCatalog>());
        
            services.AddHostedService<CourseCatalogUpdater>();
            services.AddHostedService<MsGraphSubscriptionUpdater>();

            // Register the moodle adapter
            services.AddSingleton<IIdentityResolver, MsGraphIdentityResolver>();
            services.AddSingleton<MoodleAdapter>();
            services.AddSingleton<MsGraphAdapter>();

            // Register Resourcium adapter if apiKey configured
            if (Configuration.GetValue<string>("Orca:Resourcium:ApiKey", null) != null)
            {
                services.Configure<ResourciumSettings>(Configuration.GetSection("Orca:Resourcium"));
                services.AddSingleton<ResourciumAdapter>();
            }

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orca", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<DatabaseFields> dbSettings, ILogger<Startup> logger)
        {
            if (DatabaseConnect.HasDatabase(dbSettings.Value))
            {
                logger.LogInformation("Initializing database schema");
                DatabaseConnect.CreateDatabase(dbSettings.Value, env);
            } else
            {
                logger.LogWarning("Database credentials are missing. Application will run without connecting to a database. Engagement events will not be persisted");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orca v1"));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
