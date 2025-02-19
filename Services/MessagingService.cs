using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products3.Services
{
    public static class MessagingService
    {
        public const string UrlReceivedMessage = "UrlReceived";

        public static void SendUrlReceivedMessage(string url)
        {
            MessagingCenter.Send<object, string>(new object(), UrlReceivedMessage, url);
        }

        public static void SubscribeToUrlReceivedMessage(object subscriber, Action<string> callback)
        {
            MessagingCenter.Subscribe<object, string>(subscriber, UrlReceivedMessage, (sender, url) =>
            {
                callback(url);
            });
        }

        public static void UnsubscribeFromUrlReceivedMessage(object subscriber)
        {
            MessagingCenter.Unsubscribe<object, string>(subscriber, UrlReceivedMessage);
        }
    }
}
