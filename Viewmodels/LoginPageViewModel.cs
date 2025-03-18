using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Newtonsoft.Json;
using Plugin.Firebase.CloudMessaging;
using PriceTracker.API.Models;
using Products3.Interfaces;
using Products3.Models.Authentication;
using Products3.Models.User;
using Products3.Views.Pages;
using Products3.States;

namespace Products3.Viewmodels
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        private MailLoginModel loginModel;
        private readonly FirebaseAuthClient _authClient;
        private readonly IBackendClient _backendClient;
        private readonly IProductsDatabase _database;
        //private readonly State _state;
        private string _token { get; set; } = string.Empty;
        public MailLoginModel LoginModel
        {
            get => loginModel;
            set => SetProperty(ref loginModel, value);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private string activityIndicatorText = "Iniciando sesión";
        public string ActivityIndicatorText
        {
            get => activityIndicatorText;
            set => SetProperty(ref activityIndicatorText, value);
        }

        public LoginPageViewModel(
            FirebaseAuthClient authClient,
            IBackendClient backendClient,
            IProductsDatabase database,
            State state
            ) : base( state )
        {
            LoginModel = new MailLoginModel();
            _authClient = authClient;
            _backendClient = backendClient;
            _database = database;
            //_state = state;
        }


        [RelayCommand]
        public async Task GotoRegisterPageAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
        }

        [RelayCommand]
        async Task GotoHomePageAsync()
        {
            if (LoginModel.HasErrors)
            {
                var validationErrors = LoginModel.GetErrors().ToArray();
                for (int i = 0; i < validationErrors.Count(); i++)
                {
                    await Shell.Current.DisplayAlert("Error", validationErrors[i].ToString(), "Ok");
                }
                return;
            }
            try
            {
                await _authClient.SignInWithEmailAndPasswordAsync(LoginModel.Email, LoginModel.Password);

                var user = _authClient.User;
                if (user == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Email/Password is incorrect", "Ok");
                    return;
                }
                IsLoading = true;
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync().ConfigureAwait(false);
                _token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync().ConfigureAwait(false);
                    //user cierra sesion - PENSANDO QUE TODOS SON FREE DE MOMENTO
                    // dispara una llamad apara quitar el token de su data en mongo - EL NUEVO USUARIO QUE SE LOGGEA USARA EL TOKEN EN SU LUGAR Y ESTO TAMBIEN EVITA QUE SI LA SESION ESTA CERRADA RECIBA NOTIFICAIONES
                    // a los productos a los que esta suscrito quitar el token -  COSMOS
                    // elimina el contador de suscripciones (secure storage) y elimina suscripciones de mongo
                    //implementar un estado de usuario que tenga el token, el contador, el nombre, is premium - DONE

                var authToken = await _backendClient.GetBackendToken().ConfigureAwait(false);
                //User logs in
                //if we dont have a fcmToken we generate one and try to fetch user data from or DB
                var remoteUserData = await _backendClient.CheckUserInfo(new ClientUserData()
                {
                    UserId = user.Uid,
                    FcmToken = _token,
                    UserSubscriptions = []
                }, authToken).ConfigureAwait(false);

                //if it's a new registration this code won't get executed it's for the next loging even of other devices
                // if we generated a fcmToken and we got user from db it's likely to be a different/new installation
                //check if the fcmToken it's different if so:
                //update the token and delete the subscriptions (at least right now as we only support free users)
                var userData = JsonConvert.DeserializeObject<UserData>(await remoteUserData.Content.ReadAsStringAsync());
                if (userData!.Details!.FcmToken != _token)
                {
                    var updateUser = new ClientUserData() {FcmToken = _token, UserId = userData.Details.UserId, UserSubscriptions = [] };
                    await Task.WhenAll(
                        _backendClient.UpdateUserInfo(updateUser, authToken),
                        _backendClient.UnSubscribeToProduct(userData.Details.UserSubscriptions.ToList(), userData.Details.FcmToken!, authToken)
                    );

                    await _database.SaveUserInfo(new UserLocalData()
                    {
                        UserId = user.Uid,
                        FcmToken = _token,
                        IsPremiumUser = 0,
                        DisplayName = _authClient.User.Info.DisplayName,
                        UserSubscriptions = string.Join(",", [])
                    });

                } else
                {
                    //the same and original user has logged again
                    await _database.SaveUserInfo(new UserLocalData()
                    {
                        UserId = user.Uid,
                        FcmToken = userData.Details.FcmToken,
                        IsPremiumUser = 0,
                        DisplayName = _authClient.User.Info.DisplayName,
                        //later on here you have to manage users subscription/follows for common & premium users
                        UserSubscriptions = string.Join(",", [])
                    });
                    
                }
                var userino = await _database.GetUserInfo();
                State.CurrentUserInfo.Set(userino!);


                //you wil always get to this page driven by another
                //therefore when navigating back to main page you need
                //to do it in the main thread
                Application.Current?.Dispatcher.Dispatch(async() =>
                {
                    IsLoading = false;
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}",
                        true,
                        new Dictionary<string, object>()
                        {
                            { "UserInfo",
                                new CurrentUserModel {
                                    UserId = user.Uid,
                                    FullName =user.Info.DisplayName
                                }
                            }
                        }
                    );
                });
            }
            catch (NullReferenceException ex)
            {
                IsLoading = false;
                // Aquí puedes imprimir la pila de llamadas y el mensaje para ver más detalles.
                Console.WriteLine($"Se ha producido una excepción: {ex.Message}");
                Console.WriteLine($"Pila de llamadas: {ex.StackTrace}");

                // Si quieres especificar el objeto que es null, puedes hacerlo así:
                Console.WriteLine("Error al intentar asignar CurrentUserInfo: El objeto _state o CurrentUserInfo podría ser null.");
            }
            catch (FirebaseAuthHttpException ex)
            {
                IsLoading = false;
                Console.WriteLine(ex.Message);
            }


        }
        //every time user logs in
        //1.check if we have user data
        //if not generate his token 
        //2 check if his token is different from the one registered
        // it means the installation is new therefore delete his subcriptions
        public override async Task Initialize() => await Task.Delay(0);
    }
}
