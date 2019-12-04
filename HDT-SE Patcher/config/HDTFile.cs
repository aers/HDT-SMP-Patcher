using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HDT_SE_Patcher.config
{
    class HDTFile
    {
        [JsonPropertyName("versions")]
        public Dictionary<string, string> Versions { get; set; }
    }
}
