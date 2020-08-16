using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ModMaker;
using Kingmaker.EntitySystem.Persistence.JsonUtility;

namespace PortraitManager
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DefaultLanguage: ILanguage
    {
        [JsonProperty]
        public string Language { get; set; } = "English (Default)";

        [JsonProperty]
        public Version Version { get; set; }

        [JsonProperty]
        public string Contributors { get; set; }

        [JsonProperty]
        public string HomePage { get; set; }

        [JsonProperty]
        public Dictionary<string, string> Strings { get; set; } = new Dictionary<string, string>()
        {
            { "Menu_Tab_Settings", "Settings" },
            { "Menu_All_Label_NotInGame", "Not in the game. Please start or load the game first." }
        };

        public T Deserialize<T>(TextReader reader)
        {
            DefaultJsonSettings.Initialize();
            return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
        }

        public void Serialize<T>(TextWriter writer, T obj)
        {
            DefaultJsonSettings.Initialize();
            writer.Write(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

    }
}
