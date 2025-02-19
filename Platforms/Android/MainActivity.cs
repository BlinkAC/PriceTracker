using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Products3.Platforms.Android;
using Products3.Services;

namespace Products3
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = [Intent.CategoryDefault], DataMimeType = "text/plain")]
    public class MainActivity : MauiAppCompatActivity
    {
        private Intent _initialIntent;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine("Se lanzo el main activity");
            ShareHandler.Initialize(new AndroidShareHandler());

            // Manejar el intent
            //HandleIntent(Intent);
            //var sharedText = Intent.Extras.GetString(Intent.ExtraText);
            //if (Intent?.Action == Intent.Action && Intent.Type == "text/plain")
            //{
            //    var data = Intent?.ClipData?.GetItemAt(0);
            //    var text = data.Text;
            //    System.Diagnostics.Debug.WriteLine("url obtenido: "+ text);
            //}

            //if (Uri.IsWellFormedUriString(sharedText, UriKind.Absolute))
            //{
            //    // Manejar la URL compartida
            //}
            _initialIntent = Intent;
        }
        private void HandleIntent(Intent intent)
        {
            if (intent != null && intent.Action == Intent.ActionSend && intent.Type == "text/plain")
            {
                System.Diagnostics.Debug.WriteLine("Intent recibido en HandleIntent");
                ShareHandler.HandleShare(intent);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Intent nulo o no es ActionSend");
            }
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
            HandleIntent(intent);
        }
    }
}
