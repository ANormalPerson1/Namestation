using UnityEngine;
using Mirror;
using Namestation.Saving;
using Namestation.Grids;

namespace Namestation.Grids
{
    //Server only class!
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager instance;
        private void Awake()
        {
            if(instance != null)
            {
                Debug.LogError("More than 1 instance of BuildingManager found!");
                return;
            }
            instance = this;
        }

        public GameObject buildingGridPrefab;
        public GameObject tilePrefab;

        public BuildingGrid CreateBuildingGridServer(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            GameObject newGridGameObject = Instantiate(buildingGridPrefab, position, rotation);
            Rigidbody rigidbody = newGridGameObject.GetComponent<Rigidbody>();
            rigidbody.velocity = velocity;

            NetworkServer.Spawn(newGridGameObject.gameObject);
            BuildingGrid newGrid = newGridGameObject.GetComponent<BuildingGrid>();
            SaveManager.buildingGrids.Add(newGrid);

            newGrid.TryAssignValues();
            SyncBuildingGrid(newGrid);
            return newGrid;
        }

        [ClientRpc]
        private void SyncBuildingGrid(BuildingGrid buildingGrid)
        {
            buildingGrid.TryAssignValues();
        }

        public Tile CreateTileServer(BuildingGrid parentBuildingGrid, Vector2 localPosition)
        {
            GameObject newTileGameObject = Instantiate(tilePrefab, Vector3.zero, parentBuildingGrid.transform.rotation);
            NetworkServer.Spawn(newTileGameObject);
            Tile newTile = newTileGameObject.GetComponent<Tile>();
            parentBuildingGrid.tiles.Add(newTile);

            newTile.currentParent = parentBuildingGrid.transform;
            newTile.position = new Vector2Int(Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.y));

            newTile.TryAssignValues();
            SyncTile(newTile);
            return newTile;
        }

        [ClientRpc]
        private void SyncTile(Tile tile)
        {
            tile.TryAssignValues();
        }

        public void CreateTileObjectServer(GameObject prefab, Tile tile, string jsonOverride = null)
        {
            GameObject newTileGameObject = Instantiate(prefab, Vector3.zero, tile.transform.rotation);
            NetworkServer.Spawn(newTileGameObject);
            TileObject newTileObject = newTileGameObject.GetComponent<TileObject>();
            tile.tileObjects.Add(newTileObject);
            //Check if syncvar works for tileobjects!
            if(jsonOverride != null)
            {
                JsonUtility.FromJsonOverwrite(jsonOverride, newTileObject);
            } 

            newTileObject.currentParent = tile.transform;

            newTileObject.TryAssignValues();
            SyncTileObject(newTileObject);
        }

        [ClientRpc]
        private void SyncTileObject(TileObject tileObject)
        {
            tileObject.TryAssignValues();
        }
    }
}

