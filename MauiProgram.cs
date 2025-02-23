
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Products3.Interfaces;
using Products3.Services;
using Products3.Viewmodels;
using Products3.Views.Pages;
using Syncfusion.Maui.Core.Hosting;

namespace Products3
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();//ProductFormPage

            builder.Services.AddSingleton<ProductFormViewModel>();
            builder.Services.AddSingleton<ProductFormPage>();

            builder.Services.AddSingleton<ProductDetailsPage>();
            builder.Services.AddSingleton<ProductDetailsViewModel>();

            builder.Services.AddSingleton<IProductsDatabase, ProductsDatabase>(); 
            builder.Services.AddSingleton<ISqliteConnectionFactory, SqliteConnectionFactory>();
            builder.Services.AddSingleton<Interfaces.ISecureStorage, SecureStorageWrapper>();
            builder.Services.AddSingleton<IToastService, ToastService>();
            builder.Services.AddSingleton<IBackendClient, BackendClientService>();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF1cXmhLYVF+WmFZfVtgdl9GYFZVQGY/P1ZhSXxWdkdhWH5acHRRQmVeWEE=");
            RegisterHttpClient(builder);
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterHttpClient(this MauiAppBuilder builder)
        {
            var services = builder.Services;

            services.AddHttpClient<IBackendClient, BackendClientService>(httpClient => httpClient.BaseAddress = new Uri("http://192.168.100.26:8080/api/"));
                //.AddHttpMessageHandler<ValidateHeaderHandler>()
                //.AddRetryPolicy(3);

            //services.AddHttpClient<IMmpkDownloadService, MmpkDownloadService>();

            return builder;
        }
    }
}
