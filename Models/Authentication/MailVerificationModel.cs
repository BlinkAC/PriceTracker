using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Products3.Models.Authentication
{
    class MailVerificationModel
    {
        [JsonProperty("requestType")]
        public string? RequestType { get; set; }

        [JsonProperty("idToken")]
        public string? IdToken { get; set; }
    }
}
