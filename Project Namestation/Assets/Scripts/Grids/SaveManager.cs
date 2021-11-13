using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Namestation.Grids;

namespace Namestation.Saving
{
    public static class SaveManager
    {
        public static List<BuildingGrid> buildingGrids = new List<BuildingGrid>();
        private static string savePath = Application.dataPath + "saveFile.json";
        public static bool isSaving = false;

        public static void Load()
        {
            string jsonString = null;
            if(File.Exists(savePath))
            {
                jsonString = File.ReadAllText(savePath);
            }

            if (jsonString == "" || jsonString == null)
            {
                Debug.Log("Save file not found, starting new game.");
            }
            else
            {
                Debug.Log("Save file found, loading game....");
                LoadObjects(jsonString);
            }
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
            }

            Debug.Log("Game loaded!");
        }

        public static void Save()
        {
            isSaving = true;
            SaveObjects();
        }

        public static void SaveObjects()
        {
            if (buildingGrids.Count <= 0)
            {
                isSaving = false;
                return;
            }

            BuildingGridWrapper buildingGridWrapper = new BuildingGridWrapper(buildingGrids);
            string json = JsonUtility.ToJson(buildingGridWrapper, true);

            File.WriteAllText(savePath, json.ToString());
            isSaving = false;
        }
    }
}

