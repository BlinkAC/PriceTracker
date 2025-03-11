using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Products3.Models.Authentication
{
    public class MailLoginModel : ObservableValidator
    {
        private string email = string.Empty;
        private string password = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value, true);
        }

        [Required]
        [MaxLength(50)]
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value, true);
        }
    }
}
