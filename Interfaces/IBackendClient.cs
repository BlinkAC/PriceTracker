using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products3.Interfaces
{
    public interface IBackendClient
    {
        public Task<string> GetBackendToken();
        public Task<HttpResponseMessage> GetProducHistory(string productId, string token);
        public Task<HttpResponseMessage> SubscribeToProduct(string productId, string fcmToken, string token, int action = 1, string productStore = "ML");

        public Task<HttpResponseMessage> UnSubscribeToProduct(string productId, string fcmToken, string token, int action = 0, string productStore = "ML");

        public Task<HttpResponseMessage> CheckProductAvailability(string productId, string productUrl, string productStore, string token);
    }
}
