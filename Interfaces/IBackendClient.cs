using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Products3.Models.User;

namespace Products3.Interfaces
{
    public interface IBackendClient
    {
        public Task<string> GetBackendToken();
        public Task<HttpResponseMessage> GetProducHistory(string productId, string token, string fcmToken);
        public Task<HttpResponseMessage> SubscribeToProduct(List<string> productsId, string fcmToken, string token,int action = 1, string productStore = "ML");

        public Task<HttpResponseMessage> UnSubscribeToProduct(List<string> productsId, string fcmToken, string token, int action = 0, string productStore = "ML");

        public Task<HttpResponseMessage> CheckProductAvailability(string productId, string productUrl, string productStore, string token);

        public Task<HttpResponseMessage> CheckUserInfo(ClientUserData userData, string authToken);

        public Task<HttpResponseMessage> UpdateUserInfo (ClientUserData userData, string authToken);


    }
}
