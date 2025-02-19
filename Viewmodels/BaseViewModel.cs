using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Products3.Viewmodels
{
    public abstract class BaseViewModel : ObservableObject
    {
        public abstract Task Initialize();
    }
}
