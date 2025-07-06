using System.IO;
using System.Reflection;
using System.Windows;
using BL.External;
using BL.Services;
using BL.TourImage;
using DAL;
using DAL.Repositories;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UI.ViewModels;

namespace UI
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<TourPlannerContext>(options =>
                        options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddTransient<IMapImageGenerator, MapImageGenerator>();
                    services.AddTransient<ITourService, TourService>();

                    services.AddScoped<ITourRepository, TourRepository>();
                    services.AddScoped<IRouteService, RouteService>();
                    services.AddScoped<IReportService, ReportService>();
                    services.AddScoped<MainViewModel>();
                })
                .Build();

            AppHost.Start();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var log = LogManager.GetLogger(typeof(App));
            log.Info("Application started.");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppHost.Dispose();
            base.OnExit(e);
        }
    }
}
