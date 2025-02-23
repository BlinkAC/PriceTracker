using Products3.Viewmodels;

namespace Products3.Views.Pages
{
    public partial class ProductDetailsPage : BasePage
    {
        private readonly ProductDetailsViewModel _viewModel;
        public ProductDetailsPage(ProductDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        
    }
}
