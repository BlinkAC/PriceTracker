using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace PriceTracker.API.Models
{
    public class UserDetails
    {
        [JsonProperty("userId")]
        public string? UserId { get; set; }
        [JsonProperty("fcmToken")]
        public string? FcmToken { get; set; }
        [JsonProperty("isPremiumUser")]
        public int IsPremiumUser { get; set; }

        [JsonProperty("userSubscriptions")]
        public IEnumerable<string>? UserSubscriptions { get; set; }
    }
}