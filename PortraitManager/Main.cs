using System;
using System.Reflection;
using Kingmaker.Blueprints;
using UnityModManagerNet;
using ModMaker;
using ModMaker.Utility;
using static PortraitManager.Utility.SettingsWrapper;


namespace PortraitManager
{
#if (DEBUG)
    [EnableReloading]
#endif

    static class Main
    {
        public static LocalizationManager<DefaultLanguage> Local;
        public static ModManager<Core, Settings> Mod;
        public static MenuManager Menu;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            //HarmonyLib.Harmony.DEBUG = true;
            Local = new LocalizationManager<DefaultLanguage>();
            Mod = new ModManager<Core, Settings>();
            Menu = new MenuManager();
            modEntry.OnToggle = OnToggle;
            modEntry.OnToggle = OnToggle;
#if (DEBUG)
            modEntry.OnUnload = Unload;
            return true;
        }

        static bool Unload(UnityModManager.ModEntry modEntry)
        {
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
