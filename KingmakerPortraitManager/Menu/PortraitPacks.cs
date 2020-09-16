using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using Kingmaker.Blueprints.Root;
using ModMaker;
using ModMaker.Utility;
using static KingmakerPortraitManager.Main;

namespace KingmakerPortraitManager.Menu
{
    //Export+Import of Portrait Packs
    class PortraitPacks : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_PortraitPacks"];
        public int Priority => 200;
        private bool MergeTags = true;
        private string exportMessage;
        private string importMessage;
        private GUIStyle _buttonStyle;
        private GUIStyle _fixedStyle;
        private Dictionary<string, TagData> currentTagDirData;
        private Dictionary<string, TagData> currentPortraitTagsData;
        private Dictionary<string, TagData> importingTagsData;
        private Dictionary<string, TagData> importingPortraitTagsData;


        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            string defaultHash = Helpers.GetPseudoHash(BlueprintRoot.Instance.CharGen.BasePortraitBig.texture).ToString();
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            if (currentPortraitTagsData == null)
                currentPortraitTagsData = new Dictionary<string, TagData>();
            if (importingPortraitTagsData == null)
                importingPortraitTagsData = new Dictionary<string, TagData>();
            if (exportMessage == null)
                exportMessage = "";
            if (importMessage == null)
                importMessage = "";
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
                        GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTotal"], currentPortraitTagsData.Count()));
                        GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTagged"], currentPortraitTagsData.Where(
                            kvp => kvp.Value.tags.Count() > 0).ToList().Count()));
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
            }
            using (new GUILayout.HorizontalScope())
            {
                //Importing portrait pack
                if (currentPortraitTagsData.Count > 0)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ImportingData"]);
                        if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_LoadPortraitsImport"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            importingTagsData = Tags.LoadTagsData(true);
                            importingPortraitTagsData = Helpers.ImportPortraitsTags(importingTagsData, false);
#if (DEBUG)
                            foreach (var kvp in importingPortraitTagsData)
                            {
                                Mod.Log(kvp.Key);
                                Mod.Log(kvp.Value.Hash);
                                Mod.Log(string.Join(",",kvp.Value.tags));
                            }
#endif
                        }
                        if (importingPortraitTagsData.Count > 0)
                        {
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTotal"], importingPortraitTagsData.Count()));
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsError"], importingPortraitTagsData.Where(
                                   kvp => kvp.Value.Hash == defaultHash).Count()));
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTagged"], importingPortraitTagsData.Where(
                                kvp => kvp.Value.tags.Count() > 0).ToList().Count));
                            IEnumerable<string> idDuplicates = currentPortraitTagsData.Keys.Intersect(importingPortraitTagsData.Keys);
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsDuplicateHash"],
                                Tags.HashDuplicatesTagDictionary(currentPortraitTagsData,importingPortraitTagsData)));
                        }
                    }
                    if (importingPortraitTagsData.Count > 0)
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            GUIHelper.ToggleButton(ref MergeTags, Local["Menu_PortraitPacks_Tgl_MergeTags"]);
                            if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_Import"], _buttonStyle, GUILayout.ExpandWidth(false)))
                            {
                                importMessage = Helpers.ImportPortraits(currentPortraitTagsData, importingPortraitTagsData, MergeTags);
                                if (exportMessage != "")
                                    GUILayout.Label(exportMessage);
                                PortraitList.Unload();
                            }
                            //TODO: Import function.
                        }
                    }
                }

            }
        }
    }
}
