using Microsoft.Extensions.Logging;
using Products3.Interfaces;
using Products3.Services;
using Products3.Viewmodels;
using Products3.Views.Pages;

namespace Products3
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<IProductsDatabase, ProductsDatabase>();
            builder.Services.AddSingleton<ISqliteConnectionFactory, SqliteConnectionFactory>();
            builder.Services.AddSingleton<Interfaces.ISecureStorage, SecureStorageWrapper>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
