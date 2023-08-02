using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Variables
    public enum SpawnerType
    {
        None,
        Player
    }

    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private List<GameObject> objectsToSpawn;
    [SerializeField] private float spawnRadius;

    #endregion Variables

    #region MonoBehaviours
    private void Start()
    {
        SpawnerManager.instance.AddSelfToSpawnerList(gameObject);
    }

    #endregion MonoBehaviours

    #region GetSet
    public SpawnerType GetSpawnerType()
    {
        return spawnerType;
    }

    #endregion GetSet

    #region SpawnerFunctions
    public void SpawnObject(int index)
    {
        if (objectsToSpawn.Count == 0 || index > objectsToSpawn.Count)
        {
            return;
        }

        float randSpawnX = Random.Range(-1f, 1f) * spawnRadius;
        float randSpawnZ = Random.Range(-1f, 1f) * spawnRadius;
        float randAngle = Random.Range(0f, 360f);
        Vector3 randPosition = new Vector3(randSpawnX, 0, randSpawnZ);

        GameObject newObj = Instantiate(objectsToSpawn[index], gameObject.transform.position +
            randPosition, gameObject.transform.rotation);

        newObj.transform.rotation = Quaternion.Euler(Vector3.up * randAngle);
    }

    public void SpawnRandomObject()
    {
        if (objectsToSpawn.Count == 0)
        {
            return;
        }

        int randIndex = Random.Range(0, objectsToSpawn.Count);
        float randSpawnX = Random.Range(-1f, 1f) * spawnRadius;
        float randSpawnZ = Random.Range(-1f, 1f) * spawnRadius;
        float randAngle = Random.Range(0f, 360f);
        Vector3 randPosition = new Vector3(randSpawnX, 0, randSpawnZ);

        GameObject newObj = Instantiate(objectsToSpawn[randIndex], gameObject.transform.position +
            randPosition, gameObject.transform.rotation);

        newObj.transform.rotation = Quaternion.Euler(Vector3.up * randAngle);
    }

    #endregion SpawnerFunctions

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }

    #endregion Gizmos
}