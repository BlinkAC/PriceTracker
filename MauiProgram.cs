
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Products3.Interfaces;
using Products3.Services;
using Products3.Viewmodels;
using Products3.Views.Pages;
using Syncfusion.Maui.Core.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.CloudMessaging;
using Firebase.Auth.Providers;
using Firebase.Auth;
using Firebase.Auth.Repository;
using Microsoft.Extensions.DependencyInjection;
using Products3.States;





#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

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
                .RegisterFirebaseServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            var assembly = typeof(MauiProgram).Assembly;
            using var stream = assembly.GetManifestResourceStream("Products3.appsettings.json");
            builder.Configuration.AddJsonStream(stream!);

            builder.Services.AddSingleton<State>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();//ProductFormPage

            builder.Services.AddSingleton<ProductFormViewModel>();
            builder.Services.AddSingleton<ProductFormPage>();

            builder.Services.AddSingleton<ProductDetailsPage>();
            builder.Services.AddSingleton<ProductDetailsViewModel>();

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginPageViewModel>();

            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddSingleton<RegisterPageViewModel>();

            builder.Services.AddSingleton<IProductsDatabase, ProductsDatabase>(); 
            builder.Services.AddSingleton<ISqliteConnectionFactory, SqliteConnectionFactory>();
            builder.Services.AddSingleton<Interfaces.ISecureStorage, SecureStorageWrapper>();
            builder.Services.AddSingleton<IToastService, ToastService>();
            builder.Services.AddSingleton<IBackendClient, BackendClientService>();
            builder.Services.AddSingleton<IUserNotification, UserNotificationService>();
            builder.Services.AddSingleton<IFirebaseAuthenticatorValidatorService, FirebaseAuthenticatorValidatorService>();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["Config:SyncFusionKey"]);
            builder.Services.AddSingleton(
              new FirebaseAuthClient(
                  new FirebaseAuthConfig()
                  {
                      ApiKey = builder.Configuration["Config:FirebaseApiKey"],
                      AuthDomain = builder.Configuration["Config:AuthDomain"],
                      Providers = new FirebaseAuthProvider[]
                      {
                          new EmailProvider()
                      },
                      UserRepository = new FileUserRepository("UserReposiroy")
                  }
            ));
            RegisterHttpClient(builder);
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterHttpClient(this MauiAppBuilder builder)
        {
            var services = builder.Services;

            services.AddHttpClient<IBackendClient, BackendClientService>(httpClient => httpClient.BaseAddress =
            new Uri(builder.Configuration["Config:BackendClientUri"]!));

            services.AddHttpClient<IFirebaseAuthenticatorValidatorService, FirebaseAuthenticatorValidatorService>(httpClient => 
            httpClient.BaseAddress = new Uri(builder.Configuration["Config:FirebaseApiUri"]!));
            //.AddHttpMessageHandler<ValidateHeaderHandler>()
            //.AddRetryPolicy(3);

            //services.AddHttpClient<IMmpkDownloadService, MmpkDownloadService>();

            return builder;
        }

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
        #if IOS
                events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) => {
                    CrossFirebase.Initialize();
                    FirebaseCloudMessagingImplementation.Initialize();
                    return false;
                }));
        #elif ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                CrossFirebase.Initialize(activity)));
        #endif
            });

            return builder;
        }


    }
}
