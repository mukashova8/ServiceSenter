using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ServiceSenter.Data;
using ServiceSenter.Views;

namespace ServiceSenter
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<AppDbContext>();


            serviceCollection.AddTransient<LoginWindow>();
            serviceCollection.AddTransient<AdminWindow>();


            ServiceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}