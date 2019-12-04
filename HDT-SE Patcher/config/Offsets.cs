using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HDT_SE_Patcher.config
{
    class Offsets
    {
        [JsonPropertyName("versions")]
        public List<string> AvailableVersions { get; set; }
        [JsonPropertyName("files")]
        public Dictionary<String, HDTFile> Files { get; set; }
    }
}
