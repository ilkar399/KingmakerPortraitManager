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
        //        private List<string> _tagList;
        internal string[] _tagList;
        private TagData _tagData; 
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
                    if (portraitIDs == null)
                    {
                        portraitsData = new List<PortraitData>();
                        portraitsData = Helpers.LoadAllCustomPortraits(ToggleIgnoreDefaultPortraits);
                        portraitIDs = portraitsData.Select(type => type?.CustomId).ToArray();
                        portraitIndex = -1;
                        portraitData = portraitsData[0];
                        tagsData = Tags.LoadTagsData();
                        _tagList = new string[] { };
                    }
                    modEntry.Logger.Log($"portraitDatas count: {portraitsData.Count}");
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_UnloadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    portraitData = null;
                    portraitIDs = null;
                    portraitsData = null;
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitDataAll"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    if (tagsData != null)
                    {

                    }
                }
            }
            if (portraitIDs != null)
            {
//                _tagList = new List<string>();
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
 //                                       _tagList.Clear();
                                    }
                                    _tagIndex = -1;
                                    modEntry.Logger.Log(portraitData.CustomId);
                                    modEntry.Logger.Log(_tagList.Length.ToString());
                                }, _buttonStyle, GUILayout.ExpandWidth(false));
                        }
                    }
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
                                        //                                        _tagList.RemoveAt(_tagIndex);
                                        _tagData.tags.RemoveAt(_tagIndex);
                                        _tagList = Helpers.RemoveIndices(_tagList, _tagIndex);
                                        modEntry.Logger.Log(_tagList.Length.ToString());
                                        _tagIndex = -1;
                                    }
                                }, _buttonStyle, GUILayout.ExpandWidth(false));
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
                                if (inputTagName != "")
                                {
                                    _tagData.tags.Add(inputTagName.ToLower());
                                    _tagList.AddItem(inputTagName.ToLower());
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
                        //TODO: all 3 images or only 1? AreaScopes.
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
