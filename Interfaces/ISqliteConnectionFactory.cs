using SQLite;

namespace Products3.Interfaces
{
    public interface ISqliteConnectionFactory
    {
        public Task<SQLiteConnection> Create(string databasePath);
    }
}
