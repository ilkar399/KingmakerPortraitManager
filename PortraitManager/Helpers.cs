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


namespace KingmakerPortraitManager
{
    //Tag Data class. Note that hash is always calculated only based on FullLengthPortrait texture
    [Serializable]
    public class TagData
    {
        public string Hash;
        public string CustomId;
        public List<String> tags;

        public TagData(string Hash,string CustomId, List<string> tags)
        {
            this.Hash = Hash;
            this.CustomId = CustomId;
            this.tags = tags;
        }


        //Serializer and deserializer.
        public void SaveData()
        {
            var JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            DefaultJsonSettings.Initialize();
            try
            {
                if (!Directory.Exists(ModPath + @"/tags/"))
                {
                    Directory.CreateDirectory(ModPath + @"/tags/");
                }
                JsonSerializer serializer = JsonSerializer.Create(JsonSettings);
                using (StreamWriter sw = new StreamWriter(ModPath + $"/tags/{this.CustomId}.json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {   
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception e)
            {
                Main.Mod.Log($"Error processing {this.CustomId}");
                Main.Mod.Log(e.StackTrace);
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
        public static void SaveTagsData(Dictionary<string,TagData> tagsData)
        {
            foreach (TagData tagData in tagsData.Values)
            {
                tagData.SaveData();
            }
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

    class Helpers
    {
		//TODO: Tag management

        //Modified LoadAllCustomPortraits from the game. Added skipping default portraits
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
				//TOOD: Tags
			}
			return list;
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

    }

}
