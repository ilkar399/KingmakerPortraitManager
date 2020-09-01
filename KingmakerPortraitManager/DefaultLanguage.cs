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
            { "Menu_Tab_PortraitPacks", "Portrait Pack Export/Import" },
            { "Menu_PortraitList_Btn_UnloadPortraits", "Unload Portraits"},
            { "Menu_PortraitList_Btn_LoadPortraits", "Load Portraits"},
            { "Menu_PortraitList_Btn_OpenPortraitFolder", "Open Portrait Folder"},
            { "Menu_PortraitList_Btn_SavePortraitData", "Save Portrait Data"},
            { "Menu_PortraitList_Btn_CancelPortrait", "Cancel changes"},
            { "Menu_PortraitList_Btn_SavePortraitDataAll", "Save All Portrait Data"},
            { "Menu_PortraitList_Btn_OpenFolder", "Open Portrait Folder"},
            { "Menu_PortraitList_Btn_AddTag", "Add Tag"},
            { "Menu_PortraitList_Btn_RemoveTag", "Remove Tag"},
            { "Menu_PortraitList_Btn_ClearTags", "Clear Tags"},
            { "Menu_PortraitList_Btn_AllTags", "All Tags"},
            { "Menu_PortraitList_Btn_ApplyFilters", "Apply filters to game"},
            { "Menu_PortraitList_Lbl_IsCustom", "Is Custom: {0}"},
            { "Menu_PortraitList_Lbl_PortraitImage", "Full portrait image (scaled to fit)"},
            { "Menu_PortraitList_Lbl_PortraitID", "Portrait ID: {0}"},
            { "Menu_PortraitList_Lbl_Hash", "Portrait hash: {0}"},
            { "Menu_PortraitList_Lbl_tagMsg", "Add tag:"},
            { "Menu_PortraitList_Lbl_tagList", "List of tags (click to remove):"},
            { "Menu_PortraitList_Lbl_Filters","Filter portraits by:"},
            { "Menu_PortraitList_Lbl_PortraitListHeader","Portrait list by id:"},
            { "Menu_PortraitPacks_Btn_LoadPortraits","Load Portraits"},
            { "Menu_PortraitPacks_Btn_LoadPortraitsImport","Load Portraits from the Import folder"},
            { "Menu_PortraitPacks_Btn_Export","Export all custom portraits to the Export folder"},
            { "Menu_PortraitPacks_Btn_Import","Import all custom portraits from the import folder"},
            { "Menu_PortraitPacks_Tgl_MergeTags","Merge tags (they're overwritten otherwise)"},
            { "Menu_PortraitPacks_Lbl_CurrentData","Current portraits info:"},
            { "Menu_PortraitPacks_Lbl_ImportingData","Portrait info from the import folder:"},
            { "Menu_PortraitPacks_Lbl_PortraitsTotal","Total non-default portraits: {0}"},
            { "Menu_PortraitPacks_Lbl_PortraitsTagged","Tagged portraits: {0}"},
            { "Menu_PortraitPacks_Lbl_PortraitsDuplicateHash","Duplicate Hash: {0}"},
            { "Menu_PortraitPacks_Lbl_PortraitsError","Errors checking: {0}"},
            { "Menu_PortraitPacks_Lbl_TagsTotal","Total tags: {0}"},
            //TODO
            { "Menu_PortraitPacks_Lbl_ExportHint","TODO. Portraits saved to the Exported directory. Portraits to one folder, tags to another. Add zip archiver?"},
            { "Menu_PortraitPacks_Lbl_ImportHint","TODO."},
            { "Menu_Settings_ToggleIgnoreDefaultPortraits","Ignore default portraits while processing"},
            { "Menu_All_Label_NotInGame", "Not in the game. Please start or load the game first." },
            { "GUI_TagSelector_Label","Select tag to filter by:"},
            { "GUI_TagSelectorItem_Label","Custom portrait tag"},
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
