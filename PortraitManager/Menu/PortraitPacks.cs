using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using ModMaker;
using ModMaker.Utility;
using static KingmakerPortraitManager.Main;
using static KingmakerPortraitManager.Helpers;
using static KingmakerPortraitManager.Utility.SettingsWrapper;
using Kingmaker.Blueprints;

namespace KingmakerPortraitManager.Menu
{
    //Export+Import of Portrait Packs
    class PortraitPacks : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_PortraitPacks"];
        public int Priority => 200;
        private string exportMessage;
        private GUIStyle _buttonStyle;
        private GUIStyle _fixedStyle;
        private Dictionary<string, TagData> currentTagDirData;
        private Dictionary<string, TagData> currentPortraitTagsData;
        private Dictionary<string, TagData> importingTagsData;
        private Dictionary<string, TagData> importingPortraitTagsData;


        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            if (currentPortraitTagsData == null)
                currentPortraitTagsData = new Dictionary<string, TagData>();
            if (exportMessage == null)
                exportMessage = "";
            GUILayout.Space(10f);
            using (new GUILayout.HorizontalScope())
            {
                //Exporting portraits
                //TODO: Remove duplicates
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Local["Menu_PortraitPacks_Lbl_CurrentData"]);
                    if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_LoadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                    {
                        currentTagDirData = Tags.LoadTagsData(false);
                        currentPortraitTagsData = Helpers.LoadAllPortraitsTags(currentTagDirData, true);
                    }
                    if (currentPortraitTagsData.Count > 0)
                    {
                        GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTotal"], currentPortraitTagsData.Count));
                        GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTagged"], currentPortraitTagsData.Where(
                            kvp => kvp.Value.tags.Count() > 0).ToList().Count));
                    }
                }
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ExportHint"]);
                    if (currentPortraitTagsData.Count > 0)
                    {
                        if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_Export"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            exportMessage = Helpers.ExportPortraits(currentPortraitTagsData);
                        }
                        if (exportMessage != "")
                            GUILayout.Label(exportMessage);
                    }

                }
                //Importing portrait pack
                if (currentPortraitTagsData.Count > 0)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ImportingData"]);
                        if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_LoadPortraitsImport"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            importingTagsData = Tags.LoadTagsData(true);
                            importingPortraitTagsData = Helpers.ImportPortraitsTags(importingTagsData, true);
                        }
                        if (importingPortraitTagsData.Count > 0)
                        {
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTotal"], importingPortraitTagsData.Count));
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTagged"], importingPortraitTagsData.Where(
                                kvp => kvp.Value.tags.Count() > 0).ToList().Count));
                            //TODO: duplicates with existing by Hash
                            //TODO: duplicates with existing by ID
                        }
                    }
                    using (new GUILayout.VerticalScope())
                    {
                        //TODO: Toggle Owervrite existing portraits with the same ID, disabled
                        //TODO: Toggle Ignore portraits with the same hash, enabled
                        //TODO: Import function.
                    }

                }
            }
        }
    }
}
