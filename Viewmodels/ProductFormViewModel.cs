using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Products3.Interfaces;
using Products3.Services;

namespace Products3.Viewmodels
{
    public partial class ProductFormViewModel : BaseViewModel
    {
        private readonly IProductsDatabase _database;
        private readonly IToastService _toastService;
        private readonly IBackendClient _backendClient;

        [ObservableProperty]
        private string productURL = string.Empty;

        [ObservableProperty]
        private string productId = string.Empty;

        [ObservableProperty]
        private string productName = string.Empty;

        public ProductFormViewModel(IProductsDatabase database, IToastService toastService,IBackendClient backendClient)
        {
            _database = database;
            _toastService = toastService;
            _backendClient = backendClient;

            MessagingService.SubscribeToUrlReceivedMessage(this, url =>
            {
                var productData = url.Split(",");
                ProductId = productData[1];
                ProductName = productData[0];
                ProductURL = productData[2];

                System.Diagnostics.Debug.WriteLine("url obtenido: " + url);
            });
        }

            [RelayCommand]
        public async Task OnSaveProductButton()
        {

            var token = await _backendClient.GetBackendToken();
            var checkProductTask = await _backendClient.CheckProductAvailability(ProductId, ProductURL, "ML", token);

            if (checkProductTask.IsSuccessStatusCode)
            {

                var result = await _database.AddProduct(
                new Models.SQLModels.Product()
                {
                    ProductId = ProductId,
                    ProductName = ProductName,
                    ProductUrl = ProductURL
                });
                // Mostrar notificación
                await _toastService.ShowToast("Producto guardado correctamente", CommunityToolkit.Maui.Core.ToastDuration.Short);
                // Verificar la disponibilidad del producto
                

                // Esperar a que todas las tareas asincrónicas se completen

                // Navegar a la página anterior
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await _toastService.ShowToast("Hubo un error al guardar el producto. \nIntenta mas tarde.", CommunityToolkit.Maui.Core.ToastDuration.Short);
                await Shell.Current.GoToAsync("..");
            }
            
        }

        public override async Task Initialize() => await Task.Delay(0);
    }
}

//https://www.amazon.com.mx/GIGABYTE-Tarjeta-Ventiladores-WINDFORCE-GV-N407SEAGLE/dp/B0CSJYJP6M/?_encoding=UTF8&pd_rd_w=g7pUE&content-id=amzn1.sym.708fce32-82a4-4ef4-b3cd-abf53ddfc63d%3Aamzn1.symc.abfa8731-fff2-4177-9d31-bf48857c2263&pf_rd_p=708fce32-82a4-4ef4-b3cd-abf53ddfc63d&pf_rd_r=2VB47WMA69FNZJKVCTTP&pd_rd_wg=Sb6PW&pd_rd_r=72a72382-6c79-4c51-ae12-fb68784cf59d&ref_=pd_hp_d_btf_ci_mcx_mr_ca_id_hp_d