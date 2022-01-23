using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipX.Models
{
    public class ClipWrapper
    {
        [JsonProperty("data")]
        public List<Clip> Data { get; set; }
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }
}
