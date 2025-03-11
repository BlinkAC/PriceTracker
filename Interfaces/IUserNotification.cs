using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;

namespace Products3.Interfaces
{
    public interface IUserNotification
    {
        public Task HandleToastNavigationAsync(string message, ToastDuration duration, string navigationPage);
    }
}
