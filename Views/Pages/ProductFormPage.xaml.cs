using Products3.Viewmodels;

namespace Products3.Views.Pages
{
    public partial class ProductFormPage : BasePage
    {
        private readonly ProductFormViewModel _viewModel;
        public ProductFormPage(ProductFormViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
            //viewModel.Initialize();
            //BindingContext = viewModel
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.Initialize();
        }
    }
}
