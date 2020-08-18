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

namespace KingmakerPortraitManager.Menu
{
    class Settings: IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_Settings"];
        public int Priority => 300;
        private GUIStyle _buttonStyle;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled) return;
            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
#if (DEBUG)
            if (GUILayout.Button("Run tests", _buttonStyle, GUILayout.ExpandWidth(false)))
            {
                //TODO: Add tessssssssts. Separate for data getters, tag management, harmony patching and UI patching.
            }
#endif
        }

    }
}
