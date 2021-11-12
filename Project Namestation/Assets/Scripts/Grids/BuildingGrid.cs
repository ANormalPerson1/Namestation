using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Mirror;

namespace Namestation.Grids
{
    /// <summary>
    /// Grids used by the namestation building system (Examples: Station, asteroid, planet, ship)
    /// </summary>
    public class BuildingGrid : MonoBehaviour
    {
        public int netID;
        public List<GridObject> gridObjects;

        public void SetNetId(int newID)
        {
            netID = newID;
        }

        public SerializableBuildingGrid GetSeriarizableBuildingGrid ()
        {
            return new SerializableBuildingGrid(this);
        }
    }

    [Serializable]
    public class SerializableBuildingGrid
    {
        public int netID;
        public GridObjectWrapper gridObjectWrapper;

        public SerializableBuildingGrid (BuildingGrid buildingGrid)
        {
            netID = buildingGrid.netID;
            List<SerializableGridObject> serializableGridObjects = new List<SerializableGridObject>();
            foreach(GridObject gridObject in buildingGrid.gridObjects)
            {
                serializableGridObjects.Add(gridObject.GetSerializableGridObject());
            }

            gridObjectWrapper = new GridObjectWrapper(serializableGridObjects);
        }
    }

    [Serializable]
    public class BuildingGridWrapper
    {
        public List<SerializableBuildingGrid> serializableBuildingGrids;

        public BuildingGridWrapper (List<SerializableBuildingGrid> serializableBuildingGrids)
        {
            this.serializableBuildingGrids = serializableBuildingGrids;
        }
    }
}