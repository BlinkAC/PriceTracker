using Products3.Services;
using Products3.Viewmodels;
using static System.Net.Mime.MediaTypeNames;

namespace Products3.Views.Pages
{
    public partial class MainPage : BasePage
    {
        int count = 0;

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }

}
