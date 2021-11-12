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
            BuildingGridWrapper buildingGridWrapper = JsonUtility.FromJson<BuildingGridWrapper>(jsonString);
            List<SerializableBuildingGrid> serializableBuildingGrids = buildingGridWrapper.serializableBuildingGrids;

            Debug.Log(serializableBuildingGrids[0]);
            foreach (SerializableGridObject gridObject in serializableBuildingGrids[0].gridObjectWrapper.serializableGridObjects)
            {
                Debug.Log(gridObject.position);
            }
        }

        public static void Save()
        {
            if (buildingGrids.Count <= 0) return;
            List<SerializableBuildingGrid> serializableBuildingGrids = new List<SerializableBuildingGrid>();
            foreach(BuildingGrid buildingGrid in buildingGrids)
            {
                serializableBuildingGrids.Add(buildingGrid.GetSeriarizableBuildingGrid());
            }

            BuildingGridWrapper buildingGridWrapper = new BuildingGridWrapper(serializableBuildingGrids);
            string json = JsonUtility.ToJson(buildingGridWrapper, true);

            File.WriteAllText(savePath, json.ToString());
            Load();
        }

        public static string DebugLoad()
        {
            string jsonString = File.ReadAllText(savePath);
            return DebugLoad();
        }
    }


}

