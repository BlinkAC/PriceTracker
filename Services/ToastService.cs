using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;
using Products3.Interfaces;

namespace Products3.Services
{
    public class ToastService : IToastService
    {
        public async Task ShowToast(string message, ToastDuration duration )
        {
            var toast = CommunityToolkit.Maui.Alerts.Toast.Make(message, duration);
            await toast.Show();
        }
    }
}
