﻿using System;
using System.Reflection;
using Kingmaker.Blueprints;
using UnityModManagerNet;
using ModMaker;
using ModMaker.Utility;
using static KingmakerPortraitManager.Utility.SettingsWrapper;
using UnityEngine;
using HarmonyLib;

namespace KingmakerPortraitManager
{
#if (DEBUG)
    [EnableReloading]
#endif

    static class Main
    {
        public static LocalizationManager<DefaultLanguage> Local;
        public static ModManager<Core, Settings> Mod;
        public static MenuManager Menu;
        public static UnityModManager.ModEntry ModEntry;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
#if (DEBUG)
                HarmonyLib.Harmony.DEBUG = true;
#endif
                Local = new LocalizationManager<DefaultLanguage>();
                Mod = new ModManager<Core, Settings>();
                Menu = new MenuManager();
                ModEntry = modEntry;
                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                modEntry.OnToggle = OnToggle;
#if (DEBUG)
                modEntry.OnUnload = Unload;
            }
            catch (Exception e)
            {
                modEntry.Logger.Critical(e.ToString() + "\n" + e.StackTrace);
                throw e;
            }
            return true;
        }

        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.UnpatchAll(modEntry.Info.Id);
            Mod.Disable(modEntry, true);
            Menu = null;
            Mod = null;
            Local = null;
            return true;
        }
#else
            return true;
        }
#endif
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            try
            {
                if (value)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Local.Enable(modEntry);
                    Mod.Enable(modEntry, assembly);
                    Menu.Enable(modEntry, assembly);
                    ModPath = modEntry.Path;
                }
                else
                {
                    Menu.Disable(modEntry);
                    Mod.Disable(modEntry, false);
                    Local.Disable(modEntry);
                    ReflectionCache.Clear();
                }
            }
            catch (Exception ex)
            {
                Mod.Error(ex.Message + ex.StackTrace);
            }
            return true;
        }
    }

}