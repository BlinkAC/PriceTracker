using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Products3.Interfaces;
using SQLite;
using ISecureStorage = Products3.Interfaces.ISecureStorage;

namespace Products3.Services
{
    public class SqliteConnectionFactory : ISqliteConnectionFactory
    {
	    private readonly ISecureStorage _secureStorage;
        //private readonly IFileSystem _fileSystem;
            private readonly  SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        //IFileSystem fileSystem
        public SqliteConnectionFactory(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage;
            //_fileSystem = fileSystem;
        }

        public async Task<SQLiteConnection> Create(string databasePath)
        {
            var encryptionKey = await _secureStorage.GetAsync("db_key").ConfigureAwait(false);
            if (encryptionKey == null)
            {
                encryptionKey = Guid.NewGuid().ToString();
                await _secureStorage.SetAsync("db_key", encryptionKey).ConfigureAwait(false);
                // Database is no longer encrypted and we also have the uninstaller in SC to remove the database from the device, leaving this code out -TB
                //if (_fileSystem.File.Exists(databasePath))
                //{
                //	_fileSystem.File.Delete(databasePath);
                //}
            }
            /// As a part of ADO 651313, Encryption from database file has been removed for easy troubleshooting.
            var options = new SQLiteConnectionString(databasePath, Flags, true);
            return new SQLiteConnection(options);
        }
    }
}
