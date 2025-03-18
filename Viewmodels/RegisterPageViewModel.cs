using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using Plugin.Firebase.CloudMessaging;
using Products3.Interfaces;
using Products3.Models.Authentication;
using Products3.Models.User;
using Products3.Views.Pages;
using Products3.States;
namespace Products3.Viewmodels
{
    public partial class RegisterPageViewModel : BaseViewModel
    {
        private RegisterModel? registerModel;
        private readonly IBackendClient _backendClient;
        private readonly IFirebaseAuthenticatorValidatorService _emailValidator;
        private readonly IProductsDatabase _database;
        //private readonly State _state;
        public RegisterModel RegisterModel
        {
            get => registerModel!;
            set => SetProperty(ref registerModel, value);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private string activityIndicatorText = "Registrando usuario";
        public string ActivityIndicatorText
        {
            get => activityIndicatorText;
            set => SetProperty(ref activityIndicatorText, value);
        }

        // register the FirebaseAuthClient
        private readonly FirebaseAuthClient _authClient;
        public RegisterPageViewModel(FirebaseAuthClient authClient, IBackendClient backendClient, IFirebaseAuthenticatorValidatorService emailValidator, IProductsDatabase database, State state) : base(state)
        {
            _authClient = authClient;
            _backendClient = backendClient;
            _emailValidator = emailValidator;
            _database = database;
            //_state = state;
            RegisterModel = new RegisterModel();
        }


        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (RegisterModel.HasErrors)
            {
                var validationErrors = registerModel!.GetErrors().ToArray();
                for (int i = 0; i < validationErrors.Count(); i++)
                {
                    await Shell.Current.DisplayAlert("Error", validationErrors[i].ToString(), "Ok");
                }
                return;
            }
            IsLoading = true;
            // After verifing our model, we accept user to register an account.
            var userTransaction = await _authClient.CreateUserWithEmailAndPasswordAsync(registerModel!.Email, registerModel.Password, RegisterModel.Name);
            var authtoken = await _backendClient.GetBackendToken();
            //_authClient.User.Info.IsEmailVerified = true;
            if (userTransaction.AuthCredential != null) {
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync().ConfigureAwait(false);
                var fcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                var userFirebaseToken = _authClient.User.Credential.IdToken;


                await Task.WhenAll( _emailValidator.SendVerificationEmail(userFirebaseToken),
                    _backendClient.CheckUserInfo(new ClientUserData()
                    {
                        FcmToken = fcmToken,
                        UserId = userTransaction.User.Uid,
                        UserSubscriptions = []
                    }, authtoken));
                
                await _database.SaveUserInfo(new UserLocalData()
                {
                    DisplayName = registerModel.Name,
                    FcmToken = fcmToken,
                    UserId = userTransaction.User.Uid,
                    IsPremiumUser = 0,
                    UserSubscriptions = string.Join(",", [])
                });

                State.CurrentUserInfo.Set(await _database.GetUserInfo());
                IsLoading = false;
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");

            }

            
        }
        [RelayCommand]
        public async Task GotoLoginPageAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        public override async Task Initialize() => await Task.Delay(0);
    }
}