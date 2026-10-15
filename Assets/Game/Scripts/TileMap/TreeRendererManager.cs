using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeRendererManager : MonoBehaviour
{
    [SerializeField] private Tilemap treeTilemap;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private Sprite[] treeSprites;
    [SerializeField] private Transform treeContainer;
    [SerializeField] private int baseSortingOrder = 0;
    [SerializeField] private int sortingOrderPerY = 10;

    private void Start()
    {
        SyncTrees();
    }

    public void SyncTrees()
    {
        // 清除现有的树木
        foreach (Transform child in treeContainer)
        {
            Destroy(child.gameObject);
        }

        // 获取Tilemap中所有已放置的Tile
        BoundsInt bounds = treeTilemap.cellBounds;
        for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = treeTilemap.GetTile(position);
                if (tile != null)
                {
                    CreateTreeAt(position);
                }
            }
        }
    }

    private void CreateTreeAt(Vector3Int position)
    {
        UnityEngine.Vector3 worldPosition = treeTilemap.GetCellCenterWorld(position);
        GameObject tree = Instantiate(treePrefab, worldPosition, UnityEngine.Quaternion.identity, treeContainer);
        
        if(treeSprites != null && treeSprites.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, treeSprites.Length);
            tree.GetComponent<SpriteRenderer>().sprite = treeSprites[randomIndex];
        }

        SpriteRenderer spriteRenderer = tree.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = baseSortingOrder - (position.y * sortingOrderPerY);
        }
    }

    // 在Tilemap发生变化时调用此方法
    public void OnTilemapChanged()
    {
        SyncTrees();
    }
} 