using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Products3.Models.SQLModels
{
    public record ProductRecord
    {
        [JsonProperty("id")]
        public string ProductId { get; set; } = string.Empty;

        [JsonProperty("productImage")]
        public string ProductImage { get; set; } = string.Empty;

        [JsonProperty("lastUpdateDate")]
        public DateTime LastUpdateDate { get; set; }

        [JsonProperty("currentPrice")]
        public float ProductCurrentPrice { get; set; }

        [JsonProperty("highestPrice")]
        public float ProductHighestPrice { get; set; }

        [JsonProperty("lowestPrice")]
        public float ProductLowestPrice { get; set; }

        [JsonProperty("isUserSubscribed")]
        public int? IsUserSubscribed { get; set; }

        [JsonProperty("priceHistory")]
        public IEnumerable<ProductHistory> ProductHistory { get; set; } = new List<ProductHistory>();
    }

    public record ProductHistory
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("price")]
        public float Price { get; set; }
    }

}
