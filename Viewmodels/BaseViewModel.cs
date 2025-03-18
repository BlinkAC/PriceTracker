using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Products3.States;
namespace Products3.Viewmodels
{
    public abstract class BaseViewModel : ObservableObject
    {
        public State State { get; private set; }


        protected BaseViewModel(State state)
        {
            State = state;
        }
        public abstract Task Initialize();
        public virtual void Reset(){}
    }
}
