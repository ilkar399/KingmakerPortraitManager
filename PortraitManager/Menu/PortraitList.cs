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
    class PortraitList : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_PortraitList"];
        public int Priority => 100;
        internal List<PortraitData> portraitsData;
        internal Dictionary<string, TagData> tagsData;
        private int portraitIndex;
        private string[] portraitIDs;
        private PortraitData portraitData;
        private GUIStyle _buttonStyle;
        private Vector2 _scrollPosition;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_LoadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    if (portraitIDs == null)
                    {
                        portraitsData = new List<PortraitData>();
                        portraitsData = Helpers.LoadAllCustomPortraits(ToggleIgnoreDefaultPortraits);
                        portraitIDs = portraitsData.Select(type => type?.CustomId).ToArray();
                        portraitIndex = 0;
                        portraitData = portraitsData[0];
                    }
                    modEntry.Logger.Log($"portraitDatas count: {portraitsData.Count}");
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_UnloadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    portraitData = null;
                    portraitIDs = null;
                    portraitsData = null;
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitData"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    //TODO
                }
            }
            if (portraitIDs != null)
            {
                tagsData = Tags.LoadTagsData();
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
                                    portraitData = portraitsData[portraitIndex];
                                }, _buttonStyle, GUILayout.ExpandWidth(false));
                        }
                    }
                    using (new GUILayout.VerticalScope())
                    {
                        //TODO: labels hash (colored if it's wrong)
                        //TOOD: 
                        //TODO: Buttons Open folder, Add tag, Clear tags, Save data. List of tags with an "X" that remove said tag
                        GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_IsCustom"], portraitData.IsCustom));
                        GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_PortraitID"], portraitData.CustomId));
                        //Note: hash is calculated only based on the FulLengthPortrait
                        GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_Hash"], Helpers.GetPseudoHash(portraitData.FullLengthPortrait.texture)));
                        if (GUILayout.Button(Local["Menu_PortraitList_Btn_OpenFolder"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            if (CustomPortraitsManager.Instance == null)
                            {
                                return;
                            }
                            CustomPortraitsManager.Instance.OpenPortraitFolder(portraitData.CustomId);
                        }
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label(Local["Menu_PortraitList_Lbl_tagMsg"]);

                        }
                        if (GUILayout.Button(Local["Menu_PortraitList_Btn_AddTag"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {

                        }
                        if (GUILayout.Button(Local["Menu_PortraitList_Btn_RemoveTag"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {

                        }
                        if (GUILayout.Button(Local["Menu_PortraitList_Btn_ClearTags"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {

                        }
                        if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitData"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {

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
                            //                               modEntry.Logger.Error("Error getting portrait texture");
                            return;
                        }
                    }
                }
            }
        }

    }
}
