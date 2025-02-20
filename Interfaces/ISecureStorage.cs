using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products3.Interfaces
{
    public interface ISecureStorage
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value);
        bool Remove(string key);
    }
}
