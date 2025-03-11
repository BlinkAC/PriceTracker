using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace PriceTracker.API.Models
{
    public class UserData
    {
        [JsonProperty("details")]
        public UserDetails? Details { get; set; }
    }
}
