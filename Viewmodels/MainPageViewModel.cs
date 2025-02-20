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

namespace Products3.Viewmodels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly IProductsDatabase _database;

        [ObservableProperty]
        private string rata = "alooooooooooooooooo";

        public MainPageViewModel(IProductsDatabase database)
        {
            _database = database;
            MessagingService.SubscribeToUrlReceivedMessage(this, url =>
            {
                Rata = url; // Mostrar la URL en la etiqueta
                System.Diagnostics.Debug.WriteLine("url obtenido: " + url);
                //SemanticScreenReader.Announce(CounterBtn.Text);
            });
        }

        public ICommand Button1Command { get; }

        [RelayCommand]
        public async Task OnButton1Clicked()
        {
            var  urls = await _database.GetProducts();
            foreach (var url in urls)
            {
                System.Diagnostics.Debug.WriteLine($"heyu {url.ProductId} - {url.ProductId}");
            }
            //await _database.AddProduct(new Models.SQLModels.Product() { ProductId = "123", ProductUrl = Rata });

            //reg eexps
            //mercado libre Ñ ([A-Za-z]+(\.[A-Za-z]+)+)
            //nombre producto ([A-Za-z0-9]+(-[A-Za-z0-9]+)+)
            //const Regex regex = new Regex("^.*[A-Za-z0-9]+.*([A-Za-z]+(-[A-Za-z]+)+).*[A-Za-z]+.*$", RegexOptions.IgnoreCase);
        }
        public override async Task Initialize() => await Task.Delay(0);
    }
}
