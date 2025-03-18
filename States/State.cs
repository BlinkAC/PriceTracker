using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Products3.Models.User;

namespace Products3.States
{
    public class State
    {
        public readonly ObservableProperty<UserLocalData> CurrentUserInfo = new(new UserLocalData());
    }
}
