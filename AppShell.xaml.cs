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

        protected override bool OnBackButtonPressed()
        {
            // Obtén la página actual en la que el usuario se encuentra
            var currentPage = Shell.Current?.CurrentPage;

            // Si estamos en la página de registro (RegisterPage) o login (LoginPage)
            if (currentPage is RegisterPage || currentPage is LoginPage)
            {
                // Regresar a LoginPage, que es la página principal cuando el usuario no está autenticado
                Shell.Current.GoToAsync("//LoginPage");
                return true; // Prevenir el comportamiento predeterminado (cerrar la app)
            }

            // Si no estamos en LoginPage o RegisterPage, entonces se maneja la navegación estándar
            return base.OnBackButtonPressed();
        }


    }
}
