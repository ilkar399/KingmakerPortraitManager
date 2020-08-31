using System;
using System.Reflection;
using ModMaker;
using KingmakerPortraitManager.Controllers;

namespace KingmakerPortraitManager
{
    class Core
    {
        public int Priority => 0;
        public UIController UI { get; internal set; }

        public static void FailedToPath(MethodBase patch)
        {

        }

        public void HandleModDisable()
        {

        }
        public void HandleModEnable()
        {

        }
    }
}
