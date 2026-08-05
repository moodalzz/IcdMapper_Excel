using IcdMapper_Excel.Services;
using IcdMapper_Excel.Services.Interfaces;
using IcdMapper_Excel.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;


namespace IcdMapper_Excel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<IExcelReaderService, ExcelReaderService>();
            services.AddSingleton<IJsonExportService, JsonExportService>();
            services.AddSingleton<IMappingProfileService, MappingProfileService>();

            services.AddTransient<ColumnMappingViewModel>();
            services.AddTransient<IcdMapperViewModel>();
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _serviceProvider.GetRequiredService<MainWindow>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);

        }
    }
}
