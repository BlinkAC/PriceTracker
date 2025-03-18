using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Microsoft.Maui.Graphics;
using PriceTracker.API.Models;
using Products3.Interfaces;
using Products3.Models.SQLModels;
using Products3.Models.User;
using SQLite;

namespace Products3.Services
{
    public class ProductsDatabase : IProductsDatabase
    {
        private readonly AsyncLazy<SQLiteConnection> _dbConnection;
        private readonly object _dbLock = new();

        public AsyncLazy<SQLiteConnection> DbConnection => _dbConnection;
        public ProductsDatabase(ISqliteConnectionFactory connectionFactory)
        {
            var connection = new AsyncLazy<SQLiteConnection>(async () =>
            {
                var connection = await connectionFactory.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Urls.db3")).ConfigureAwait(false);
                connection.Execute("PRAGMA foreign_keys = ON");
                connection.CreateTable<Product>();
                connection.CreateTable<UserLocalData>();

                return connection;
            });

            _dbConnection = connection;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product">The product id and url to be stored locally</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> AddProduct(Product product)
        {
            var conn = await _dbConnection;
            return InsertThreadSafe(product, conn);
        }

        /// <summary>
        /// Retrieves the store products
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<Product>> GetProducts()
        {
            var conn = await _dbConnection;
            return conn.Table<Product>().ToList();
        }

        /// <summary>
        /// Removes ALL products
        /// used when user has signed out
        /// </summary>
        /// <returns></returns>
        public async Task<int> RemoveAllProducts()
        {
            var conn = await _dbConnection;

            return DeleteAllThreadSafe<Product>(conn);
        }

        /// <summary>
        /// Removes single/multiple product based on ID
        /// used when user stops following mannualy
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> RemoveProducts(IEnumerable<string> productIds)
        {
            var conn = await _dbConnection;

            // Construir la consulta de eliminación
            var parameterizedQuery = "DELETE FROM Product WHERE ProductId IN (" +
                                      string.Join(",", productIds.Select((id) => $"'{id}'")) + ")";

            // Ejecutar la consulta de eliminación y obtener el número de filas afectadas
            var rowsAffected = await Task.Run(() => conn.Execute(parameterizedQuery));

            return rowsAffected;
        }

        public async Task<Product> GetProduct(string productId)
        {
            var conn = await _dbConnection;
            var product = conn.Table<Product>().Where(v => v.ProductId.Equals(productId)).FirstOrDefault();

            return product;

        }
        public async Task<UserLocalData?> GetUserInfo()
        {
            var conn = await _dbConnection;
            return conn.Query<UserLocalData>("SELECT * FROM UserLocalData LIMIT 1").FirstOrDefault();
        }

        /// <summary>
        /// Caches the user info
        /// </summary>
        /// <param name="userInfo">The user info to save</param>
        /// <returns>A task</returns>
        public async Task SaveUserInfo(UserLocalData userInfo)
        {
            var conn = await _dbConnection;
            InsertThreadSafe(userInfo, conn);
        }

        public async Task<int> DeleteUserInfo()
        {
            var conn = await _dbConnection;

            return DeleteAllThreadSafe<UserLocalData>(conn);
        }

        private int DeleteAllThreadSafe<T>(SQLiteConnection conn)
        {
            lock (_dbLock)
            {
                return conn.DeleteAll<T>();
            }
        }
        private int UpdateAll<T>(IEnumerable<T> data, SQLiteConnection conn)
        {
            return conn.UpdateAll(data);

        }
        private int InsertThreadSafe(object data, SQLiteConnection conn)
        {
            lock (_dbLock)
            {
                return conn.Insert(data);
            }
        }

        public async Task UpdateUserSubscriptions(string products, string userId)
        {
            var conn = await _dbConnection;
            var user = conn.Find<UserLocalData>(userId);
            user.UserSubscriptions = products;

            conn.Update(user);
            //return conn.Query<UserLocalData>("SELECT * FROM UserLocalData LIMIT 1").FirstOrDefault();
        }




        //private readonly IGistDatabase _database;
        //IGistDatabase database

    }
}
