using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components; 
using System.Collections.Generic;

public class TilemapObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public Tilemap targetTilemap;
    public GameObject[] spawnPrefabs;
    public Transform playerTransform;
    public NavMeshSurface navSurface;

    [Header("Spawn Settings")]
    [Range(0f, 1f)] public float spawnChance = 0.1f;
    public float positionJitter = 0.2f;
    public bool clearOldSpawns = true;
    public float minDistanceFromPlayer = 2.5f;
    public Transform objectParent;

    private readonly string spawnedTag = "SpawnedObject";

    void Start()
    {
        SpawnObjects();
    }

    public void SpawnObjects()
    {
        if (targetTilemap == null || spawnPrefabs == null || spawnPrefabs.Length == 0)
            return;

        if (clearOldSpawns)
            ClearSpawnedObjects();

        Vector3 playerPos = playerTransform ? playerTransform.position : Vector3.zero;
        BoundsInt bounds = targetTilemap.cellBounds;
        List<Vector3Int> validCells = new();

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (targetTilemap.HasTile(pos))
                validCells.Add(pos);
        }

        int spawnedCount = 0;
        foreach (var cellPos in validCells)
        {
            Vector3 worldPos = targetTilemap.CellToWorld(cellPos);
            if (Vector3.Distance(worldPos, playerPos) < minDistanceFromPlayer)
                continue;

            if (Random.value <= spawnChance)
            {
                Vector3 offset = new(
                    Random.Range(-positionJitter, positionJitter),
                    Random.Range(-positionJitter, positionJitter),
                    0
                );

                GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
                GameObject instance = Instantiate(prefab, worldPos + offset, Quaternion.identity);
                instance.transform.SetParent(objectParent ? objectParent : transform);
                instance.tag = spawnedTag;
                spawnedCount++;
            }
        }

        Debug.Log($"Spawned {spawnedCount} objects.");

        if (navSurface != null)
        {
            navSurface.BuildNavMesh();
        }
    }

    public void ClearSpawnedObjects()
    {
        var parent = objectParent ? objectParent : transform;
        var toRemove = new List<GameObject>();

        foreach (Transform child in parent)
        {
            if (child.CompareTag(spawnedTag))
                toRemove.Add(child.gameObject);
        }

        foreach (var obj in toRemove)
            Destroy(obj);
    }
}
