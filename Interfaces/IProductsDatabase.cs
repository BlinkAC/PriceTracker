using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Products3.Models.SQLModels;
using Products3.Services;
using SQLite;

namespace Products3.Interfaces
{
    public interface IProductsDatabase
    {
        AsyncLazy<SQLiteConnection> DbConnection { get; }
        public Task<int> AddProduct(Product product);

        public Task<IEnumerable<Product>> GetProducts();

        public Task RemoveProducts(IEnumerable<string> productString);

        public Task RemoveProduct(string logIds);

    }
}
