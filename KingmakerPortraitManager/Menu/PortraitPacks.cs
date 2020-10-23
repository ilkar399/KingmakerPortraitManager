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
        private Dictionary<string, bool> exportTagList;
        private Dictionary<string, bool> importTagList;
        private Dictionary<string, TagData> allTagDirData;
        private Dictionary<string, TagData> allPortraitTagsData;
        private Dictionary<string, TagData> currentPortraitTagsData;
        private Dictionary<string, TagData> importingTagsData;
        private Dictionary<string, TagData> importingPortraitTagsData;
        private Dictionary<string, TagData> importingSelectedTagsData;


        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            string defaultHash = Helpers.GetPseudoHash(BlueprintRoot.Instance.CharGen.BasePortraitBig.texture).ToString();
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            if (allPortraitTagsData == null)
                allPortraitTagsData = new Dictionary<string, TagData>();
            if (currentPortraitTagsData == null)
                currentPortraitTagsData = new Dictionary<string, TagData>();
            if (importingPortraitTagsData == null)
                importingPortraitTagsData = new Dictionary<string, TagData>();
            if (exportMessage == null)
                exportMessage = "";
            if (importMessage == null)
                importMessage = "";
            GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ExportHeader"]);
            GUILayout.Space(10f);
            using (new GUILayout.HorizontalScope())
            {
                //Exporting portraits
                //TODO: Remove duplicates
                //All portrait data
                using (new GUILayout.VerticalScope(GUILayout.MaxWidth(250), GUILayout.ExpandWidth(false)))
                {
                    GUILayout.Label(Local["Menu_PortraitPacks_Lbl_CurrentData"]);
                    if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_LoadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                    {
                        allTagDirData = Tags.LoadTagsData(false);
                        allPortraitTagsData = Helpers.LoadAllPortraitsTags(allTagDirData, true);
                    }
                    if (allPortraitTagsData.Count > 0)
                    {
                        GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTotal"], allPortraitTagsData.Count()));
                        GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsTagged"], allPortraitTagsData.Where(
                            kvp => kvp.Value.tags.Count() > 0).ToList().Count()));
                    }
                }
                //Tag selector
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ExportHint"]);
                    if (allPortraitTagsData.Count > 0)
                    {
                        if (exportTagList == null)
                            exportTagList = Tags.AllTagsFilter(allPortraitTagsData);
                        GUILayout.Label(Local["Menu_PortraitList_Lbl_Filters"]);
                        using (new GUILayout.HorizontalScope())
                        {
                            var exportFilterKeys = new List<string>(exportTagList.Keys);
                            foreach (string filterName in exportFilterKeys)
                            {
                                bool FilterValue = exportTagList[filterName];
                                GUIHelper.ToggleButton(ref FilterValue, filterName, () =>
                                {
                                    currentPortraitTagsData = Helpers.FilterPortraitData(filterName, FilterValue, allPortraitTagsData, exportTagList);
                                }, () =>
                                {
                                    currentPortraitTagsData = Helpers.FilterPortraitData(filterName, FilterValue, allPortraitTagsData, exportTagList);
                                },
                                _fixedStyle);
                                exportTagList[filterName] = FilterValue;
                            }
                        }
                        if (currentPortraitTagsData != null)
                        {
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsSelected"], currentPortraitTagsData.Count()));
                        }
                    }
                }
                //Exporting
                using (new GUILayout.VerticalScope(GUILayout.MaxWidth(250), GUILayout.ExpandWidth(false)))
                {
                    if (allPortraitTagsData.Count > 0)
                    {
                        if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_Export"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            if (currentPortraitTagsData != null)
                                exportMessage = Helpers.ExportPortraits(currentPortraitTagsData);
                            else
                                exportMessage = Helpers.ExportPortraits(allPortraitTagsData);
                        }
                        if (exportMessage != "")
                            GUILayout.Label(exportMessage);
                    }

                }
            }
            GUILayout.Space(10f);
            GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ImportHeader"]);
            GUILayout.Space(10f);
            using (new GUILayout.HorizontalScope())
            {
                //Importing portrait pack
                if (allPortraitTagsData.Count > 0)
                {
                    //Loading data from import folder
                    using (new GUILayout.VerticalScope(GUILayout.MaxWidth(250), GUILayout.ExpandWidth(false)))
                    {
                        GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ImportingData"]);
                        if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_LoadPortraitsImport"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            importingTagsData = Tags.LoadTagsData(true);
                            importingPortraitTagsData = Helpers.ImportPortraitsTags(importingTagsData, false);
#if (DEBUG)
                            foreach (var kvp in importingPortraitTagsData)
                            {
                                Mod.Log(string.Join(", ",kvp.Key, kvp.Value.Hash, string.Join(",", kvp.Value.tags)));
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
                            IEnumerable<string> idDuplicates = allPortraitTagsData.Keys.Intersect(importingPortraitTagsData.Keys);
                            GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsDuplicateHash"],
                                Tags.HashDuplicatesTagDictionary(allPortraitTagsData, importingPortraitTagsData)));
                        }
                    }
                    //Tag selector
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label(Local["Menu_PortraitPacks_Lbl_ImportHint"]);
                        if (importingPortraitTagsData.Count > 0)
                        {
                            if (importTagList == null)
                                importTagList = Tags.AllTagsFilter(importingPortraitTagsData);
                            GUILayout.Label(Local["Menu_PortraitList_Lbl_Filters"]);
                            using (new GUILayout.HorizontalScope())
                            {
                                var filterKeys = new List<string>(importTagList.Keys);
                                foreach (string filterName in filterKeys)
                                {
                                    bool FilterValue = importTagList[filterName];
                                    GUIHelper.ToggleButton(ref FilterValue, filterName, () =>
                                    {
                                        importingSelectedTagsData = Helpers.FilterPortraitData(filterName, FilterValue, importingPortraitTagsData, importTagList);
                                    }, () =>
                                    {
                                        importingSelectedTagsData = Helpers.FilterPortraitData(filterName, FilterValue, importingPortraitTagsData, importTagList);
                                    },
                                    _fixedStyle);
                                    importTagList[filterName] = FilterValue;
                                }
                            }
                            if (importingSelectedTagsData != null)
                            {
                                GUILayout.Label(string.Format(Local["Menu_PortraitPacks_Lbl_PortraitsSelected"], importingSelectedTagsData.Count()));
                            }
                        }
                    }
                    //Importing data
                    using (new GUILayout.VerticalScope(GUILayout.MaxWidth(250), GUILayout.ExpandWidth(false)))
                    {
                        GUIHelper.ToggleButton(ref MergeTags, Local["Menu_PortraitPacks_Tgl_MergeTags"]);
                        if (GUILayout.Button(Local["Menu_PortraitPacks_Btn_Import"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            if (importingSelectedTagsData != null)
                                if (importingSelectedTagsData.Count > 0)
                                    importMessage = Helpers.ImportPortraits(allPortraitTagsData, importingSelectedTagsData, MergeTags);
                            else
                                importMessage = Helpers.ImportPortraits(allPortraitTagsData, importingPortraitTagsData, MergeTags);
                            PortraitList.Unload();
                        }
                        if (importMessage != "")
                            GUILayout.Label(importMessage);
                    }
                }

            }
        }
    }
}
