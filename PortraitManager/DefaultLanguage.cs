using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ModMaker;
using Kingmaker.EntitySystem.Persistence.JsonUtility;

namespace KingmakerPortraitManager
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
            { "Menu_Tab_PortraitList", "Portrait List" },
            { "Menu_PortraitList_Btn_UnloadPortraits", "Unload Portraits"},
            { "Menu_PortraitList_Btn_LoadPortraits", "Load Portraits"},
            { "Menu_PortraitList_Btn_OpenPortraitFolder", "Open Portrait Folder"},
            { "Menu_PortraitList_Btn_SavePortraitData", "Save Portrait Data"},
            { "Menu_PortraitList_Btn_OpenFolder", "Open Portrait Folder"},
            { "Menu_PortraitList_Btn_AddTag", "Add Tag"},
            { "Menu_PortraitList_Btn_RemoveTag", "Remove Tag"},
            { "Menu_PortraitList_Btn_ClearTags", "Clear Tags"},
            { "Menu_PortraitList_Lbl_IsCustom", "Is Custom: {0}"},
            { "Menu_PortraitList_Lbl_PortraitImage", "Full portrait image"},
            { "Menu_PortraitList_Lbl_PortraitID", "Portrait ID: {0}"},
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
