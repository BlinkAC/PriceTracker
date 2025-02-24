using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Azure.NotificationHubs;
using Plugin.Firebase.CloudMessaging;
using Products3.Platforms.Android;
using Products3.Services;
using Products3.Views.Pages;

namespace Products3
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = [Intent.CategoryDefault], DataMimeType = "text/plain")]
    public class MainActivity : MauiAppCompatActivity
    {
        private Intent _initialIntent;
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine("Se lanzo el main activity");
            ShareHandler.Initialize(new AndroidShareHandler());
            HandleIntent(Intent);
            CreateNotificationChannelIfNeeded();

            _initialIntent = Intent;
        }
        private bool HandleIntent(Intent intent)
        {
            if (intent != null && intent.Action == Intent.ActionSend && intent.Type == "text/plain")
            {
                System.Diagnostics.Debug.WriteLine("Intent recibido en HandleIntent");

                return ShareHandler.HandleShare(intent);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Intent nulo o no es ActionSend");
                return ShareHandler.HandleShare(intent!);
            }
        }

        private void CreateNotificationChannelIfNeeded()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
            }
        }

        private void CreateNotificationChannel()
        {
            var channelId = $"{PackageName}.general";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = channelId;
        }

        protected override void OnResume()
        {

            base.OnResume();
            // Manejar el intent
            HandleIntent(_initialIntent);

        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            _initialIntent = intent;

            if (HandleIntent(intent))
            {
                //Redirect to form page if data was extracted sucessfully
                FirebaseCloudMessagingImplementation.OnNewIntent(_initialIntent);
                Shell.Current.GoToAsync(nameof(ProductFormPage));
            }
            
        }
    }
}
