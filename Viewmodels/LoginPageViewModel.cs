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

namespace Products3.Viewmodels
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        private MailLoginModel loginModel;
        private readonly FirebaseAuthClient _authClient;
        private readonly IBackendClient _backendClient;
        private string _token { get; set; } = string.Empty;
        public MailLoginModel LoginModel
        {
            get => loginModel;
            set => SetProperty(ref loginModel, value);
        }

        public LoginPageViewModel(
            FirebaseAuthClient authClient,
            IBackendClient backendClient
            )
        {
            LoginModel = new MailLoginModel();
            _authClient = authClient;
            _backendClient = backendClient;
        }


        [RelayCommand]
        public async Task GotoRegisterPageAsync()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
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

                var deviceFcmToken = await SecureStorage.GetAsync("fcmToken").ConfigureAwait(false);
                if (deviceFcmToken == null)
                {
                    await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync().ConfigureAwait(false);
                    _token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync().ConfigureAwait(false);
                    await SecureStorage.SetAsync("fcmToken", _token).ConfigureAwait(false);
                }
                _token = deviceFcmToken;
                var authToken = await _backendClient.GetBackendToken().ConfigureAwait(false);
                //User logs in
                //if we dont have a fcmToken we generate one and try to fetch suer data from or DB
                var remoteUserData = await _backendClient.CheckUserInfo(new ClientUserData()
                {
                    UserId = user.Uid,
                    FcmToken = _token,
                    UserSubscriptions = []
                }, authToken).ConfigureAwait(false);

                //if it's a new login this code won't get executed it's for the next loging even of other devices
                // if we generated a fcmToken and we got user from db it's like to be a different installation
                //check if the fcmToken it's different if so:
                //update the token and delete the subscriptions (at least right now as we only support free users)
                var userData = JsonConvert.DeserializeObject<UserData>(await remoteUserData.Content.ReadAsStringAsync().ConfigureAwait(true));
                if (userData!.Details!.FcmToken != _token)
                {
                    var updateUser = new ClientUserData() {FcmToken = _token, UserId = userData.Details.UserId, UserSubscriptions = [] };
                    await _backendClient.UpdateUserInfo(updateUser, authToken).ConfigureAwait(false);
                    await _backendClient.UnSubscribeToProduct(userData.Details.UserSubscriptions.ToList()!, userData.Details.FcmToken!, authToken).ConfigureAwait(false);
                }

                //if it's a login on same device then just keep record of it's subcription as it's only allowed to subcribe to 3 products
                string subscriptions = userData.Details.UserSubscriptions?.Any() == true
                                        ? string.Join(",", userData.Details.UserSubscriptions.Where(x => !string.IsNullOrWhiteSpace(x.ToString())))
                                        : string.Empty;

                await SecureStorage.SetAsync("userSubcriptions", subscriptions); ;

                //you wil always get to this page driven by another
                //therefore when navigating back to main page you need
                //to do it in the main thread
                Application.Current?.Dispatcher.Dispatch(async() =>
                {
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
            catch (FirebaseAuthHttpException ex)
            {
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
