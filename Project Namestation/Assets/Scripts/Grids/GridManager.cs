using UnityEngine;
using System.Collections;
using Mirror;
using Namestation.Saving;
using Namestation.Player;

namespace Namestation.Grids
{
    public class GridManager : NetworkBehaviour
    {
        private GameObject buildingGridPrefab;
        private GameObject tilePrefab;


        public static GridManager instance;
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than 1 instance of GridManager found!");
                return;
            }
            instance = this;
        }

        private void Start()
        {
            buildingGridPrefab = ResourceManager.GetGridPrefab("BuildingGrid");
            tilePrefab = ResourceManager.GetGridPrefab("Tile");
        }

        public BuildingGrid CreateBuildingGridServer(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            GameObject newGridGameObject = Instantiate(buildingGridPrefab, position, rotation);
            Rigidbody2D rigidbody2D = newGridGameObject.GetComponent<Rigidbody2D>();
            rigidbody2D.velocity = velocity;

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
            tile.GetConnectedTiles();
        }

        public void CreateTileObjectServer(GameObject prefab, Tile tile, string jsonOverride = null)
        {
            GameObject newTileGameObject = Instantiate(prefab, Vector3.zero, tile.transform.rotation);
            NetworkServer.Spawn(newTileGameObject);
            TileObject newTileObject = newTileGameObject.GetComponent<TileObject>();

            //Check if syncvar works for tileobjects!
            if (jsonOverride != null)
            {
                JsonUtility.FromJsonOverwrite(jsonOverride, newTileObject);
            }

            newTileObject.currentParent = tile.transform;
            newTileObject.tileName = prefab.name;
            newTileObject.TryAssignValues();
            SyncTileObject(newTileObject);
            AddBuildingFeedbackClient(tile.transform.parent, newTileObject, tile.transform.localPosition);
        }

        [ClientRpc]
        private void SyncTileObject(TileObject tileObject)
        {
            tileObject.TryAssignValues();
        }

        [ClientRpc]
        private void AddBuildingFeedbackClient(Transform buildingGridTransform, TileObject tileObject, Vector3 localPosition)
        {
            PlayBuildSoundClient(buildingGridTransform, localPosition);
            StartCoroutine(IE_PlayBuildAnimationClient(tileObject));
        }

        private void PlayBuildSoundClient(Transform parent, Vector3 localPosition)
        {
            SoundManager localPlayerSoundManager = PlayerManager.localPlayerManager.soundManager;
            localPlayerSoundManager.PlayBuildingSound(parent, localPosition);
        }

        IEnumerator IE_PlayBuildAnimationClient(TileObject tileObject)
        {
            SpriteRenderer tileObjectRenderer = tileObject.GetComponent<SpriteRenderer>();
            float timePassed = 0f;
            float duration = 0.2f;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                float lerpAmount = (timePassed / duration) * 1.1f;
                tileObjectRenderer.size = new Vector2(lerpAmount, lerpAmount);
                yield return null;
            }

            timePassed = 0f;
            float downscaleDuration = 0.1f;
            while (timePassed < downscaleDuration)
            {
                timePassed += Time.deltaTime;
                float lerpAmount = 1.1f - (timePassed / downscaleDuration) * 0.1f;
                tileObjectRenderer.size = new Vector2(lerpAmount, lerpAmount);
                yield return null;
            }
            tileObjectRenderer.size = new Vector2(1f, 1f);

            yield return null;
        }
    }
}

