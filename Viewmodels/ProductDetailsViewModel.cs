
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Newtonsoft.Json;
using Plugin.Firebase.CloudMessaging;
using Products3.Interfaces;
using Products3.Models.SQLModels;
using Products3.Models.User;
using Products3.States;
using Products3.Views.Pages;
using SQLite;

namespace Products3.Viewmodels
{
    public partial class ProductDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IProductsDatabase _database;
        private readonly IBackendClient _backendClient;
        private readonly IToastService _toastService;
        private readonly FirebaseAuthClient _firebaseAuthClient;
        private readonly IUserNotification _notificationService;
        //private readonly State State;

        #region Properties
        private string chartTitle = string.Empty;
        public string ChartTitle
        {
            get => chartTitle;
            set => SetProperty(ref chartTitle, value);
        }


        private double currentPrice = 0;

        public double CurrentPrice
        {
            get => currentPrice;
            set => SetProperty(ref currentPrice, value);
        }

        private double highestPrice = 0;

        public double HighestPrice
        {
            get => highestPrice;
            set => SetProperty(ref highestPrice, value);
        }


        private double lowestPrice = 0;

        public double LowestPrice
        {
            get => lowestPrice;
            set => SetProperty(ref lowestPrice, value);
        }

        private DateTime lastUpdateDate;

        public DateTime LastUpdateDate
        {
            get => lastUpdateDate;
            set => SetProperty(ref lastUpdateDate, value);
        }

        private string productId = string.Empty;
        public string ProductId
        {
            get => productId;
            set => SetProperty(ref productId, value);
        }

        private string productImage = string.Empty;
        public string ProductImage
        {
            get => productImage;
            set => SetProperty(ref productImage, value);
        }

        private string productUrl = string.Empty;
        public string ProductUrl
        {
            get => productUrl;
            set => SetProperty(ref productUrl, value);
        }

        private bool isFollowingProduct = false;
        public bool IsFollowingProduct
        {
            get => isFollowingProduct;
            set => SetProperty(ref isFollowingProduct, value);
        }

        private IEnumerable<ProductHistory> data = [];
        public IEnumerable<ProductHistory> Data
        {
            get => data;
            set => SetProperty(ref data, value);
        }


        private bool popUpVisble = false;
        public bool PopUpVisble
        {
            get => popUpVisble;
            set => SetProperty(ref popUpVisble, value);
        }

        private string dialogText = string.Empty;
        public string DialogText
        {
            get => dialogText;
            set => SetProperty(ref dialogText, value);
        }

        private bool isLoading = true;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                // Agregar un log para ver si se está llamando correctamente
                Console.WriteLine($"Setting HighestPrice to {value}");
                SetProperty(ref isLoading, value);
            }
        }

        private string activityindicatorText;
        public string ActivityindicatorText
        {
            get => activityindicatorText;
            set
            {
                SetProperty(ref activityindicatorText, value);
            }
        }

        private double chartMaxvalue = 0;

        public double ChartMaxvalue
        {
            get => chartMaxvalue;
            set => SetProperty(ref chartMaxvalue, value);
        }

        private double chartMinvalue = 0;

        public double ChartMinvalue
        {
            get => chartMinvalue;
            set => SetProperty(ref chartMinvalue, value);
        }

        private bool subscriptionButton = false;
        public bool SubscriptionButton
        {
            get => subscriptionButton;
            set => SetProperty(ref subscriptionButton, value);
        }
        #endregion

        public ProductDetailsViewModel(IProductsDatabase database, 
                                       IBackendClient backendClient,
                                       IToastService toastService,
                                       FirebaseAuthClient firebaseAuthClient,
                                       IUserNotification notificationService,
                                       State state) : base(state)
        {
            _database = database;
            _backendClient = backendClient;
            _toastService = toastService;
            _firebaseAuthClient = firebaseAuthClient;
            _notificationService = notificationService;
            //State = state;
        }
        public override void Reset()
        {
            IsLoading = true;
            ChartTitle = string.Empty;
            ProductImage = string.Empty;
            SubscriptionButton = false;
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
                await _notificationService.HandleToastNavigationAsync($"Haz dejado de seguir {ProductId}", 
                                                                        CommunityToolkit.Maui.Core.ToastDuration.Long,
                                                                        $"//{nameof(MainPage)}");

            } else
            {
                await _notificationService.HandleToastNavigationAsync($"Ocurrio un error al intentar eliminar {ProductId}",
                                                        CommunityToolkit.Maui.Core.ToastDuration.Long,
                                                        $"//{nameof(MainPage)}");
            }

        }

        [RelayCommand]
        public async Task OnSubscribeToProduct()
        {
            if (_firebaseAuthClient.User.Info.IsEmailVerified)
            {
                var fcmToken = State.CurrentUserInfo.Get().FcmToken;
                var localUserSubcriptions = State.CurrentUserInfo.Get().UserSubscriptions!.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                var userId = State.CurrentUserInfo.Get().UserId;

                var tasks = new []
                {
                    _backendClient.GetBackendToken(),
                    
                };

                await Task.WhenAll(tasks);

                var token = tasks[0].Result;

                if (localUserSubcriptions.Count < 3)
                {
                    ActivityindicatorText = "Suscribiendote al producto...";
                    IsLoading = true;
                    localUserSubcriptions.Add(ProductId);
                    var cosmosProductResult = await _backendClient.SubscribeToProduct([ProductId], fcmToken, token);

                    var userDataUpdate = new ClientUserData() { FcmToken = fcmToken, UserId = userId, UserSubscriptions = localUserSubcriptions };
                    

                    if (cosmosProductResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var subscriptionsString = string.Join(",", localUserSubcriptions.Distinct().Select(x => x));

                        //mongo update it nos useful for ui thread so can be executed in a different thread
                        await _backendClient.UpdateUserInfo(userDataUpdate, token);
                        await _database.UpdateUserSubscriptions(subscriptionsString, userId!);

                        State.CurrentUserInfo.Set(await _database.GetUserInfo());

                        isLoading = false;
                        await _notificationService.HandleToastNavigationAsync($"Comenzaras a recibir notificaciones sobre este producto",
                                            CommunityToolkit.Maui.Core.ToastDuration.Short,
                                            $"//{nameof(MainPage)}");

                    }
                    else
                    {

                        isLoading = false;
                        await _notificationService.HandleToastNavigationAsync($"Ha ocurrido un error, intenta mas tarde",
                        CommunityToolkit.Maui.Core.ToastDuration.Short,
                        $"//{nameof(MainPage)}");
                    }
                }
                else
                {
                    // Solo cambia el valor en el ViewModel
                    PopUpVisble = true;
                    DialogText = "Te encuentras suscrito a 3 productos. \nActualiza tu cuenta o desuscribete de algun producto primero.";
                }
            } else
            {
                PopUpVisble = true;
                DialogText = "Para suscribirte y recibir notificaciones de un producto debes verificar tu correo primero";
            }
            


        }

        [RelayCommand]
        public async Task OnUnSubscribeToProduct()
        {
            try
            {
                // Mostrar indicador de carga (esto sería una propiedad bindable en tu ViewModel)
                ActivityindicatorText = "Desuscribiendote del producto...";
                IsLoading = true;
                var fcmToken = State.CurrentUserInfo.Get().FcmToken;
                var localUserSubcriptions = State.CurrentUserInfo.Get().UserSubscriptions!.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                var userId = State.CurrentUserInfo.Get().UserId;

                // Llamadas asíncronas y captura de datos
                var tasks = new[]
                {
                    _backendClient.GetBackendToken(),
                };

                await Task.WhenAll(tasks); // Ejecutar tareas en paralelo para optimizar tiempo de espera

                var token = tasks[0].Result;

                localUserSubcriptions.Remove(ProductId);

                // Llamada al backend para desuscribirse
                var cosmosProductResult = await _backendClient.UnSubscribeToProduct([ProductId], fcmToken, token);

                string subscriptions = localUserSubcriptions?.Any() == true
                                        ? string.Join(",", localUserSubcriptions.Where(x => !string.IsNullOrWhiteSpace(x.ToString())))
                                        : string.Empty;

                var userDataUpdate = new ClientUserData() { FcmToken = fcmToken, UserId = _firebaseAuthClient.User.Uid, UserSubscriptions = localUserSubcriptions };
                var mongoUserResult = await _backendClient.UpdateUserInfo(userDataUpdate, token);
                

               IsLoading = false;
                // Manejo del resultado (notificaciones o navegación)
                var toastMessage = cosmosProductResult.StatusCode == System.Net.HttpStatusCode.OK
                    ? "Dejarás de recibir notificaciones sobre este producto"
                    : "Ha ocurrido un error, intenta más tarde";

                var navigationPage = $"//{nameof(MainPage)}";
                
                await _notificationService.HandleToastNavigationAsync(toastMessage, CommunityToolkit.Maui.Core.ToastDuration.Short, navigationPage);
                if (cosmosProductResult.StatusCode == System.Net.HttpStatusCode.OK && mongoUserResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await _database.UpdateUserSubscriptions(subscriptions, userId!);
                    State.CurrentUserInfo.Set(await _database.GetUserInfo());

                }
            }
            catch (Exception ex)
            {
                isLoading = false;
                // Manejar errores inesperados
                Console.WriteLine($"Error: {ex.Message}");
                await _notificationService.HandleToastNavigationAsync("Ha ocurrido un error, intenta más tarde",
                    CommunityToolkit.Maui.Core.ToastDuration.Short, $"//{nameof(MainPage)}");
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
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
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
                productId = query["productId"] as string;
                await Initialize();
            }
        }
        public override async Task Initialize()
        {
            Reset();
            try
            {
                ActivityindicatorText = "Obteniendo información...";
                var backendToken = await _backendClient.GetBackendToken().ConfigureAwait(false);
                var fcmToken = State.CurrentUserInfo.Get().FcmToken;
                    //await SecureStorage.GetAsync("fcmToken").ConfigureAwait(false);

                //intenta obtener el id de mongo
                //lo obtuvo usa el parentId para buscar en GetProducHistory(ProductId, backendToken)
                //si no obtuvo nada usa el original
                if (!string.IsNullOrEmpty(backendToken) && !string.IsNullOrEmpty(ProductId))
            {
                var backendProductTask = _backendClient.GetProducHistory(productId, backendToken, fcmToken!);
                var sqliteProductTask = _database.GetProduct(ProductId);

                var backendProductResponse = await backendProductTask.ConfigureAwait(false);
                var localProductData = await sqliteProductTask.ConfigureAwait(false);

                backendProductResponse.EnsureSuccessStatusCode();
                if (backendProductResponse.IsSuccessStatusCode)
                {
                    var productData = JsonConvert.DeserializeObject<ProductRecord>(await backendProductResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

                    if (productData != null && !string.IsNullOrEmpty(productData.ProductId))
                    {
                        if (productData != null && !string.IsNullOrEmpty(productData.ProductId))
                        {
                            ProductImage = productData.ProductImage;
                            ChartTitle = $"Historial de precios para:\n" + productData.ProductId;
                            CurrentPrice = productData.ProductCurrentPrice;
                            HighestPrice = productData.ProductHighestPrice;
                            LowestPrice = productData.ProductLowestPrice;
                            LastUpdateDate = productData.LastUpdateDate;
                            ProductUrl = localProductData.ProductUrl;
                            IsFollowingProduct = localProductData.IsFollowingProduct;
                            SubscriptionButton = productData.IsUserSubscribed == 1 ? true : false;

                            ChartMaxvalue = highestPrice + 100;
                            ChartMinvalue = lowestPrice - 50;
                            Data = productData.ProductHistory;
                            IsLoading = false;
                        }
                    }
                }
            }


                //en un hilo aparte se va actualizar el id del de qli con el que se guardo para usar el parent siempre
                //Task.Run(async () =>
                //{
                //    var updatedProductId = await _database.UpdateProductParentId(ProductId);
                //}).ConfigureAwait(false);
            }
            catch (HttpRequestException httpEx)
            {
                //Cosmos microservice is set to throw a 404 when product is not found
                //however the exception thrown is considered as a Exception and not a CosmosException
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    IsLoading = false;
                    PopUpVisble = true;
                    DialogText = "Por el momento aun no contamos con informacion para este producto.\r\nIntentalo mas tarde.";
                    Console.WriteLine($"Error en la llamada al servicio backend: {httpEx.Message}");
                });
            }
            catch (SQLiteException sqliteEx)
            {
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    IsLoading = false;
                    PopUpVisble = true;
                    DialogText = "Ha ocurrido un error al obtener la informacion del producto (1).";

                    Console.WriteLine($"Error en la llamada a SQLite: {sqliteEx.Message}");
                });
            }
            catch (InvalidOperationException invalidOpEx)
            {
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    IsLoading = false;
                    PopUpVisble = true;
                    DialogText = "Ha ocurrido un error al obtener la informacion del producto (2).";

                    Console.WriteLine($"Error de deserialización: {invalidOpEx.Message}");
                });
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    IsLoading = false;
                    PopUpVisble = true;
                    DialogText = "Ha ocurrido un error al obtener la informacion del producto (3).";

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