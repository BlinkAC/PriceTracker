using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;

namespace Products3.Models.User
{
    public class UserLocalData
    {
        [PrimaryKey]
        public string? UserId { get; set; }
        public string? FcmToken { get; set; }
        public int IsPremiumUser { get; set; }
        public string? UserSubscriptions { get; set; }

        public string? DisplayName { get; set; }

        public int SubcriptionsCounter => UserSubscriptions!.Split(",").Count();
    }
}
