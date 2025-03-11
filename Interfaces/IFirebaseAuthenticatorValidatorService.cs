using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products3.Interfaces
{
    public interface IFirebaseAuthenticatorValidatorService
    {
        public Task SendVerificationEmail(string idToken);
    }
}
