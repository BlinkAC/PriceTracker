
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Products3.Interfaces;
using Products3.Models.SQLModels;
using SQLite;

namespace Products3.Viewmodels
{
    public partial class ProductDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IProductsDatabase _database;
        private readonly IBackendClient _backendClient;
        private readonly IToastService _toastService;

        [ObservableProperty]
        private string chartTitle = string.Empty;

        [ObservableProperty]
        private string productImage = string.Empty;

        [ObservableProperty]
        private double currentPrice = 0;

        [ObservableProperty]
        private double highestPrice = 0;

        [ObservableProperty]
        private double lowestPrice = 0;

        [ObservableProperty]
        private DateTime lastUpdateDate;

        [ObservableProperty]
        private string productId = string.Empty;

        [ObservableProperty]
        private string productUrl = string.Empty;

        [ObservableProperty]
        private bool isFollowingProduct = false;

        [ObservableProperty]
        public IEnumerable<ProductHistory> data = [];

        [ObservableProperty]
        public bool popUpVisble = false;

        [ObservableProperty]
        public bool isLoading = true;

        [ObservableProperty]
        public double chartMaxvalue;

        [ObservableProperty]
        public double chartMinvalue;

        public ProductDetailsViewModel(IProductsDatabase database, 
                                       IBackendClient backendClient,
                                       IToastService toastService)
        {
            _database = database;
            _backendClient = backendClient;
            _toastService = toastService;
        }
        public override void Reset()
        {
            IsLoading = true;
            ChartTitle = string.Empty;
            ProductImage = string.Empty;
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
                await _toastService.ShowToast($"Haz dejado de seguir {ProductId}", CommunityToolkit.Maui.Core.ToastDuration.Short);
                await OnUnSubscribeToProduct();
                await Shell.Current.GoToAsync("..");
            } else
            {
                await _toastService.ShowToast($"Ocurrio un error al intentar eliminar {ProductId}", CommunityToolkit.Maui.Core.ToastDuration.Short);
                await Shell.Current.GoToAsync("..");
            }

        }

        [RelayCommand]
        public async Task OnSubscribeToProduct()
        {
            var token = await _backendClient.GetBackendToken();

            var result = await _backendClient.SubscribeToProduct(ProductId,
                "eGw39NR4T_2gwk6mfMoQXw:APA91bGJi11vxsjaUPFcXoecNn_9rRFory4xZmQ6x4O1v3k_Ie5nau6O1BPcEUB6LrQKks7lb35YxdTpCbSD1T4w96p6e-7dp-yL2e0QC_S-7qn5F38Gxw0", token);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await _toastService.ShowToast("Comenzaras a recibir notificaiones sobre este producto", CommunityToolkit.Maui.Core.ToastDuration.Long);
                await Shell.Current.GoToAsync("..");
            } else
            {
                await _toastService.ShowToast("Ha ocurrido un error, intenta mas tarde", CommunityToolkit.Maui.Core.ToastDuration.Short);
                await Shell.Current.GoToAsync("..");
            }

        }

        [RelayCommand]
        public async Task OnUnSubscribeToProduct()
        {
            var token = await _backendClient.GetBackendToken();

            var result = await _backendClient.UnSubscribeToProduct(ProductId,
                 "eGw39NR4T_2gwk6mfMoQXw:APA91bGJi11vxsjaUPFcXoecNn_9rRFory4xZmQ6x4O1v3k_Ie5nau6O1BPcEUB6LrQKks7lb35YxdTpCbSD1T4w96p6e-7dp-yL2e0QC_S-7qn5F38Gxw0", token);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await _toastService.ShowToast("Dejararas a recibir notificaiones sobre este producto", CommunityToolkit.Maui.Core.ToastDuration.Long);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await _toastService.ShowToast("Ha ocurrido un error, intenta mas tarde", CommunityToolkit.Maui.Core.ToastDuration.Short);
                await Shell.Current.GoToAsync("..");
            }

        }

        [RelayCommand]
        public async Task OnLaunchProduct()
        {
            //var result = await _backendClient.SubscribeToProduct(ProductId,
            //    "eGw39NR4T_2gwk6mfMoQXw:APA91bGJi11vxsjaUPFcXoecNn_9rRFory4xZmQ6x4O1v3k_Ie5nau6O1BPcEUB6LrQKks7lb35YxdTpCbSD1T4w96p6e-7dp-yL2e0QC_S-7qn5F38Gxw0");

            //if (result.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    //await _toastService.ShowToast("Dejararas a recibir notificaiones sobre este producto");
            //    //await Shell.Current.GoToAsync("..");
            //}
            //else
            //{
            //    //await _toastService.ShowToast("Ha ocurrido un error, intenta mas tarde");
            //    //await Shell.Current.GoToAsync("..");
            //}

        }

        [RelayCommand]
        public async static Task OnConfirmationPopUp()
        {
            await Shell.Current.GoToAsync("..");
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
            Reset();
            try
            {
                var backendToken = await _backendClient.GetBackendToken().ConfigureAwait(false);

                if (!string.IsNullOrEmpty(backendToken) && !string.IsNullOrEmpty(ProductId))
                {
                    var backendProductTask = _backendClient.GetProducHistory(ProductId, backendToken);
                    var sqliteProductTask = _database.GetProduct(ProductId);

                    var backendProductResponse = await backendProductTask.ConfigureAwait(false);
                    var localProductData = await sqliteProductTask.ConfigureAwait(false);

                    backendProductResponse.EnsureSuccessStatusCode();
                    var productData = JsonConvert.DeserializeObject<ProductRecord>(await backendProductResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

                    if (!string.IsNullOrEmpty(productData!.ProductId))
                    {
                        Application.Current?.Dispatcher.Dispatch(() =>
                        {
                            ProductImage = productData.ProductImage;
                            ChartTitle = $"Historial de precios para:\n" + productData.ProductId;
                            CurrentPrice = productData.ProductCurrentPrice;
                            HighestPrice = productData.ProductHighestPrice;
                            LowestPrice = productData.ProductLowestPrice;
                            LastUpdateDate = productData.LastUpdateDate;
                            ProductUrl = localProductData.ProductUrl;
                            IsFollowingProduct = localProductData.IsFollowingProduct;

                            ChartMaxvalue = HighestPrice + 100;
                            ChartMinvalue = LowestPrice - 50;
                            Data = productData.ProductHistory;
                            IsLoading = false;
                        });
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    //Backend is set to "fail" when product is not found
                    //it doens't fails actually
                    IsLoading = false;
                    PopUpVisble = true;

                    Console.WriteLine($"Error en la llamada al servicio backend: {httpEx.Message}");
                });
            }
            catch (SQLiteException sqliteEx)
            {
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    IsLoading = false;
                    PopUpVisble = true;

                    Console.WriteLine($"Error en la llamada a SQLite: {sqliteEx.Message}");
                });
            }
            catch (InvalidOperationException invalidOpEx)
            {
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    IsLoading = false;
                    PopUpVisble = true;

                    Console.WriteLine($"Error de deserialización: {invalidOpEx.Message}");
                });
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    IsLoading = false;
                    PopUpVisble = true;
                    
                    Console.WriteLine($"Se produjo un error: {ex.Message}");
                });
            }

        }
    }
}

//Funciones de paga
// Sigue mas de 10 productos
// Recibe notificaciones para mas de 3 productos - esto seva almacenar en el securestorage
// ver historial pasado y filtrado
// persistencia de productos seguidos