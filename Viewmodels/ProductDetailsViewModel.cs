using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Products3.Interfaces;
using Products3.Models.SQLModels;
using Syncfusion.Maui.Core.Carousel;

namespace Products3.Viewmodels
{
    public partial class ProductDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IProductsDatabase _database;
        private readonly IBackendClient _backendClient;
        private readonly IToastService _toastService;

        [ObservableProperty]
        private string chartTitle = "Historial de precios para:\n";

        [ObservableProperty]
        private string productImage;

        [ObservableProperty]
        private double currentPrice;

        [ObservableProperty]
        private double highestPrice;

        [ObservableProperty]
        private double lowestPrice;

        [ObservableProperty]
        private DateTime lastUpdateDate;

        [ObservableProperty]
        private string productId;

        [ObservableProperty]
        public IEnumerable<ProductHistory> data = [];

        public ProductDetailsViewModel(IProductsDatabase database, 
                                       IBackendClient backendClient,
                                       IToastService toastService)
        {
            _database = database;
            _backendClient = backendClient;
            _toastService = toastService;
        }

        [RelayCommand]
        public async Task OnStopFollowingProduct()
        {
            var ids = new List<string>()
            {
                ProductId
            };
            var rowsAffected = await _database.RemoveProducts(ids);
            if(rowsAffected > 0)
            {
                await _toastService.ShowToast($"Haz dejado de seguir {ProductId}");
                await Shell.Current.GoToAsync("..");
            } else
            {
                await _toastService.ShowToast($"Ocurrio un error al intentar eliminar {ProductId}");
                await Shell.Current.GoToAsync("..");
            }

        }

        //get triggered every time ProductId is updated
        //partial void OnProductIdChanged(string value)
        //{
        //    // Lógica adicional cuando Id cambia
        //    RealizarLlamadaServicioAsync(value);
        //}

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("productId"))
            {
                ProductId = query["productId"] as string;
                await Initialize();
            }
        }
        public override async Task Initialize()
        {
            if (ProductId != null) {
                var resposne = await _backendClient.GetProducHistory(ProductId);
                var content = resposne.Content.ReadAsStringAsync().Result.ToString();
                var product = JsonSerializer.Deserialize<ProductRecord>(content);


                ProductImage = product.ProductImage;
                ChartTitle += product.ProductId;
                CurrentPrice = product.ProductCurrentPrice;
                HighestPrice = product.ProductHighestPrice;
                LowestPrice = product.ProductLowestPrice;
                LastUpdateDate = product.LastUpdateDate;

                Data = product.ProductHistory;
            }
        }
    }
}
