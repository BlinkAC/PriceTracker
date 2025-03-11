using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Extensions.Configuration;
using Plugin.Firebase.CloudMessaging;
using Products3.Interfaces;
using Products3.Models.Authentication;
using Products3.Views.Pages;

namespace Products3.Viewmodels
{
    public partial class RegisterPageViewModel : BaseViewModel
    {
        private RegisterModel? registerModel;
        private readonly IBackendClient _backendClient;
        private readonly IFirebaseAuthenticatorValidatorService _emailValidator;

        public RegisterModel RegisterModel
        {
            get => registerModel!;
            set => SetProperty(ref registerModel, value);
        }

        // register the FirebaseAuthClient
        private readonly FirebaseAuthClient _authClient;
        public RegisterPageViewModel(FirebaseAuthClient authClient, IBackendClient backendClient, IFirebaseAuthenticatorValidatorService emailValidator)
        {
            _authClient = authClient;
            _backendClient = backendClient;
            _emailValidator = emailValidator;
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
            // After verifing our model, we accept user to register an account.
            var userTransaction = await _authClient.CreateUserWithEmailAndPasswordAsync(registerModel!.Email, registerModel.Password, RegisterModel.Name);

            _authClient.User.Info.IsEmailVerified = true;
            if (userTransaction.AuthCredential != null) {
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync().ConfigureAwait(false);
                var fcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync().ConfigureAwait(false);

                await SecureStorage.SetAsync("fcmToken", fcmToken);
                await SecureStorage.SetAsync("userSubcriptions", string.Join(",", []));
                await _emailValidator.SendVerificationEmail(_authClient.User.Credential.IdToken);

                await Shell.Current.GoToAsync($"{nameof(MainPage)}",
                true,
                new Dictionary<string, object>()
                {
                { "UserInfo",
                    new CurrentUserModel {
                        UserId = registerModel.Name,
                        FullName = registerModel.Name
                    }
                }
                });
            }

            
        }
        [RelayCommand]
        public async Task GotoLoginPageAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
        public override async Task Initialize() => await Task.Delay(0);
    }
}