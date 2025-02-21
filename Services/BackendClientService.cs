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
        public Task<HttpResponseMessage> GetProducHistory()
        {
            return _httpClient.GetAsync(new Uri("http://192.168.100.26:8080/api/get-product-history?productId=MLM24529297"));
        }
    }
}
