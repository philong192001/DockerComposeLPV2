using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;


namespace RabbitMQ_logstash
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

            Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Index", "name")
    .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
    {
        clientConfiguration.Username = "guest";
        clientConfiguration.Password = "guest";
        clientConfiguration.Exchange = "monitoring";
        clientConfiguration.ExchangeType = "topic";
        clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
        clientConfiguration.RouteKey = "monitoring";
        clientConfiguration.Port = 5672;
        clientConfiguration.Hostnames.Add("localhost");


        sinkConfiguration.TextFormatter = new JsonFormatter();
    }).CreateLogger();

            services.AddLogging(loggingBuilder =>
           loggingBuilder.AddSerilog(dispose: true).AddConsole());


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
