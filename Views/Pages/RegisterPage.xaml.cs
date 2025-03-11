using Products3.Viewmodels;

namespace Products3.Views.Pages
{
    public partial class RegisterPage : BasePage
    {
        public RegisterPage(RegisterPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
