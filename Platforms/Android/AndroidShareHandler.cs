using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Products3.Services;

namespace Products3.Platforms.Android
{
    public class AndroidShareHandler : IShareHandler
    {
        public void HandleShare(object intent)
        {
            var androidIntent = intent as Intent;
            if (androidIntent?.Action == Intent.ActionSend && androidIntent.Type == "text/plain")
            {
                    var data = androidIntent?.ClipData?.GetItemAt(0);
                    var text = data.Text;
                    // Enviar el mensaje con la URL recibida
                    MessagingService.SendUrlReceivedMessage(text);

                    System.Diagnostics.Debug.WriteLine("URL compartida: " + text);

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("El intent no contiene datos de texto.");
            }
        }
    }
}
