using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HDT_SMP_Patcher.config
{
    class Offset
    {
        [JsonPropertyName("offset")]
        public string Address { get; set; }
        [JsonPropertyName("patch")]
        public string Patch { get; set; }
    }
}
