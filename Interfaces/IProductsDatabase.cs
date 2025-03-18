using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PriceTracker.API.Models;
using Products3.Models.SQLModels;
using Products3.Models.User;
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

        public Task<int> RemoveAllProducts();

        public Task<int> RemoveProducts(IEnumerable<string> logIds);

        public Task<UserLocalData?> GetUserInfo();
        public Task SaveUserInfo(UserLocalData userInfo);

        public Task UpdateUserSubscriptions(string products, string userId);
        public Task<int> DeleteUserInfo();

    }
}
