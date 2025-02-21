using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Products3.Models.SQLModels
{
    public record ProductRecord
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;
        [JsonPropertyName("productImage")]
        public string ProductImage { get; set; } = string.Empty;
        [JsonPropertyName("lastUpdateDate")]
        public DateTime LastUpdateDate { get; set; }
        [JsonPropertyName("currentPrice")]
        public float ProductCurrentPrice { get; set; }
        [JsonPropertyName("highestPrice")]
        public float ProductHighestPrice { get; set; }
        [JsonPropertyName("lowestPrice")]
        public float ProductLowesttPrice { get; set; }
        [JsonPropertyName("priceHistory")]
        public IEnumerable<ProductHistory> ProductHistory { get; set; } = new List<ProductHistory>();
    }

    public record ProductHistory
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("price")]
        public float Price { get; set; }
    }

}
