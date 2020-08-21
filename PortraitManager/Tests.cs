using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using static KingmakerPortraitManager.Helpers;
using static KingmakerPortraitManager.Main;
using static KingmakerPortraitManager.Utility.SettingsWrapper;

namespace KingmakerPortraitManager
{
    class Tests
    {
        public static Dictionary<string, TagData> testData1 = new Dictionary<string, TagData>
        {
            { "testff7999997", new TagData("001","testff7999997",new List<string>{"elf","male","favorite" })},
            { "testff7999998", new TagData("002","testff7999998",new List<string>{"orc","female","favorite" })},
            { "testff7999999", new TagData("003","testff7999999",new List<string>{"elf","male",})},
        };


        public static bool TestLoadAllCustomPortraits()
        {
            List<PortraitData> portraitDatas = new List<PortraitData>();
            portraitDatas = Helpers.LoadAllCustomPortraits(false);
            Main.Mod.Log($"LoadAllCustomPortraits count: {portraitDatas.Count}");
            bool result = (portraitDatas.Count > 0);
            portraitDatas = null;
            if (result) {
                Main.Mod.Log("TestLoadAllCustomPortraits success");
                return true;
            }
            else return false;
           
        }

        public static bool TestTagIO()
        {
            //TODO: remove files after completion
            foreach(TagData value in testData1.Values)
            {
                value.SaveData();
            }
            var testDict = new Dictionary<string, TagData>();
            testDict = Tags.LoadTagsData();
            if (testDict != null)
            {
                Main.Mod.Log("TestTagIO success");
                return true;
            }
            else return false;
        }

        public static bool AllTests()
        {
            //TODO
            bool result = true;
            result = result && TestLoadAllCustomPortraits();
            result = result && TestTagIO();
            return result;
        }
    }
}
