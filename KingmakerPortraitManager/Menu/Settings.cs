using System;
using System.IO;
using UnityEngine;
using UnityModManagerNet;
using ModMaker;
using static KingmakerPortraitManager.Main;
using static KingmakerPortraitManager.Utility.SettingsWrapper;
using static KingmakerPortraitManager.Tests;

namespace KingmakerPortraitManager.Menu
{
    class Settings: IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_Settings"];
        public int Priority => 300;
        private GUIStyle _buttonStyle;
        private GUIStyle _fixedStyle;
        private GUIStyle _toggleStyle;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            if (_toggleStyle == null)
                _toggleStyle = new GUIStyle(GUI.skin.toggle) { wordWrap = true };
            if (_fixedStyle == null)
                _fixedStyle = new GUIStyle(GUI.skin.button) { fixedWidth = 150f, wordWrap = true };
#if (DEBUG)
            if (GUILayout.Button("Run tests", _buttonStyle, GUILayout.ExpandWidth(false)))
            {
                if (Tests.AllTests()) { Mod.Log("Tests are successfull!"); }
                //TODO: Add tessssssssts. Separate for data getters, tag management, harmony patching and UI patching.
            }
#endif
            using (new GUILayout.VerticalScope("box"))
            {
                GUILayout.Space(10f);
                if (GUILayout.Button(Local["Menu_Settings_Btn_ClearTags"], _fixedStyle, GUILayout.ExpandWidth(false)))
                {
                    try
                    {
                        if (Directory.Exists(Path.Combine(ModPath,@"/tags/")))
                        {
                            var dir = new DirectoryInfo(Path.Combine(ModPath, @"/tags/"));
                            foreach (var file in dir.EnumerateFiles("*.json"))
                            {
                                file.Delete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModEntry.Logger.Error(ex.StackTrace);
                        throw ex;
                    }
                }
                ToggleIgnoreDefaultPortraits = GUILayout.Toggle(ToggleIgnoreDefaultPortraits,Local["Menu_Settings_ToggleIgnoreDefaultPortraits"],Array.Empty<GUILayoutOption>());
            }
        }

    }
}
