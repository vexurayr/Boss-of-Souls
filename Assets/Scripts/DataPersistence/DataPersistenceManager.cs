using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool isUsingEncryption;

    // Instance can be gotten publicly, but only changed privately
    public static DataPersistenceManager instance { get; private set; }

    private List<IDataPersistence> dataPersistenceObjects;

    private GameData gameData;

    private FileDataHandler dataHandler;

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
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, isUsingEncryption);

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load any saved data from the file using the data handler
        this.gameData = dataHandler.Load();

        // If there's no data to load, initialize a new game
        if (this.gameData == null)
        {
            Debug.Log("No data found. Initializing with default values.");
            NewGame();
        }

        // Give data to all scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        try
        {
            // Give data to other scripts so they can update it
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.SaveData(gameData);
            }

            // Save that data to a file using the data handler
            dataHandler.Save(gameData);
        }
        catch (Exception)
        {
            Debug.LogWarning("Failed to save. Likely do to changing code while the game was running.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    // Useful for scenarios where not all objects with persisting data are in the scene on start
    public void UpdateNuberOfDataPersistenceObjects()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    // Finds and returns all scripts that implement the IDataPersistence interface
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().
            OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}