using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [SerializeField] Terrain terrain;

    [Header("Interactions")]
    [SerializeField] List<GameObject> interactionList;
    [SerializeField] GameObject interactionContainer;
    [SerializeField] float interactionFrequency = 1f;
    [SerializeField] int interactionLimit = 100;

    [Header("Enemies")]
    [SerializeField] List<GameObject> enemyList;
    [SerializeField] GameObject enemyContainer;
    [SerializeField] float enemyFrequency = 1f;
    [SerializeField] int enemyLimit = 1000;


    void Start()
    {
        StartCoroutine(SpawnInteractions());
        StartCoroutine(SpawnEnemies());
    }

    // INTERACTIONS
    IEnumerator SpawnInteractions()
    {
        float delay = interactionFrequency;

        while (true)
        {
            int count = interactionContainer.transform.childCount;

            if (count < interactionLimit)
            {
                SpawnObject(interactionList, interactionContainer, true, false);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    // ENEMIES
    IEnumerator SpawnEnemies()
    {
        float delay = enemyFrequency;

        while (true)
        {
            int count = enemyContainer.transform.childCount;

            if (count < enemyLimit)
            {
                SpawnObject(enemyList, enemyContainer, false, true);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    // SPAWN RANDOM OBJECT
    void SpawnObject(
        List<GameObject> list,
        GameObject parent,
        bool alignToTerrain,
        bool requireNavMesh
    )
    {
        if (list.Count == 0) return;

        GameObject prefab = list[Random.Range(0, list.Count)];
        TerrainData td = terrain.terrainData;

        const int maxAttempts = 30; // AVOID LOOP

        for (int i = 0; i < maxAttempts; i++)
        {
            // random terrain position
            float x = Random.Range(0f, td.size.x);
            float z = Random.Range(0f, td.size.z);
            float height = td.GetInterpolatedHeight(x / td.size.x, z / td.size.z);

            Vector3 terrainPos = new Vector3(x, height, z) + terrain.transform.position;
            Vector3 spawnPos = terrainPos;

            Quaternion rot = Quaternion.identity;

            // NavMesh check
            if (requireNavMesh)
            {
                if (!NavMesh.SamplePosition(terrainPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    continue; // try another point

                spawnPos = hit.position;
            }


            // terrain alignment
            if (alignToTerrain)
            {
                float normX = x / td.size.x;
                float normZ = z / td.size.z;
                Vector3 normal = td.GetInterpolatedNormal(normX, normZ);
                rot = Quaternion.FromToRotation(Vector3.up, normal);
            }

            Instantiate(prefab, spawnPos, rot, parent.transform);
            return;
        }

        Debug.LogWarning("Failed to find valid spawn position.");
    }
}