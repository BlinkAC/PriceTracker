using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Products3.Services;
using Products3.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using Products3.Models.SQLModels;
using System.Text.Json;

namespace Products3.Viewmodels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly IProductsDatabase _database;
        private readonly IBackendClient _backendClient;
        [ObservableProperty]
        private string rata = "alooooooooooooooooo";

        [ObservableProperty]
        private IEnumerable<Product> productsList = [];

        public MainPageViewModel(IProductsDatabase database, IBackendClient backendClient)
        {
            _database = database;
            _backendClient = backendClient;

            MessagingService.SubscribeToUrlReceivedMessage(this, url =>
            {
                Rata = url; // Mostrar la URL en la etiqueta
                System.Diagnostics.Debug.WriteLine("url obtenido: " + url);
            });
        }

        [RelayCommand]
        public async Task OnButton1Clicked()
        {
            var resposne = await _backendClient.GetProducHistory();
            var content = resposne.Content.ReadAsStringAsync().Result.ToString();
            Rata = JsonSerializer.Deserialize<ProductRecord>(content).ProductId;

        }
        public override async Task Initialize()  {

            ProductsList = await _database.GetProducts();
        }
        //logica cuando agreguen un producto:
        //se le meustra en su apantalla
        //al ahcer click lo intenta guardar traer del servicio, originalmente la funcion no se va ejecutar cada que se inserte un producto por lo que se le va mostrar un vuelve mas tarde
        //donde si se mandara es la funcion para insertar en mysql la url del productor
    }
}
