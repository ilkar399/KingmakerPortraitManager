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

namespace KingmakerPortraitManager.HarmonyPatches
{
    static class PortraitManagerPatcher
    {
        [HarmonyLib.HarmonyPatch(typeof(CustomPortraitsManager), "GetExistingCustomPortraitIds")]
        static class CustomPortraitsManager_GetExistingCustomPortraitIds_Patch
        { 
            static void Postfix(ref string[] __result)
            {
                if (Mod == null || !Mod.Enabled || PortraitList.portraitIDs == null) return;
                if (PortraitList.portraitIDs.Length > 0)
                {
                    try
                    {
                        __result = PortraitList.portraitIDs;
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
