using Microsoft.Extensions.DependencyInjection;
using PopWithOAuth2Wpf.Services;
using PopWithOAuth2Wpf.Services.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PopWithOAuth2Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped((p) =>
            {
                return new GoogleOAuthConfig()
                {
                    ClientId = "719245573621-msst3cmacr0i70u4iqudkqf141qu34fh.apps.googleusercontent.com",
                    ClientSecret = "{{client_secret}}",
                    Scopes = new string[] { "https://mail.google.com/" },
                    Account = "yarimit@gmail.com"
                };
            });

            services.AddSingleton<GoogleOAuthService>();

            services.AddScoped((p) =>
            {
                return new ExchangeOAtuhConfig()
                {
                    ClientId = "76fc19fc-e57f-435a-b689-cee7e9e00e7c",
                    TenantId = "consumers",
                    Scopes = new string[] {
                        "email",
                        "offline_access",
                        "https://outlook.office.com/IMAP.AccessAsUser.All",
                        "https://outlook.office.com/POP.AccessAsUser.All",
                        "https://outlook.office.com/SMTP.Send",
                    },
                    Account = "yarimit@outlook.com"
                };
            });

            services.AddSingleton<ExchangeOAuthService>();

            services.AddSingleton<Pop3Service>();


            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }

    public class ViewModelLocator
    {
        public MainViewModel MainViewModel
            => App.ServiceProvider.GetRequiredService<MainViewModel>();
    }
}

