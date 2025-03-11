using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products3.Models.Authentication
{
    public class CurrentUserModel
    {
        public string UserId { get; set; } = string.Empty!;
        public string FullName { get; set; } = string.Empty!;
    }
}
