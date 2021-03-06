﻿using System.Collections.Generic;
using UnityEngine;
using static KingmakerPortraitManager.Main;

namespace KingmakerPortraitManager.Utility
{
    public static class SettingsWrapper
    {
        public static string ModPath
        {
            get => Mod.Settings.modPath;
            set => Mod.Settings.modPath = value;
        }

        public static string LocalizationFileName
        {
            get => Mod.Settings.localizationFileName;
            set => Mod.Settings.localizationFileName = value;
        }

        public static bool ToggleIgnoreDefaultPortraits
        {
            get => Mod.Settings.ToggleIgnoreDefaultPortraits;
            set => Mod.Settings.ToggleIgnoreDefaultPortraits = value;
        }

    }

}
