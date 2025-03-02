using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;

namespace Products3.Interfaces
{
    public interface IToastService
    {
        public Task ShowToast(string message, ToastDuration duration);
    }
}
