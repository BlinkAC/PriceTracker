using Products3.Viewmodels;

namespace Products3.Views.Pages
{
    public partial class LoginPage : BasePage
    {
        public LoginPage(LoginPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

        }

        protected override bool OnBackButtonPressed()
        {
            // Simplemente retorna true para desactivar el botón de retroceso
            return true;
        }
    }
}
