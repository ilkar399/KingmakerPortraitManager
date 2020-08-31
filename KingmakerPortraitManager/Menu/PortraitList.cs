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
using KingmakerPortraitManager.UI;
using static KingmakerPortraitManager.Utility.SettingsWrapper;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.Blueprints;
using Kingmaker;
using Kingmaker.UI;
using Kingmaker.Utility;
using static ModMaker.Utility.RichTextExtensions;
using System.Security.Cryptography;
using Kingmaker.UI.SettingsUI;
using Kingmaker.UI.LevelUp;
using Kingmaker.UI.LevelUp.Phase;
using TinyJson;

namespace KingmakerPortraitManager.Menu
{
    class PortraitList : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_PortraitList"];
        public int Priority => 100;
        internal static string[] ReservedTags = { "", "all", "recent"};
        internal static string[] portraitIDs;
        internal static Dictionary<string,TagData> allPortraitsData;
        private static Dictionary<string, TagData> tagsData;
        internal static Dictionary<string,bool> tagListAll;
        private int portraitIndex;
        private int _tagIndex;
        private string inputTagName;
        private static string[] _tagList;
        private static TagData _tagData;
        private static PortraitData portraitData;
        private GUIStyle _buttonStyle;
        private GUIStyle _fixedStyle;
        private Vector2 _scrollPosition;      

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            if (_fixedStyle == null)
                _fixedStyle = new GUIStyle(GUI.skin.button) { fixedWidth = 150f, wordWrap = true };
            //Overall list operations
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(10f);
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_LoadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    tagsData = Tags.LoadTagsData(false);
                    allPortraitsData = new Dictionary<string, TagData>();
                    allPortraitsData = Helpers.LoadAllPortraitsTags(tagsData,ToggleIgnoreDefaultPortraits);
                    portraitIDs = allPortraitsData.Values.Select(type => type?.CustomId).ToArray();
                    portraitIndex = -1;
                    portraitData = null;
                    tagListAll = Tags.AllTagsFilter(tagsData);
                    _tagList = new string[] { };
                    _tagData = null;
#if (DEBUG)
                    modEntry.Logger.Log($"portraitDatas count: {allPortraitsData.Count}");
#endif
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_UnloadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    Unload();
                }
                //Save all non-default data to the filesystem
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitDataAll"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    if (tagsData != null)
                    {
                        var changedTagsData = tagsData.Where(kvp =>
                            {
                            bool tresult = false;
                            if (allPortraitsData.ContainsKey(kvp.Key))
                            {
                                IEnumerable<string> tdifference = allPortraitsData[kvp.Key].tags.Except(kvp.Value.tags);
                                    if (!tdifference.Any())
                                    {
                                        tresult = true;
                                    }
                                }
                                return tresult;
                            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                        Tags.SaveTagsData(changedTagsData, false);
                        allPortraitsData = Helpers.LoadAllPortraitsTags(tagsData, ToggleIgnoreDefaultPortraits);
                        tagListAll = Tags.AllTagsFilter(tagsData);
                    }
                }
            }
            //Filter portrait list by tags
            if (tagListAll != null)
            {
                GUILayout.Label(Local["Menu_PortraitList_Lbl_Filters"]);
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_AllTags"],_fixedStyle,GUILayout.ExpandWidth(false)))
                 {
                    portraitIDs = allPortraitsData.Values.Select(type => type?.CustomId).ToArray();
                    tagListAll = tagListAll.ToDictionary(p => p.Key, p => false);
                    portraitIndex = -1;
                    portraitData = null;
                    _tagList = new string[] { };
                    _tagData = null;
                }
                //Apply filters to the game UI. Requires working harmony patch and a published assembly to complie
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_ApplyFilters"], _fixedStyle, GUILayout.ExpandWidth(false)))
                {
                    PortraitTagSelector.portraitIDsUI = new string[portraitIDs.Count()];
                    portraitIDs.CopyTo(PortraitTagSelector.portraitIDsUI, 0);
                    if (Game.Instance.IsControllerMouse)
                    {
                        if (Game.Instance.UI.CharacterBuildController.Portrait != null) 
                        {
                            if (Game.Instance.UI.CharacterBuildController.Portrait.IsUnlocked)
                            {
                                Game.Instance?.UI.CharacterBuildController.Portrait.PortraitSelector.HandleClickUpload(false);
                            }
                            else
                            {
                                UIHelpers.KbmUpdateCustomPortraits();
                            }
                        }
                    }
                    //Reserved for when you'll be able to select a custom portrait from the gamepad UI
/*                    if (Game.Instance.IsControllerGamepad)
                    {
                        
                    }*/
                }
                //Portrait filters
                using (new GUILayout.HorizontalScope())
                {
                    var filterKeys = new List<string> (tagListAll.Keys);
                    foreach (string filterName in filterKeys)
                    {
                        bool FilterValue = tagListAll[filterName];
                        GUIHelper.ToggleButton(ref FilterValue, filterName,() =>
                        {
                            portraitIDs = Helpers.FilterPortraitIDs(filterName, FilterValue, allPortraitsData, tagListAll);
                            portraitIndex = -1;
                            portraitData = null;
                            _tagList = new string[] { };
                            _tagData = null;
                        },() =>
                        {
                            portraitIDs = Helpers.FilterPortraitIDs(filterName, FilterValue, allPortraitsData, tagListAll);
                            portraitIndex = -1;
                            portraitData = null;
                            _tagList = new string[] { };
                            _tagData = null;
                        },
                        _fixedStyle);
                        tagListAll[filterName] = FilterValue;
                    }
                }
            }
            //Portraits list
            //1st column - portrait list
            //2rd column - buttons. Open folder. Tags. Clear tags.
            //3nd column - portrait image
            if (portraitIDs != null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    using (new GUILayout.VerticalScope(GUILayout.Width(120)))
                    {
                        GUILayout.Label(Local["Menu_PortraitList_Lbl_PortraitListHeader"]);
                        using (var ScrollView = new GUILayout.ScrollViewScope(_scrollPosition))
                        {
                            _scrollPosition = ScrollView.scrollPosition;
                            GUIHelper.SelectionGrid(ref portraitIndex, portraitIDs, 1,
                                () =>
                                {
                                    if (_tagData != null)
                                    {
                                        //TODO check overwrite settings
 //                                       _tagData.Hash = allPortraitsData.Find(tag => tag.CustomId == portraitData.CustomId).Hash;
                                        _tagData.Hash = Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture).ToString();
                                        tagsData[portraitData.CustomId] = _tagData;
                                    }
                                    portraitData = Helpers.LoadPortraitData(allPortraitsData[portraitIDs[portraitIndex]].CustomId);
                                    inputTagName = "";
                                    if (tagsData.ContainsKey(portraitData.CustomId))
                                    {
                                        _tagData = new TagData(
                                            Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture).ToString(),
                                            portraitData.CustomId,
                                            tagsData[portraitData.CustomId].tags);
//                                        _tagData = tagsData[portraitData.CustomId];
                                        _tagList = _tagData.tags.ToArray();
                                    }
                                    else
                                    {
                                        _tagData = new TagData(
                                            Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture).ToString(),
                                            portraitData.CustomId,
                                            new List<string>());
                                        _tagList = new string[] { };
                                    }
                                    _tagIndex = -1;
                                }, _buttonStyle, GUILayout.ExpandWidth(false));
                        }
                    }
                    if (portraitData != null)
                    {

                        using (new GUILayout.VerticalScope(GUILayout.Width(350)))
                        {
                            GUILayout.Label(Local["Menu_PortraitList_Lbl_tagList"]);                            
                            using (new GUILayout.HorizontalScope())
                            {
                                var allToggleTags = new List<string> (tagListAll.Keys);
                                foreach (string toggleTagName in allToggleTags)
                                {
                                    bool tagToggleValue = _tagData.tags.Contains(toggleTagName);
                                    GUIHelper.ToggleButton(ref tagToggleValue, toggleTagName,() =>
                                    {
                                        if (!_tagData.tags.Contains(toggleTagName))
                                            _tagData.tags.Add(toggleTagName);
                                    },() =>
                                    {
                                        if (_tagData.tags.Contains(toggleTagName))
                                            _tagData.tags.Remove(toggleTagName);
                                    },
                                    _fixedStyle);
                                }
                            }
                            GUILayout.Label(Local["Menu_PortraitList_Lbl_tagMsg"]);
                            using (new GUILayout.HorizontalScope())
                            {
                                GUIHelper.TextField(ref inputTagName, _fixedStyle);
                                if (GUILayout.Button(Local["Menu_PortraitList_Btn_AddTag"], _buttonStyle, GUILayout.ExpandWidth(false)))
                                {
                                    if (Array.IndexOf(ReservedTags,inputTagName.ToLower())<0 && !(_tagData.tags.Contains(inputTagName.ToLower())))
                                    {
                                        _tagData.tags.Add(inputTagName.ToLower());
                                        _tagList = _tagList.Concat(new string[] { inputTagName.ToLower() }).ToArray();
                                    }
                                }
                            }
                            if (GUILayout.Button(Local["Menu_PortraitList_Btn_ClearTags"], _fixedStyle, GUILayout.ExpandWidth(false)))
                            {
                                _tagData.tags.Clear();
                                _tagList = new string[] { };
                                inputTagName = "";
                                _tagIndex = -1;
                            }
                            if (GUILayout.Button(Local["Menu_PortraitList_Btn_CancelPortrait"], _fixedStyle, GUILayout.ExpandWidth(false)))
                            {
                                _tagData = new TagData(
                                    _tagData.Hash,
                                    _tagData.CustomId,
                                    new List<string>(allPortraitsData[portraitData.CustomId].tags));
                                modEntry.Logger.Log(string.Join(", ",_tagData.tags.ToArray()));
                                _tagList = _tagData.tags.ToArray();
                            }
                            if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitData"], _fixedStyle, GUILayout.ExpandWidth(false)))
                            {
                                _tagData.SaveData(false);
                                tagsData[_tagData.CustomId] = new TagData(
                                            _tagData.CustomId,
                                            _tagData.Hash,
                                            new List<string>(_tagData.tags));
                                allPortraitsData[_tagData.CustomId] = _tagData;
                                tagListAll = Tags.AllTagsFilter(tagsData);
                            }
                            GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_IsCustom"], portraitData.IsCustom));
                            GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_PortraitID"], portraitData.CustomId));
                            //Note: hash is calculated only based on the FulLengthPortrait
                            string HashText = string.Format(Local["Menu_PortraitList_Lbl_Hash"], Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture));
                            if (!tagsData.ContainsKey(portraitData.CustomId))
                                GUILayout.Label(HashText);
                            else
                                if ((_tagData.Hash == tagsData[portraitData.CustomId].Hash))
                                    GUILayout.Label(HashText);
                                else
                                    GUILayout.Label(HashText.Color(RGBA.red));
                            if (GUILayout.Button(Local["Menu_PortraitList_Btn_OpenFolder"], _fixedStyle, GUILayout.ExpandWidth(false)))
                            {
                                if (CustomPortraitsManager.Instance == null)
                                {
                                    return;
                                }
                                CustomPortraitsManager.Instance.OpenPortraitFolder(portraitData.CustomId);
                            }
                        }
                        
                        using (new GUILayout.VerticalScope())
                        {
                            //TODO: all 3 images or only 1? AreaScopes?
                            Texture tex = portraitData.FullLengthPortrait.texture;
                            if (tex)
                            {
                                GUILayout.Label(Local["Menu_PortraitList_Lbl_PortraitImage"]);
//                                GUI.DrawTexture(new Rect(10, 10, 346, 512), tex, ScaleMode.ScaleToFit, true, 10.0F);
                                GUILayout.Box(tex);
                            }
                            else
                            {
                                modEntry.Logger.Error("Error getting portrait texture");
                                return;
                            }
                        }
                    }
                }
            }
        }

        internal static void Unload()
        {
            allPortraitsData = null;
            portraitData = null;
            portraitIDs = null;
            tagsData = null;
            tagListAll = null;
            _tagList = null;
            _tagData = null;
        }

    }
}
