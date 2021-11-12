using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Namestation.Grids;

namespace Namestation.SaveSystem
{
    public static class SaveManager
    {
        public static List<BuildingGrid> buildingGrids = new List<BuildingGrid>();
        private static string savePath = Application.dataPath + "saveFile.json";

        public static void Load()
        {
            string jsonString = File.ReadAllText(savePath);
            BuildingGrid[] buildingGridsArray = JsonHelper.FromJson<BuildingGrid>(jsonString);
            buildingGrids = buildingGridsArray.ToList();
            Debug.Log(buildingGrids[0]);
            foreach(GridObject gridObject in buildingGrids[0].gridObjects)
            {
                Debug.Log(gridObject.name);
            }
        }

        public static void Save()
        {
            if (buildingGrids.Count <= 0) return;

            StringBuilder jsonString = new StringBuilder("{\"Items\":[" + buildingGrids[0].ToJson());
            for(int i = 1; i < buildingGrids.Count; i++)
            {
                string buildingGridJson = buildingGrids[i].ToJson();
                jsonString.Append("," + buildingGridJson);
            }
            jsonString.Append("]}");
            //Issue, function writes pointer instead of object values!
            File.WriteAllText(savePath, jsonString.ToString());
            Load();
        }

        public static string DebugLoad()
        {
            string jsonString = File.ReadAllText(savePath);
            return DebugLoad();
        }
    }
}

