using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Products3.Models.SQLModels
{
    public record Product
    {
        [PrimaryKey]
        public string ProductId { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        [Ignore]
        public string ProductImage { get; set; } = string.Empty;

        [Ignore]
        public bool ProductChecked{ get; set; } = false;

        [Ignore]
        public IEnumerable<ProductRecord> ProductPriceHistory { get; set; } = [];

    }
}
