using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Products3.Interfaces;
using Products3.Models;
using Products3.Models.Payloads;

namespace Products3.Services
{
    public class BackendClientService : IBackendClient
    {
        private readonly HttpClient _httpClient;
        public BackendClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
            
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/api/ValidateProduct", content);
            return response;
        }

        public async Task<HttpResponseMessage> GetProducHistory(string productId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var result = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress + $"/api/ProductHistory?productId={productId}&productStore=ML")).ConfigureAwait(false);
            return result;
        }

        public async Task<HttpResponseMessage> SubscribeToProduct(string productId, string fcmToken, string token, int action = 1, string productStore = "ML")
        {
            var bodyContent = new SubscribeProduct()
            {
                Action = action,
                ProductId = productId,
                ProductStore = productStore,
                FcmToken = fcmToken
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/api/ProductSubscribe", content);
            return response;
        }

        public async Task<HttpResponseMessage> UnSubscribeToProduct(string productId, string fcmToken, string token, int action = 0, string productStore = "ML" )
        {
            var bodyContent = new SubscribeProduct()
            {
                Action = action,
                ProductId = productId,
                ProductStore = productStore,
                FcmToken = fcmToken
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/api/ProductSubscribe", content);
            return response;
        }

        public async Task<string> GetBackendToken()
        {
            var response = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress + "/api/app-backend-token"));
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(content);
            return token!.ApiToken!;
        }
    }//get-product-history?productId=MLM12487218&productStore=ML
}
