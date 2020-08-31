using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Kingmaker;
using KingmakerPortraitManager.Menu;
using static KingmakerPortraitManager.Main;
using KingmakerPortraitManager.UI;

namespace KingmakerPortraitManager.HarmonyPatches
{
    static class PortraitManagerPatcher
    {
        [HarmonyLib.HarmonyPatch(typeof(CustomPortraitsManager), "GetExistingCustomPortraitIds")]
        static class CustomPortraitsManager_GetExistingCustomPortraitIds_Patch
        { 
            static void Postfix(ref string[] __result)
            {
                if (Mod == null || !Mod.Enabled || PortraitTagSelector.portraitIDsUI == null) return;
                if (PortraitTagSelector.portraitIDsUI.Length > 0)
                {
                    try
                    {
                        __result = PortraitTagSelector.portraitIDsUI;
                    }
                    catch (Exception e)
                    {
                        ModEntry.Logger.Log(e.ToString());
                    }
                }

            }
        }

    }
}
