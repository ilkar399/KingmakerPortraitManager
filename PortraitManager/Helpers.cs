using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Kingmaker.Blueprints;
using TinyJson;
using Kingmaker.EntitySystem.Persistence.JsonUtility;

namespace KingmakerPortraitManager
{
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


        public void SaveData(string basePath)
        {
            //TODO: Save data into file
            var JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = DefaultJsonSettings.CommonConverters.ToList<JsonConverter>(),
                ContractResolver = new OptInContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            try
            {
                var serializer = JsonSerializer.Create(JsonSettings);
                using (var file = new StreamWriter(basePath + $"/KingmakerPortraitManager/tags/{this.CustomId}.json"))
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception e)
            {
                Main.Mod.Log(e.StackTrace);
            }
        }

        public TagData LoadData(string filePath)
        {
            TagData result;
            var JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = DefaultJsonSettings.CommonConverters.ToList<JsonConverter>(),
                ContractResolver = new OptInContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            var serializer = JsonSerializer.Create(JsonSettings);
            using (var file = new StreamReader(filePath))
            using (JsonReader reader = new JsonTextReader(file))
            {
                result = serializer.Deserialize<TagData>(reader);
            }
            return result;
        }

    }

    class Tags
    {
        public Dictionary<string, TagData> LoadTagsData()
        {
            //TODO: Load tag data from json files. Returns dictionary of 
            //customids and tagdata
            var result = new Dictionary<string, TagData>();
            return result;
        }

        public Dictionary<string,TagData> HashDictionary(Dictionary<string,TagData> tagsData, Boolean SkipBase)
        {
            //TODO: add skipping base (blank) portraits
            //TODO: hash collisions and duplicate portraits
            var result = new Dictionary<string, TagData>();
            foreach (string tagID in tagsData.Keys)
            {
                var tHash = tagsData[tagID].Hash;
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

		public static List<PortraitData> LoadAllCustomPortraits()
		{
			string[] existingCustomPortraitIds = CustomPortraitsManager.Instance.GetExistingCustomPortraitIds();
			List<PortraitData> list = new List<PortraitData>();
			for (int i = 0; i < existingCustomPortraitIds.Length; i++)
			{
				PortraitData portraitData = new PortraitData(existingCustomPortraitIds[i]);
				portraitData.EnsureImages(false);
				portraitData.CheckIfDefaultPortraitData();
				list.Add(portraitData);
				//TOOD: Tags
			}
			return list;
		}

	}

}
