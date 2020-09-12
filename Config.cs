using Newtonsoft.Json;
using System;

namespace AdviceBotVK
{
    [Serializable]
    class VkBotConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("group_id")]
        public ulong GroupId { get; set; }
    }
}
