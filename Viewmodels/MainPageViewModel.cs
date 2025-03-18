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
using Products3.States;
using Microsoft.VisualBasic;

namespace Products3.Viewmodels
{
    [QueryProperty("UserInfo", "UserInfo")]
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly IProductsDatabase _database;
        private readonly IBackendClient _backendClient;
        private readonly FirebaseAuthClient _firebaseAuthClient;
        //private readonly State _state;

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private string activityIndicatorText = "Obteniendo informacion...";
        public string ActivityIndicatorText
        {
            get => activityIndicatorText;
            set => SetProperty(ref activityIndicatorText, value);
        }

        private IEnumerable<Product> productsList = [];
        public IEnumerable<Product> ProductsList
        {
            get => productsList;
            set => SetProperty(ref productsList, value);
        }

        private bool isLocalProductListEmpty = true;
        public bool IsLocalProductListEmpty
        {
            get => isLocalProductListEmpty;
            set => SetProperty(ref isLocalProductListEmpty, value);
        }

        private CurrentUserModel? userInfo;
        public CurrentUserModel UserInfo
        {
            get => userInfo;
            set => SetProperty(ref userInfo, value);
        }


        private bool popUpVisble = false;
        public bool PopUpVisble
        {
            get => popUpVisble;
            set => SetProperty(ref popUpVisble, value);
        }

        private string dialogText = string.Empty;
        public string DialogText
        {
            get => dialogText;
            set => SetProperty(ref dialogText, value);
        }

        private string userName = string.Empty;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }
        
        public MainPageViewModel(IProductsDatabase database, IBackendClient backendClient, FirebaseAuthClient firebaseAuthClient, State state) : base(state)
        {
            _database = database;
            _backendClient = backendClient;
            _firebaseAuthClient = firebaseAuthClient;
            //_state = state;

            MessagingService.SubscribeToUrlReceivedMessage(this, url =>
            {
                System.Diagnostics.Debug.WriteLine("url obtenido: " + url);
            });

            State.CurrentUserInfo.AsObservable().Subscribe(userData =>
            {
                UserName = userData.DisplayName!;
            });
        }
        
        [RelayCommand]
        public async Task OnNavigateProductDetails(string productId)
        {
            await AppShell.Current.GoToAsync(nameof(ProductDetailsPage) + $"?productId={productId}");
        }

        [RelayCommand]
        public void OnLogout()
        {
            PopUpVisble = true;
            DialogText = "Cerrar sesion tendra los siguientes efectos: " +
                         "\n\n- Tus subscripciones a productos asi como los que sigues seran eliminados." +
                         "\n- Dejaras de recibir notificaciones.\n";
            
        }

        [RelayCommand]
        public async Task SignOutConfirmationPopUp()
        {
            //when user logs out
            ActivityIndicatorText = "Cerrando sesión";
            IsLoading = true;
            var authToken = await _backendClient.GetBackendToken();
            var userLocalData = State.CurrentUserInfo.Get();
            //Trigger a call to mongo to update/remove it's token and subcriptions  - this because you need to remove the token to prevent notificacion when user is not logged
            await Task.WhenAll(
            _backendClient.UpdateUserInfo(new Models.User.ClientUserData()
               {
                  FcmToken = string.Empty,
                  UserId = userLocalData.UserId,
                  UserSubscriptions = []
                  }, authToken),

            //Unsubcribe it from all the products as:
            //1 if new user logs in within the same device the fcm token it's not updated therefore the "new" user is the now the owner of the token
            //2 
             _backendClient.UnSubscribeToProduct(userLocalData.UserSubscriptions!.Split(",").ToList(), userLocalData.FcmToken!, authToken),

            //finally delete current user data from sqlite
             _database.DeleteUserInfo(),
             _database.RemoveAllProducts()

            //when user logs in a new userInfo state will be set so there's no need to set the state when logging out
            );


            //eventually if we get any cached solution for products like redis, user will be able to retrieve it's products when re-log in
            _firebaseAuthClient.SignOut();
            IsLoading = false;
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        [RelayCommand]
        public async Task DeclinePopUp()
        { 
            PopUpVisble = false;
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
