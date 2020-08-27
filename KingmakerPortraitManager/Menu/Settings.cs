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
using static KingmakerPortraitManager.Utility.SettingsWrapper;
using static KingmakerPortraitManager.Tests;

namespace KingmakerPortraitManager.Menu
{
    class Settings: IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_Settings"];
        public int Priority => 300;
        private GUIStyle _buttonStyle;
        private GUIStyle _toggleStyle;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
            if (_toggleStyle == null)
                _toggleStyle = new GUIStyle(GUI.skin.toggle) { wordWrap = true };
#if (DEBUG)
            if (GUILayout.Button("Run tests", _buttonStyle, GUILayout.ExpandWidth(false)))
            {
                if (Tests.AllTests()) { Mod.Log("Tests are successfull!"); }
                //TODO: Add tessssssssts. Separate for data getters, tag management, harmony patching and UI patching.
            }
#endif
            using (new GUILayout.VerticalScope("box"))
            {
                ToggleIgnoreDefaultPortraits = GUILayout.Toggle(ToggleIgnoreDefaultPortraits,Local["Menu_Settings_ToggleIgnoreDefaultPortraits"],Array.Empty<GUILayoutOption>());
            }
        }

    }
}
