
using Products3.Interfaces;
using ISecureStorage = Products3.Interfaces.ISecureStorage;
namespace Products3.Services
{
    internal class SecureStorageWrapper : ISecureStorage
    {
        public Task<string?> GetAsync(string key)
        {
#pragma warning disable CS8619 //GetAsync returns null if key not found
            return SecureStorage.GetAsync(key);
#pragma warning restore CS8619
        }

        public Task SetAsync(string key, string value)
        {
            return SecureStorage.SetAsync(key, value);
        }

        public bool Remove(string key)
        {
            return SecureStorage.Remove(key);
        }

    }
}
