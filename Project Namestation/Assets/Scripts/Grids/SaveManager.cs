using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Namestation.Grids;
using Namestation.Interactables;
using Mirror;

namespace Namestation.Saving
{
    public static class SaveManager
    {
        public static List<BuildingGrid> buildingGrids = new List<BuildingGrid>();
        private static string savePath = Application.dataPath + "saveFile.json";

        public static void Load()
        {
            string jsonString = File.ReadAllText(savePath);
            LoadObjects(jsonString);
        }

        public static void LoadObjects(string jsonString)
        {
            BuildingGridWrapper buildingGridWrapper = JsonUtility.FromJson<BuildingGridWrapper>(jsonString);
            List<SerializableBuildingGrid> serializableBuildingGrids = buildingGridWrapper.serializableBuildingGrids;

            buildingGrids.Clear();

            foreach (SerializableBuildingGrid serializableBuildingGrid in serializableBuildingGrids)
            {
                BuildingGrid buildingGrid = SaveLoader.instance.LoadBuildingGrid(serializableBuildingGrid);
                buildingGrids.Add(buildingGrid);

                //Link/add componment to corresponding transform
               
            }

        }

       

        public static void Save()
        {
            SaveObjects();
        }

        public static void SaveObjects()
        {
            if (buildingGrids.Count <= 0) return;

            BuildingGridWrapper buildingGridWrapper = new BuildingGridWrapper(buildingGrids);
            string json = JsonUtility.ToJson(buildingGridWrapper, true);

            File.WriteAllText(savePath, json.ToString());
            Load();
        }
    }
}

