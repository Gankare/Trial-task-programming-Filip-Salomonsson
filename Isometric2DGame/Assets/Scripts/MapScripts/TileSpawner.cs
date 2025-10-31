using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class IsoTileSpawner : MonoBehaviour
{
    [Header("Tilemap Setup")]
    public Tilemap tilemap;
    public TileBase[] randomTiles;

    [Header("Spawn Area")]
    public int width = 10;
    public int height = 10;
    public Vector3Int startPosition = Vector3Int.zero;

    [Header("Options")]
    public bool clearBeforeSpawn = true;
    public bool randomizeOnPlay = true;
    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart)
            GenerateTileSquare();
    }

    public void GenerateTileSquare()
    {
        if (!tilemap)
        {
            return;
        }

        if (clearBeforeSpawn)
            tilemap.ClearAllTiles();

        if (randomTiles == null || randomTiles.Length == 0)
        {
            Debug.LogWarning("No tiles assigned in array");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePos = startPosition + new Vector3Int(x, y, 0);
                TileBase tile = randomTiles[Random.Range(0, randomTiles.Length)];
                tilemap.SetTile(tilePos, tile);
            }
        }

        Debug.Log($"{width * height} at {startPosition}");
    }
}
