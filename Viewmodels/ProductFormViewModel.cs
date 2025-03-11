using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Products3.Interfaces;
using Products3.Services;
using Products3.Views.Pages;
using SQLite;

namespace Products3.Viewmodels
{
    public partial class ProductFormViewModel : BaseViewModel
    {
        private readonly IProductsDatabase _database;
        private readonly IToastService _toastService;
        private readonly IBackendClient _backendClient;
        private readonly IUserNotification _notificationService;
        private readonly FirebaseAuthClient _firebaseAuthClient;

        [ObservableProperty]
        private string productURL = string.Empty;

        [ObservableProperty]
        private string productId = string.Empty;

        [ObservableProperty]
        private string productName = string.Empty;

        public ProductFormViewModel(IProductsDatabase database, IToastService toastService,IBackendClient backendClient, IUserNotification notificationService, FirebaseAuthClient firebaseAuthClient)
        {
            _database = database;
            _toastService = toastService;
            _backendClient = backendClient;
            _notificationService = notificationService;
            _firebaseAuthClient = firebaseAuthClient;
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
            try
            {
                var token = await _backendClient.GetBackendToken();
                var checkProductTask = await _backendClient.CheckProductAvailability(ProductId, ProductURL, "ML", token);

                checkProductTask.EnsureSuccessStatusCode();
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
                    await _notificationService.HandleToastNavigationAsync("Producto guardado correctamente",
                                                                            CommunityToolkit.Maui.Core.ToastDuration.Short,
                                                                            $"//{nameof(MainPage)}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await _notificationService.HandleToastNavigationAsync("Hubo un error al guardar el producto. \nIntenta mas tarde.",
                                             CommunityToolkit.Maui.Core.ToastDuration.Short,
                                             $"//{nameof(MainPage)}");
            }
            catch (SQLiteException sqliteEx)
            {
                if(sqliteEx.Message.Contains("UNIQUE constraint failed"))
                {
                    await _notificationService.HandleToastNavigationAsync("No puedes agregar el mismo producto a tu lista",
                                                             CommunityToolkit.Maui.Core.ToastDuration.Short,
                                                             $"//{nameof(MainPage)}");
                }
                
            }


        }

        public override async Task Initialize()
        {
            if(_firebaseAuthClient.User.Uid == null)
            {
                await _notificationService.HandleToastNavigationAsync("Debes iniciar sesion para agregar productos",
                                                             CommunityToolkit.Maui.Core.ToastDuration.Short,
                                                             $"//{nameof(MainPage)}");
            }
        }
    }
}

//https://www.amazon.com.mx/GIGABYTE-Tarjeta-Ventiladores-WINDFORCE-GV-N407SEAGLE/dp/B0CSJYJP6M/?_encoding=UTF8&pd_rd_w=g7pUE&content-id=amzn1.sym.708fce32-82a4-4ef4-b3cd-abf53ddfc63d%3Aamzn1.symc.abfa8731-fff2-4177-9d31-bf48857c2263&pf_rd_p=708fce32-82a4-4ef4-b3cd-abf53ddfc63d&pf_rd_r=2VB47WMA69FNZJKVCTTP&pd_rd_wg=Sb6PW&pd_rd_r=72a72382-6c79-4c51-ae12-fb68784cf59d&ref_=pd_hp_d_btf_ci_mcx_mr_ca_id_hp_d