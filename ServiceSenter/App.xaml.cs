using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ServiceSenter.Data;
using ServiceSenter.Views;

namespace ServiceSenter
{
    /// <summary>
    /// класс для доступа к окнам и бд
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            //регистрация бд 
            serviceCollection.AddDbContext<AppDbContext>();

            //регистрация окон
            serviceCollection.AddTransient<LoginWindow>();
            serviceCollection.AddTransient<AdminWindow>();


            ServiceProvider = serviceCollection.BuildServiceProvider();

            //содание бд, если ее нет
            using (var scope = ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            //открытие окна авторизации
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}