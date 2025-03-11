using Microsoft.Maui.Controls;
using Products3.Views.Pages;

namespace Products3
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ProductFormPage), typeof(ProductFormPage));
            Routing.RegisterRoute(nameof(ProductDetailsPage), typeof(ProductDetailsPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

            //CheckUserAuthentication();
        }


        //private async void CheckUserAuthentication()
        //{
        //    var authToken = await SecureStorage.GetAsync("UserId");

        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        // Redirigir a la página principal
        //        await Shell.Current.GoToAsync(nameof(MainPage));
        //    }
        //}

    }
}
