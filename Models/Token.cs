using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Products3.Models
{
    public class Token
    {
        [JsonProperty("token")]
        public string? ApiToken { get; set; }
    }
}
