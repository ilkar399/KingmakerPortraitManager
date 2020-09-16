using Kingmaker;
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
