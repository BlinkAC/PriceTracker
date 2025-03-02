using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Products3.Models.Payloads
{
    public class SubscribeProduct
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; } = string.Empty;
        [JsonProperty("fcmToken")]
        public string FcmToken { get; set; } = string.Empty;
        [JsonProperty("productStore")]
        public string ProductStore { get; set; } = string.Empty;
        [JsonProperty("action")]
        public int Action { get; set; }

    }
}
