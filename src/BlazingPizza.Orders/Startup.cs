using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using MongoDB.Driver;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlazingPizza.Orders
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
            services.AddSingleton<IOrdersService, OrdersService>();
            services.AddControllers();
            services.AddGrpc();
            services.AddApplicationInsightsTelemetry(Configuration);

            services.ConfigureTelemetryModule<EventCounterCollectionModule>((module, options) =>
            {
                module.Counters.Add(new EventCounterCollectionRequest("BlazingOrders.Pizza", "total-orders"));
            });

            services.AddHealthChecks()
                    .AddMongoDb(Configuration["Data:Connection2"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapControllers();
                endpoints.MapGrpcService<OrderStatusService>();
            });
        }
    }
}
