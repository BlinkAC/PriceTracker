using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Products3.Interfaces;

namespace Products3.Services
{
    public class BackendClientService : IBackendClient
    {
        private readonly HttpClient _httpClient;
        public BackendClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public Task<HttpResponseMessage> GetProducHistory(string productId)
        {
            //By default flask and possibly any other local APIs are not accesible in the app
            //it's accesible from console app or postman
            //locally you have to use the machine's IPV4
            //cmd: ipconfig
            //grab IPv4 Address
            return _httpClient.GetAsync(new Uri($"http://192.168.100.26:8080/api/get-product-history?productId={productId}&productStore=ML"));
        }
    }
}
