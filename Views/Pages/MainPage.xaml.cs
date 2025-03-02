using Microsoft.Azure.NotificationHubs;
using Plugin.Firebase.CloudMessaging;
using Products3.Services;
using Products3.Viewmodels;

namespace Products3.Views.Pages
{
    public partial class MainPage : BasePage
    {
        private readonly MainPageViewModel _viewModel;
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            //Console.WriteLine($"FCM token: {token}");
            //string connectionString = "Endpoint=sb://PriceTrackerNSH.servicebus.windows.net/;SharedAccessKeyName=PriceTrackerListenSendDirective;SharedAccessKey=bWpNIIIqQsWU9M9npqo01wavfm628tDyAZvom+12z1s=";
            //string hubName = "SampleNotification ";

            //var hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);

            //try
            //{
            //    // Registra el dispositivo en Azure Notification Hub
            //    //var registration = await hub.CreateFcmV1NativeRegistrationAsync(token);
            //    var testo = registration.RegistrationId;
            //}
            //catch (Exception ex)
            //{
            //    // Manejo de errores si algo falla
            //    Console.WriteLine($"Error al registrar el dispositivo: {ex.Message}");
            //}
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.Initialize();
        }
    }

}
