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

        public Task<Product> GetProduct(string productId);

        public Task<IEnumerable<Product>> GetProducts();

        public Task<int> RemoveProducts(IEnumerable<string> productString);

        public Task RemoveProduct(string logIds);

    }
}
