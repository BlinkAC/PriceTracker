using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Products3.Interfaces;
using Products3.Models.SQLModels;
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
        /// Removes on single product based on ID
        /// </summary>
        /// <param name="logIds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task RemoveProduct(string logIds)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Used to remove multiple products at once
        /// </summary>
        /// <param name="productString"></param>
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
        //private readonly IGistDatabase _database;
        //IGistDatabase database

    }
}
