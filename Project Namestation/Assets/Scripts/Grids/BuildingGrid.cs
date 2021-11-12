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

        public BuildingGrid(int netID)
        {
            this.netID = netID;
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
            gridObjectWrapper = new GridObjectWrapper(buildingGrid.gridObjects);
        }
    }

    [Serializable]
    public class BuildingGridWrapper
    {
        public List<SerializableBuildingGrid> serializableBuildingGrids;

        public BuildingGridWrapper (List<BuildingGrid> buildingGrids)
        {
            List<SerializableBuildingGrid> serializableBuildingGrids = new List<SerializableBuildingGrid>();
            foreach (BuildingGrid buildingGrid in buildingGrids)
            {
                serializableBuildingGrids.Add(buildingGrid.GetSeriarizableBuildingGrid());
            }
             
            this.serializableBuildingGrids = serializableBuildingGrids;
        }
    }
}