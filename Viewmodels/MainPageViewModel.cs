using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Products3.Services;
using Products3.Interfaces;
using Products3.Models.SQLModels;
using System.Text.Json;
using Products3.Views.Pages;
using Products3.Models.Authentication;
using Firebase.Auth;
using Plugin.Firebase.CloudMessaging;

namespace Products3.Viewmodels
{
    [QueryProperty("UserInfo", "UserInfo")]
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly IProductsDatabase _database;
        private readonly IBackendClient _backendClient;
        private readonly FirebaseAuthClient _firebaseAuthClient;
        [ObservableProperty]
        private bool isLoading = true;

        [ObservableProperty]
        private IEnumerable<Product> productsList = [];

        [ObservableProperty]
        private bool isLocalProductListEmpty = true;

        [ObservableProperty]
        public CurrentUserModel userInfo;

        public MainPageViewModel(IProductsDatabase database, IBackendClient backendClient, FirebaseAuthClient firebaseAuthClient)
        {
            _database = database;
            _backendClient = backendClient;
            _firebaseAuthClient = firebaseAuthClient;

            MessagingService.SubscribeToUrlReceivedMessage(this, url =>
            {
                System.Diagnostics.Debug.WriteLine("url obtenido: " + url);
            });
        }
        [RelayCommand]
        public async Task OnCounterClicked()
        {
            SecureStorage.Remove("userSubcriptions");
        }

        [RelayCommand]
        public async Task OnNavigateProductDetails(string productId)
        {
            await AppShell.Current.GoToAsync(nameof(ProductDetailsPage) + $"?productId={productId}");
        }

        [RelayCommand]
        public async Task OnLogout()
        {
            _firebaseAuthClient.SignOut();
            await AppShell.Current.GoToAsync(nameof(LoginPage));
        }

        public override async Task Initialize()  {
            //await Task.Delay(10000);

            if (_firebaseAuthClient.User == null)
            {
                // Redirigir a la página principal
                await AppShell.Current.GoToAsync(nameof(LoginPage));
            }
            else
            {
                ProductsList = await _database.GetProducts();
                IsLoading = false;
                IsLocalProductListEmpty = !productsList.Any();
            }
        }


        //logica cuando agreguen un producto:
        //se le meustra en su apantalla
        //al ahcer click lo intenta guardar traer del servicio, originalmente la funcion no se va ejecutar cada que se inserte un producto por lo que se le va mostrar un vuelve mas tarde
        //donde si se mandara es la funcion para insertar en mysql la url del productor
    }
}
