using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kingmaker.Blueprints;
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

        private static Dictionary<string, TagData> tagsData;
        private static Dictionary<string, TagData> allPortraitsData;


        public static bool TestLoadAllCustomPortraits()
        {
            tagsData = Tags.LoadTagsData(false);
            allPortraitsData = new Dictionary<string, TagData>();
            allPortraitsData = Helpers.LoadAllPortraitsTags(tagsData, false);
            var portraitIDs = allPortraitsData.Values.Select(type => type?.CustomId).ToArray();
            PortraitData portraitData;
            foreach (string portraitID in portraitIDs)
                portraitData = Helpers.LoadPortraitData(allPortraitsData[portraitID].CustomId);
            Main.Mod.Log($"LoadAllCustomPortraits count: {allPortraitsData.Count}");
            bool result = (allPortraitsData.Count > 0);
            allPortraitsData = null;
            if (result) {
                Main.Mod.Log("TestLoadAllCustomPortraits success");
                return true;
            }
            else return false;
           
        }

        public static bool TestTagIO()
        {
            foreach(TagData value in testData1.Values)
            {
                value.SaveData(false);
            }
            var testDict = new Dictionary<string, TagData>();
            testDict = Tags.LoadTagsData(false);
            if (testDict != null)
            {
                Main.Mod.Log("TestTagIO success");
                return true;
            }
            else return false;
        }

        public static bool TestAllTagsList()
        {
            var AllTagsFilter = Tags.AllTagsFilter(Tags.LoadTagsData(false));
            Main.Mod.Log($"TestAllTagsList: {string.Join(", ", AllTagsFilter.ToArray())}");
            return true;
        }

        public static bool TestAllTagListUI()
        {
            var AllTagsFilter = Tags.AllTagListUI(tagsData, allPortraitsData);
            Main.Mod.Log($"TestAllTagsListUI: {string.Join(", ", AllTagsFilter.ToArray())}");
            return true;
        }

        public static bool TestLoadAllPortraitsTags()
        {
            var result = Helpers.LoadAllPortraitsTags(tagsData, true);
            Main.Mod.Log($"TestLoadAllPortraitsTags result count: { result.Count()}");
            return true;
        }

        public static void TestCleanup()
        {
            foreach (string k in testData1.Keys)
            {
                File.Delete(ModPath + $"/tags/{k}.json");
                tagsData = null;
                allPortraitsData = null;
            }
        }


        public static bool AllTests()
        {
            //TODO
            //Import&Export tests?
            //ImportPortraitsTags
            //HashDuplicatesTagDictionary
            bool result = true;
            result = result && TestLoadAllCustomPortraits();
            result = result && TestTagIO();
            result = result && TestAllTagsList();
            result = result && TestAllTagListUI();
            result = result && TestLoadAllPortraitsTags();
            TestCleanup();
            return result;
        }
    }
}
