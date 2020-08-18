using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;


namespace PortraitManager
{
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
