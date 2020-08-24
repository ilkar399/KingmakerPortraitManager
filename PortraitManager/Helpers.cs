using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Validation;
using UnityEngine;
using TinyJson;
using Kingmaker.EntitySystem.Persistence.JsonUtility;
using static KingmakerPortraitManager.Utility.SettingsWrapper;
using Kingmaker.UI._ConsoleUI.CombatLog;
using Kingmaker.EntitySystem;
using Kingmaker.Utility;
using Kingmaker.UI.EndlessGameOver;

namespace KingmakerPortraitManager
{
    //Tag Data class. Note that hash is always calculated only based on FullLengthPortrait texture
    [Serializable]
    public class TagData
    {
        public string Hash { get; set; }
        public string CustomId { get; set; }
        public List<String> tags;

        public TagData(string Hash,string CustomId, List<string> tags)
        {
            this.Hash = Hash;
            this.CustomId = CustomId;
            this.tags = tags;
        }


        //Serializer and deserializer.
        public void SaveData(bool isExport)
        {
            var JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            string savepath = "";
            if (isExport)
            {
                savepath = ModPath + @"/Export/tags/";
            }
            else
            {
                savepath = ModPath + @"/tags/";
            }
            DefaultJsonSettings.Initialize();
            try
            {
                if (!Directory.Exists(savepath))
                {
                    Directory.CreateDirectory(savepath);
                }
                JsonSerializer serializer = JsonSerializer.Create(JsonSettings);
                using (StreamWriter sw = new StreamWriter(savepath + $"{this.CustomId}.json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {   
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception e)
            {
                Main.Mod.Log($"Error processing {this.CustomId}");
#if (DEBUG)
                Main.Mod.Log(e.StackTrace);
#endif
                throw e;
            }
        }

        public static TagData LoadData(string filePath)
        {
            TagData result;
            var JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            var serializer = JsonSerializer.Create(JsonSettings);
            try
            {

                using (var file = new StreamReader(filePath))
                using (JsonReader reader = new JsonTextReader(file))
                {
                    result = serializer.Deserialize<TagData>(reader);
                }
                return result;
            }
            catch (Exception e)
            {
                Main.Mod.Log(e.StackTrace);
                return null;
            }
        }

    }

    class Tags
    {
        //Load all tags from tag directory
        public static Dictionary<string, TagData> LoadTagsData()
        {
            var result = new Dictionary<string, TagData>();
            try
            {
                if (!Directory.Exists(ModPath + @"/tags/"))
                {
                    Directory.CreateDirectory(ModPath + @"/tags/");
                }
                string[] tagFiles = Directory.GetFiles(ModPath + @"/tags/", "*.json");
                foreach (string tagFile in tagFiles)
                {
                    var tagData = TagData.LoadData(tagFile);
                    if (tagData != null)
                    {
                        result.Add(tagData.CustomId,tagData);
                    }
                }
            }
            catch (Exception e)
            {
                Main.Mod.Log(e.StackTrace);
                return null;
            }
            return result;
        }

        //Save tags all tags data
        //TOOD? add ignoring default portraits?
        public static void SaveTagsData(Dictionary<string,TagData> tagsData, bool isExport)
        {
            foreach (TagData tagData in tagsData.Values)
            {
                tagData.SaveData(isExport);
            }
        }

        //Get an initial filter tags dictionary based on all tag data
        public static Dictionary<string,bool> AllTagsFilter(Dictionary<string, TagData> tagsData)
        {
            Dictionary<string,bool> result = new Dictionary<string,bool>();
            result = tagsData.SelectMany(tagItem => tagItem.Value.tags).Distinct().ToDictionary(p => p, p => false);
            //TODO add recent
            return result;
        }

        //Converting from the {customId,tagData} dictionary to the {hash,tagData} one
        public static  Dictionary<string,TagData> HashDictionary(Dictionary<string,TagData> tagsData, Boolean SkipBase)
        {
            //TODO: hash collisions and duplicate portraits
            var result = new Dictionary<string, TagData>();
            string defaultHash = Helpers.GetPseudoHash(BlueprintRoot.Instance.CharGen.BasePortraitBig.texture).ToString();
            foreach (string tagID in tagsData.Keys)
            {
                var tHash = tagsData[tagID].Hash;
                if (tHash == defaultHash) { continue; }
                if (result.ContainsKey(tHash))
                {
                    result[tHash].tags.AddRange(tagsData[tagID].tags);
                }
                else
                {
                    result[tHash] = tagsData[tagID];
                }
            }
            return result;
        }
    }

    static class Helpers
    {
        //Filter portraits based on filter selections
        //If no tag selected, returns all
        public static string[] FilterPortraitIDs(string filterName,
            bool filterValue,
            Dictionary<string,TagData> allPortraitsData,
            Dictionary<string,bool> tagListAll)
        {
            string[] result = new string[] { };
            tagListAll[filterName] = filterValue;
            if (!tagListAll.ContainsValue(true))
            {
                result = allPortraitsData.Values.Select(type => type?.CustomId).ToArray();
                return result;
            }
            var filteredTags = tagListAll.Where(p => p.Value).Select(kvp => kvp.Key).ToList();
            result = allPortraitsData.Where(kvp => {
                bool tresult = false;
                foreach (string filteredTag in filteredTags)
                {
                    if (kvp.Value.tags.Contains(filteredTag)) return true;
                }
                return tresult;
            }).Select(kvp => kvp.Value.CustomId).ToArray();
            return result;
        }

        //Load all tags for all portraits in folder
        public static Dictionary<string,TagData> LoadAllPortraitsTags(Dictionary<string,TagData> customTags,Boolean skipDefault)
        {
            string[] existingCustomPortraitIds = CustomPortraitsManager.Instance.GetExistingCustomPortraitIds();
            Dictionary<string,TagData> result = new Dictionary<string, TagData>();
            for (int i = 0; i < existingCustomPortraitIds.Length; i++)
            {
                PortraitData portraitData = new PortraitData(existingCustomPortraitIds[i]);
                portraitData.EnsureImages(false);
                portraitData.CheckIfDefaultPortraitData();
                if (portraitData.IsDefault && skipDefault)
                {
                    continue;
                }
                List<string> tagList;
                if (customTags.ContainsKey(portraitData.CustomId))
                {
                    tagList = new List<string>(customTags[portraitData.CustomId].tags);
                }
                else
                {
                    tagList = new List<string>();
                }
                 TagData resultTag = new TagData(GetPseudoHash(portraitData.FullLengthPortrait.texture).ToString(),
                        portraitData.CustomId, tagList);
                result[resultTag.CustomId] = resultTag;
                portraitData = null;
            }
            return result;
        }

        //Load portrait data for 1 ID
        public static PortraitData LoadPortraitData(string customID)
        {
            PortraitData result = new PortraitData(customID);
            result.EnsureImages(false);
            return result;
        }


        //Modified LoadAllCustomPortraits from the game. Added skipping default portraits. Not in use now
		public static List<PortraitData> LoadAllCustomPortraits(Boolean skipDefault)
		{
			string[] existingCustomPortraitIds = CustomPortraitsManager.Instance.GetExistingCustomPortraitIds();
			List<PortraitData> list = new List<PortraitData>();
			for (int i = 0; i < existingCustomPortraitIds.Length; i++)
			{
				PortraitData portraitData = new PortraitData(existingCustomPortraitIds[i]);
				portraitData.EnsureImages(false);
                portraitData.CheckIfDefaultPortraitData();
                if (portraitData.IsDefault && skipDefault)
                {
                    continue;
                }
				list.Add(portraitData);
			}
			return list;
		}

        //Export portraits + tags from game portrait folder to ModPath/Export
        //TODO: Localized messages
        public static string ExportPortraits(Dictionary <string,TagData> currentPortraitTagsData)
        {
            string result = "";
            int TagErrorCount = 0;
            int PortraitErrorCount = 0;
            string portraitRootFolder = Path.Combine(Application.persistentDataPath,BlueprintRoot.Instance.CharGen.PortraitFolderName);
            string exportRootFolder = Path.Combine(ModPath,"Export");
            if (Directory.Exists(exportRootFolder))
                try 
                {
                    Directory.Delete(exportRootFolder, true);
                    Directory.CreateDirectory(exportRootFolder);
                    Directory.CreateDirectory(Path.Combine(exportRootFolder,"Portraits"));
                    Directory.CreateDirectory(Path.Combine(exportRootFolder,"tags"));
                }
                catch
                {
                    Main.Mod.Error("Error initializing export folder.");
                    return "Error initializing export folder.";
                }
            
            Dictionary<string, TagData> filteredTagData = currentPortraitTagsData.Where(
                           kvp => kvp.Value.tags.Count() > 0).ToDictionary(kvp => kvp.Key, kvp=> kvp.Value);
            foreach (TagData tag in filteredTagData.Values)
            {
                try
                {
                    tag.SaveData(true);
                }
                catch
                {
                    TagErrorCount++;
                }
            }
            foreach (string customId in currentPortraitTagsData.Keys)
            {
                try
                {
                    DirectoryCopy(Path.Combine(portraitRootFolder,customId),Path.Combine(exportRootFolder,"Portraits",customId),true);
                }
                catch (Exception e)
                {
#if (DEBUG)
                    Main.Mod.Log(e.StackTrace);
#endif
                    Main.Mod.Error("Error exporting " + customId);
                    PortraitErrorCount++;
                }
            }
            if ((TagErrorCount > 0) || (PortraitErrorCount > 0))
            {
                result = $"There were errors during portrait export. Tag processing errors: {TagErrorCount}. " +
                    $"Portrait processing errors: {PortraitErrorCount}";
            }
            else result = "Data exported sucessfully.";
            return result;
        }

        //Hashing function used in game. Used here for better compatability
        public static int GetPseudoHash(Texture2D texture)
        {
            int num = 100;
            int num2 = texture.width * texture.height;
            int num3 = num2 / num;
            Color32[] pixels = texture.GetPixels32();
            int num4 = -2128831035;
            for (int i = 0; i < num2 - 1; i += num3)
            {
                Color32 color = pixels[i];
                num4 = (num4 ^ (int)color.r) * 16777619;
                num4 = (num4 ^ (int)color.g) * 16777619;
                num4 = (num4 ^ (int)color.b) * 16777619;
            }
            num4 += num4 << 13;
            num4 ^= num4 >> 7;
            num4 += num4 << 3;
            num4 ^= num4 >> 17;
            return num4 + (num4 << 5);
        }

        //Helper function to remove string array element at RemoveAt
        public static string[] RemoveIndices(string [] IndicesArray, int RemoveAt)
        {
            string[] newIndicesArray = new string[IndicesArray.Length - 1];

            int i = 0;
            int j = 0;
            while (i < IndicesArray.Length)
            {
                if (i != RemoveAt)
                {
                    newIndicesArray[j] = IndicesArray[i];
                    j++;
                }

                i++;
            }

            return newIndicesArray;
        }

        //Copy directory (example from msdn) 
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }

}
