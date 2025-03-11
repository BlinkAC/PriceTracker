using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Products3.Models.Authentication
{
    public class RegisterModel : ObservableValidator
    {
        private string username = string.Empty;
        private string email = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;


        [Required]
        [MaxLength(50)]
        public string Name
        {
            get => username;
            set => SetProperty(ref username, value, true);
        }

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

        [Required]
        [MaxLength(50)]
        [Compare("Password")]
        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value, true);
        }
    }
}
