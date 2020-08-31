using Kingmaker;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static KingmakerPortraitManager.Main;

namespace KingmakerPortraitManager.Utility
{
    public static class StatusWrapper
    {
        public static bool IsEnabled()
        {
            return Mod.Enabled;
        }

        public static bool canSelectPortrait()
        {
            bool result;
            result = Game.Instance.UI.CharacterBuildController?.CurrentPhase.GetValueOrDefault() == Kingmaker.UI.LevelUp.Phase.CharBPhase.Type.Portrait;
            return result;
        }

    }
}
