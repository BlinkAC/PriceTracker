using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Products3.Models.User
{
    public class ClientUserData
    {
        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("fcmToken")]
        public string? FcmToken { get; set; }

        [JsonProperty("userSubscriptions")]
        public IEnumerable<string>? UserSubscriptions { get; set; }
    }
}
