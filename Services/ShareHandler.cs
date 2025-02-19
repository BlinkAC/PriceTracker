using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products3.Services
{
    public interface IShareHandler
    {
        void HandleShare(object intent);
    }

    public static class ShareHandler
    {
        private static IShareHandler _platformShareHandler;

        public static void Initialize(IShareHandler platformShareHandler)
        {
            _platformShareHandler = platformShareHandler;
        }

        public static void HandleShare(object intent)
        {
            _platformShareHandler?.HandleShare(intent);
        }
    }
}
