using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Products3.Interfaces;
using Products3.Models;
using Products3.Models.Payloads;
using Products3.Models.User;

namespace Products3.Services
{
    public class BackendClientService : IBackendClient
    {
        private readonly HttpClient _httpClient;
        public BackendClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetBackendToken()
        {
            var response = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress + "api/app-backend-token")).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(content);
            return token!.ApiToken!;
        }

        #region MONGO ENDPOINTS
        public async Task<HttpResponseMessage> CheckProductAvailability(string productId, string productUrl, string productStore, string token)
        {
            var bodyContent = new InsertProduct()
            {
                ProductId = productId,
                ProductStore = productStore,
                ProductUrl = productUrl
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "api/ValidateProduct", content).ConfigureAwait(false);
            return response;
        }

        public async Task<HttpResponseMessage> CheckUserInfo(ClientUserData userData, string authToken)
        {
            var bodyContent = new ClientUserData()
            {
                UserId = userData.UserId,
                FcmToken = userData.FcmToken,
                UserSubscriptions = userData.UserSubscriptions,
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "api/CheckUserInfo", content).ConfigureAwait(false);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateUserInfo(ClientUserData userData, string authToken)
        {
            var bodyContent = new ClientUserData()
            {
                UserId = userData.UserId,
                FcmToken = userData.FcmToken,
                UserSubscriptions = userData.UserSubscriptions,
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "api/UpdateUserInfo", content).ConfigureAwait(false);
            return response;
        }
        #endregion


        #region COSMOS ENDPOINTS
        public async Task<HttpResponseMessage> GetProducHistory(string productId, string token, string fcmToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var result = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress + $"api/ProductHistory?productId={productId}&productStore={"ML"}&fcmToken={fcmToken}")).ConfigureAwait(false);
            //https://aelexyz-pricetracker-products-dphrgff0d6e3h0hh.canadacentral-01.azurewebsites.net/api/ProductHistory?productId=MLM24529297&productStore=ML&fcmToken=fe2nOlpWQj-Jh2AqzPmtgp:APA91bHhcSNojEvIqP-VvVdLwrppElxggGX630jurDXOZYbo1GFISB-5tdsyk8tBNLo1siYrl_YV4lrOyHIvO3Uw4saJiF2NnKgzL_yHiEypQoKRSLqyRFE
            return result;
        }

        public async Task<HttpResponseMessage> SubscribeToProduct(List<string> productsId, string fcmToken, string token, int action = 1, string productStore = "ML")
        {
            var bodyContent = new SubscribeProduct()
            {
                Action = action,
                ProductsId = productsId,
                ProductStore = productStore,
                FcmToken = fcmToken
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "api/ProductSubscribe", content).ConfigureAwait(false);
            return response;
        }

        public async Task<HttpResponseMessage> UnSubscribeToProduct(List<string> productsId, string fcmToken, string authToken, int action = 0, string productStore = "ML")
        {
            var bodyContent = new SubscribeProduct()
            {
                Action = action,
                ProductsId = productsId,
                ProductStore = productStore,
                FcmToken = fcmToken
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "api/ProductSubscribe", content).ConfigureAwait(false);
            return response;
        }
        #endregion







    }//get-product-history?productId=MLM12487218&productStore=ML
}
