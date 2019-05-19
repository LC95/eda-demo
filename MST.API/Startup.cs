using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MST.Domain;
using MST.Domain.Abstraction.Events;
using MST.Domain.EventHandlers;
using MST.Domain.Events;
using MST.EventBus.RabbitMQ;
using MST.EventHandlerContext.Simple;
using MST.EventStore.Simple;


namespace MST.API
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, true);
            builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            _logger = logger;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("正在对服务进行配置");
            services.AddMvc();
            services.AddSingleton<IEventHandlerExecutionContext>(new EventHandlerExecutionContext(services, s => s.BuildServiceProvider()));
            services.AddRabbitMQEventBus("Exchange", "AddTest");
            services.AddTransient<IEventStore, SimpleEventStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<AddCustomerEvent, AddCustomerEventHandler>();
            app.UseMvc();
        }
    }
}