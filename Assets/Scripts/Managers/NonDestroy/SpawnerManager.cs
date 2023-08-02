using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    #region Variables
    public static SpawnerManager instance { get; private set; }

    private List<GameObject> playerSpawners;

    private int maxPlayersInScene = 4;
    private int playersAvailableToSpawn;
    private bool canSpawnPlayers;

    #endregion Variables

    #region MonoBehaviours
    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerSpawners = new List<GameObject>();
        playersAvailableToSpawn = maxPlayersInScene;
    }

    #endregion MonoBehaviours

    #region SpawnerFunctions
    // Randomly choosing a spawner and spawning 1-4 players
    public void SpawnPlayer()
    {
        // There aren't any of these spawners in the scene
        if (playerSpawners.Count == 0 || !canSpawnPlayers)
        {
            return;
        }

        AudioManager.instance.PlaySound2D("Wave Spawn");
        
        // Shuffles the list for random first spawner
        for (int i = 0; i < playerSpawners.Count; i++)
        {
            GameObject temp = playerSpawners[i];
            int randomIndex = Random.Range(i, playerSpawners.Count);
            playerSpawners[i] = playerSpawners[randomIndex];
            playerSpawners[randomIndex] = temp;
        }

        // Choose to spawn 1-4 players
        int randAmount = Random.Range(1, maxPlayersInScene + 1);
        
        playersAvailableToSpawn -= randAmount;

        for (int i = 0; i < randAmount; i++)
        {
            if (playerSpawners != null)
            {
                playerSpawners[0].GetComponent<Spawner>().SpawnRandomObject();
            }
        }

        canSpawnPlayers = false;
    }
    
    #endregion SpawnerFunctions

    #region HelperFunctions
    public void AddSelfToSpawnerList(GameObject obj)
    {
        playerSpawners.Add(obj);
    }

    public void ResetVariables()
    {
        playerSpawners.Clear();

        playersAvailableToSpawn = maxPlayersInScene;
    }

    public void IncrementPlayersLeft()
    {
        if (playersAvailableToSpawn < maxPlayersInScene)
        {
            playersAvailableToSpawn++;
        }
        
        CheckCanSpawnPlayers();
    }

    public void CheckCanSpawnPlayers()
    {
        if (playersAvailableToSpawn == maxPlayersInScene)
        {
            canSpawnPlayers = true;

            StartCoroutine("SpawnPlayerTimer");
        }
    }

    private IEnumerator SpawnPlayerTimer()
    {
        yield return new WaitForSeconds(5);
        SpawnPlayer();
    }

    #endregion HelperFunctions
}