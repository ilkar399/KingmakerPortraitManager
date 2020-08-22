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
using Kingmaker.Utility;
using HarmonyLib;

namespace KingmakerPortraitManager.Menu
{
    class PortraitList : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_PortraitList"];
        public int Priority => 100;
        internal List<PortraitData> portraitsData;
        internal Dictionary<string, TagData> tagsData;
        private int portraitIndex;
        private string[] portraitIDs;
        private int _tagIndex;
        private string inputTagName;
        internal string[] _tagList;
        internal TagData _tagData; 
        private PortraitData portraitData;
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
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_LoadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    portraitsData = new List<PortraitData>();
                    portraitsData = Helpers.LoadAllCustomPortraits(ToggleIgnoreDefaultPortraits);
                    portraitIDs = portraitsData.Select(type => type?.CustomId).ToArray();
                    portraitIndex = -1;
                    portraitData = null;
                    tagsData = Tags.LoadTagsData();
                    _tagList = new string[] { };
                    _tagData = null;
#if (DEBUG)
                    modEntry.Logger.Log($"portraitDatas count: {portraitsData.Count}");
#endif
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_UnloadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    portraitData = null;
                    portraitIDs = null;
                    portraitsData = null;
                    tagsData = null;
                    _tagList = null;
                    _tagData = null;
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitDataAll"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    if (tagsData != null)
                    {
                        Tags.SaveTagsData(tagsData);
                    }
                }
            }
            if (portraitIDs != null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    //1st column - portrait list
                    //2rd column - buttons. Open folder. Tags. Clear tags.
                    //3nd column - portrait image
                    using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(false)))
                    {
                        using (var ScrollView = new GUILayout.ScrollViewScope(_scrollPosition))
                        {
                            _scrollPosition = ScrollView.scrollPosition;
                            GUIHelper.SelectionGrid(ref portraitIndex, portraitIDs, 1,
                                () =>
                                {
                                    if (_tagData != null)
                                        {
                                            //TODO check overwrite settings
                                            _tagData.Hash = Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture).ToString();
                                            tagsData[portraitData.CustomId] = _tagData;
#if (DEBUG)
                                            modEntry.Logger.Log($"Saved {portraitData.CustomId}");
#endif
                                    }
                                    portraitData = portraitsData[portraitIndex];
                                    inputTagName = "";
                                    if (tagsData.ContainsKey(portraitData.CustomId))
                                    {
                                        _tagData = tagsData[portraitData.CustomId];
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

                        using (new GUILayout.VerticalScope())
                        {
                            //TODO: labels hash (colored if it's wrong)
                            //TOOD: 
                            //TODO: Buttons Open folder, Add tag, Clear tags, Save data. List of tags with an "X" that remove said tag
                            GUILayout.Label(Local["Menu_PortraitList_Lbl_tagList"]);
                            if (_tagList.Length > 0)
                            {
                                try
                                {
                                    //tag selection and removal
                                    GUIHelper.SelectionGrid(ref _tagIndex, _tagList, 2, () =>
                                    {
                                        if (_tagIndex >= 0 && _tagIndex < _tagData.tags.Count)
                                        {
                                            _tagData.tags.RemoveAt(_tagIndex);
                                            _tagList = Helpers.RemoveIndices(_tagList, _tagIndex);
                                            _tagIndex = -1;
                                        }
                                    }, _fixedStyle, GUILayout.ExpandWidth(false));
                                }
                                catch (Exception e)
                                {
                                    modEntry.Logger.Error(e.StackTrace);
                                    throw e;
                                }
                            }
                            GUILayout.Label(Local["Menu_PortraitList_Lbl_tagMsg"]);
                            using (new GUILayout.HorizontalScope())
                            {
                                GUIHelper.TextField(ref inputTagName, _fixedStyle);
                                if (GUILayout.Button(Local["Menu_PortraitList_Btn_AddTag"], _buttonStyle, GUILayout.ExpandWidth(false)))
                                {
                                    if (inputTagName != "" && !(_tagData.tags.Contains(inputTagName.ToLower())))
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
                                if (tagsData.ContainsKey(portraitData.CustomId))
                                {
                                    _tagData = tagsData[portraitData.CustomId];
                                    _tagList = _tagData.tags.ToArray(); ;
                                }
                                else
                                {
                                    _tagData = new TagData(
                                        Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture).ToString(),
                                        portraitData.CustomId,
                                        new List<string>());
                                    _tagList = new string[] { };
                                }
                            }
                            if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitData"], _fixedStyle, GUILayout.ExpandWidth(false)))
                            {
                                _tagData.SaveData();
                            }
                            GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_IsCustom"], portraitData.IsCustom));
                            GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_PortraitID"], portraitData.CustomId));
                            //Note: hash is calculated only based on the FulLengthPortrait
                            GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_Hash"], Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture)));
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

    }
}
