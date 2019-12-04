using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HDT_SE_Patcher.config
{
    class Version
    {
        [JsonPropertyName("files")]
        public Dictionary<string, string> Hashes { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("offset_file")]
        public string OffsetFile { get; set; }
    }
}
