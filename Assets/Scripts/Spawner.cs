using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Terrain terrain;

    [Header("Interactions")]
    [SerializeField] List<GameObject> interactionList;
    [SerializeField] GameObject interactionContainer;
    [SerializeField] float interactionStartFrequency = 5f;
    [SerializeField] float interactionIncrementFrequency = 1f;
    [SerializeField] int interactionLimit = 100;

    [Header("Enemies")]
    [SerializeField] List<GameObject> enemyList;
    [SerializeField] GameObject enemyContainer;
    [SerializeField] float enemyStartFrequency = 10f;
    [SerializeField] float enemyIncrementFrequency = 1f;
    [SerializeField] int enemyLimit = 100;


    void Start()
    {
        StartCoroutine(SpawnInteractions());
        StartCoroutine(SpawnEnemies());
    }

    // INTERACTIONS
    IEnumerator SpawnInteractions()
    {
        float delay = interactionStartFrequency;

        while (true)
        {
            int count = interactionContainer.transform.childCount;

            if (count < interactionLimit)
            {
                SpawnObject(interactionList, interactionContainer, true);
            }

            yield return new WaitForSeconds(delay);

            delay = Mathf.Max(0.1f, delay - interactionIncrementFrequency);
        }
    }

    // ENEMIES
    IEnumerator SpawnEnemies()
    {
        float delay = enemyStartFrequency;

        while (true)
        {
            int count = enemyContainer.transform.childCount;

            if (count < enemyLimit)
            {
                SpawnObject(enemyList, enemyContainer, false);
            }

            yield return new WaitForSeconds(delay);

            delay = Mathf.Max(0.1f, delay - enemyIncrementFrequency);
        }
    }

    // SPAWN OBJECT
    void SpawnObject(List<GameObject> list, GameObject parent, bool alignToTerrain)
    {
        if (list.Count == 0) return;

        GameObject prefab = list[Random.Range(0, list.Count)];

        TerrainData td = terrain.terrainData;

        // position
        float x = Random.Range(0f, td.size.x);
        float z = Random.Range(0f, td.size.z);
        float height = td.GetInterpolatedHeight(x / td.size.x, z / td.size.z);
        Vector3 pos = new Vector3(x, height, z) + terrain.transform.position;

        Quaternion rot = Quaternion.identity;

        // rotation
        if (alignToTerrain)
        {
            float normX = x / td.size.x;
            float normZ = z / td.size.z;
            Vector3 normal = td.GetInterpolatedNormal(normX, normZ);
            rot = Quaternion.FromToRotation(Vector3.up, normal);
        }

        // insta
        Instantiate(prefab, pos, rot, parent.transform);
    }
}