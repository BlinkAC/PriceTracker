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
            _viewModel = viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Liberar recursos o desuscribir eventos si es necesario
           // _viewModel.Reset();
        }

    }
}
