using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager instance { get; private set; }

    [SerializeField] private PlayerController currentPlayerController;
    [SerializeField] private GameObject playerPrefab;

    private bool isTimerRunning;
    private float timeAlive;

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
    }

    private void Start()
    {
        MenuManager.instance.SpawnThingsOnSceneLoad();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timeAlive += Time.deltaTime;
        }
    }

    #endregion MonoBehaviours

    #region GetSet
    public PlayerController GetCurrentPlayerController()
    {
        return currentPlayerController;
    }

    public void SetCurrentPlayerController(PlayerController newController)
    {
        currentPlayerController = newController;
    }

    public void SetIsTimerRunningState(bool newState)
    {
        isTimerRunning = newState;
    }

    #endregion GetSet

    public void ResetTimer()
    {
        timeAlive = 0f;
    }

    public int GetMinutes()
    {
        return Mathf.FloorToInt(timeAlive / 60);
    }

    public int GetSeconds()
    {
        float decim = timeAlive % 1;

        return Mathf.FloorToInt(decim * 60);
    }
}