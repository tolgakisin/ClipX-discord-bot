using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipX.Models
{
    public class UserWrapper
    {
        [JsonProperty("data")]
        public List<User> Data { get; set; }
    }
}
