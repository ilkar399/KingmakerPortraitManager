using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using static KingmakerPortraitManager.Helpers;
using static KingmakerPortraitManager.Main;

namespace KingmakerPortraitManager.Tests
{
    class Tests
    {
        public bool TestLoadAllCustomPortraits()
        {
            List<PortraitData> portraitDatas = new List<PortraitData>();
            portraitDatas = Helpers.LoadAllCustomPortraits();
            Main.Mod.Log($"LoadAllCustomPortraits count: {portraitDatas.Count}");
            if (portraitDatas.Count > 0) { return true; }
            else return false;
           
        }
    }
}
