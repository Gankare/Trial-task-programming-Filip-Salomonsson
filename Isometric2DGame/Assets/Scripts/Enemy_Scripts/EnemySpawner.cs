using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using NavMeshPlus.Components;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("References")]
    public Tilemap targetTilemap;
    public GameObject[] spawnPrefabs;
    public Transform playerTransform;
    public Camera mainCamera;
    public NavMeshSurface navSurface;
    public Transform objectParent;

    [Header("Spawn Settings")]
    [Range(0f, 1f)] public float spawnChance = 0.05f;
    public float positionJitter = 0.2f;
    public float minDistanceFromPlayer = 5f;
    public float spawnBufferOutsideView = 2f;
    public bool clearOldSpawns = true;
    public float navmeshCheckRadius = 1f;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        //SpawnObjectsOutsideCamera();
    }

    public void SpawnObjectsOutsideCamera()
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

        Vector2 camMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector2 camMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        Rect cameraRect = new Rect(camMin, camMax - camMin);
        cameraRect.xMin -= spawnBufferOutsideView;
        cameraRect.xMax += spawnBufferOutsideView;
        cameraRect.yMin -= spawnBufferOutsideView;
        cameraRect.yMax += spawnBufferOutsideView;

        int spawnedCount = 0;

        foreach (var cellPos in validCells)
        {
            Vector3 worldPos = targetTilemap.CellToWorld(cellPos);

            if (cameraRect.Contains(worldPos))
                continue;

            if (Vector3.Distance(worldPos, playerPos) < minDistanceFromPlayer)
                continue;

            if (Random.value > spawnChance)
                continue;

            Vector3 offset = new(
                Random.Range(-positionJitter, positionJitter),
                Random.Range(-positionJitter, positionJitter),
                0
            );

            Vector3 spawnPos = worldPos + offset;

            if (!NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, navmeshCheckRadius, NavMesh.AllAreas))
                continue;

            GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            GameObject instance = Instantiate(prefab, hit.position, Quaternion.identity);
            instance.transform.SetParent(objectParent ? objectParent : transform);
            spawnedCount++;
        }

        if (navSurface != null)
            navSurface.BuildNavMesh();
    }

    public void ClearSpawnedObjects()
    {
        var parent = objectParent ? objectParent : transform;
        var toRemove = new List<GameObject>();

        foreach (var obj in toRemove)
            Destroy(obj);
    }
}
