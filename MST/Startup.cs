using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MST.Domain;
using MST.Domain.Core;
using MST.EventBus.Simple;
using MST.EventHandlerContext;
using MST.EventStore.Simple;

namespace MST
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


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("正在对服务进行配置");
            services.AddLogging(cfg =>
            {
                cfg.AddConsole();
                cfg.AddConfiguration(Configuration.GetSection("Logging"));
                cfg.AddEventSourceLogger();
                cfg.AddDebug();
            });
            var eventHandlerExecutionContext =
                new EventHandlerExecutionContext(services, sc => sc.BuildServiceProvider());
            services.AddSingleton<IEventHandlerExecutionContext>(eventHandlerExecutionContext);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IEventBus, PassThroughEventBus>();
            services.AddTransient<IEventHandler, CustomerCreatedEventHandler>();
            services.AddTransient<IEventStore, SimpleEventStore>();
            _logger.LogInformation("已注册到IOC容器");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<CustomerCreatedEvent, CustomerCreatedEventHandler>();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}