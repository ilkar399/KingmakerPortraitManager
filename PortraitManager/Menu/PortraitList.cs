using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using ModMaker;
using ModMaker.Utility;
using static PortraitManager.Main;
using static PortraitManager.Helpers;
using static PortraitManager.Utility.SettingsWrapper;
using Kingmaker.Blueprints;

namespace PortraitManager.Menu
{
    class PortraitList : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_PortraitList"];
        public int Priority => 100;
        internal List<PortraitData> portraitDatas;
        private int portraitIndex;
        private string[] portraitIDs;
        private PortraitData portraitData;
        private GUIStyle _buttonStyle;

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
                        portraitDatas = new List<PortraitData>();
                        portraitDatas = Helpers.LoadAllCustomPortraits();
                        portraitIDs = portraitDatas.Select(type => type?.CustomId).ToArray();
                        portraitIndex = 0;
                        portraitData = portraitDatas[0];
                    }
                    modEntry.Logger.Log($"portraitDatas count: {portraitDatas.Count}");
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_UnloadPortraits"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    //TODO
                }
                if (GUILayout.Button(Local["Menu_PortraitList_Btn_SavePortraitData"], _buttonStyle, GUILayout.ExpandWidth(false)))
                {
                    //TODO
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
                        GUIHelper.SelectionGrid(ref portraitIndex, portraitIDs, 1,
                            () =>
                            {
                                portraitData = portraitDatas[portraitIndex];
                            }, _buttonStyle, GUILayout.ExpandWidth(false));
                    }
                    using (new GUILayout.VerticalScope())
                    {
                        //TODO: Readonly labels IsCustom, IsDefault, CustomID
                        //TOOD: 
                        //TODO: Buttons Open folder, Add tag, Remove tag, Clear tags, Save data
                        GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_IsCustom"], portraitData.IsCustom));
                        GUILayout.Label(string.Format(Local["Menu_PortraitList_Lbl_PortraitID"], portraitData.CustomId));
                        if (GUILayout.Button(Local["Menu_PortraitList_Btn_OpenFolder"], _buttonStyle, GUILayout.ExpandWidth(false)))
                        {
                            if (CustomPortraitsManager.Instance == null)
                            {
                                return;
                            }
                            CustomPortraitsManager.Instance.OpenPortraitFolder(portraitData.CustomId);
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
