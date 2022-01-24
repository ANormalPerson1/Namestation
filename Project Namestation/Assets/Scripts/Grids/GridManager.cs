using UnityEngine;
using System.Collections;
using Mirror;
using Namestation.Saving;
using Namestation.Player;
using Namestation.Grids.Utilities;

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
            return newGrid;
        }

        public Tile CreateTileServer(BuildingGrid parentBuildingGrid, Vector2 localPosition)
        {
            GameObject newTileGameObject = Instantiate(tilePrefab, Vector3.zero, parentBuildingGrid.transform.rotation);
            NetworkServer.Spawn(newTileGameObject);
            Tile newTile = newTileGameObject.GetComponent<Tile>();

            newTile.currentParent = parentBuildingGrid.transform;
            newTile.position = new Vector2Int(Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.y));

            newTile.TryAssignValues();
            return newTile;
        }

        public void CreateTileObjectServer(GameObject prefab, Tile tile, string jsonOverride = null)
        {
            GameObject newTileGameObject = Instantiate(prefab, Vector3.zero, tile.transform.rotation);
            NetworkServer.Spawn(newTileGameObject);
            TileObject newTileObject = newTileGameObject.GetComponent<TileObject>();

            AttemptToLoadTileObjectFromFile(newTileObject, jsonOverride);
            SyncInitialAttributes(newTileObject, prefab.name, tile);
            AttemptToTileSprite(newTileGameObject, jsonOverride);
            PlayBuildingEffectClient(tile.transform.parent, newTileObject, tile.transform.localPosition);
        }

        private void AttemptToLoadTileObjectFromFile(TileObject newTileObject, string jsonOverride)
        {
            if (jsonOverride != null)
            {
                JsonUtility.FromJsonOverwrite(jsonOverride, newTileObject);
            }
        }

        private void SyncInitialAttributes(TileObject tileObject, string name, Tile tile)
        {
            tileObject.currentParent = tile.transform;
            tileObject.tileName = name;
            tileObject.zRotation = 0f;

            Sprite sprite = tileObject.GetComponent<SpriteRenderer>().sprite;
            string spriteName = sprite.name;
            tileObject.SetSpriteServer(spriteName);

            //Called to further sync transform parent, ect.
            tileObject.TryAssignValues();
        }

        private void AttemptToTileSprite(GameObject newTileGameObject, string jsonOverride)
        {
            bool isSpriteArleadyStored = jsonOverride != null;
            bool shouldTileSprite = !isSpriteArleadyStored && newTileGameObject.GetComponent<TiledSpriteModule>();
            if (shouldTileSprite)
            {
                TileSprite(newTileGameObject);
            }

        }

        private void TileSprite(GameObject newTileGameObject)
        {
            TiledSpriteModule tiledSpriteModule = newTileGameObject.GetComponent<TiledSpriteModule>();
            tiledSpriteModule.TileSpriteServer(true);
        }

        [ClientRpc]
        private void PlayBuildingEffectClient(Transform buildingGridTransform, TileObject tileObject, Vector3 localPosition)
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

