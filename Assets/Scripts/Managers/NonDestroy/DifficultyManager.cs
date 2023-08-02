using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance { get; private set; }

    [Range(0f, 100f)] [SerializeField] private int startingStage;
    [Range(0f, 5f)] [SerializeField] private float difficultyMultiplierPerStage;
    [SerializeField] private float secondsUntilStageChange;
    [SerializeField] private bool isDebugging;

    private int currentStage;
    private float currentDifficulty = 1;
    private float timeUntilNextStageChange;
    private bool isAffectedByTime = true;

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

        currentStage = startingStage;
        timeUntilNextStageChange = secondsUntilStageChange;
    }

    private void Update()
    {
        IncreaseDifficultyWithTime();

        if (isDebugging)
        {
            Debug.Log("Diff: " + currentDifficulty);
        }
    }

    public void IncreaseDifficultyWithTime()
    {
        // Stop increasing difficulty if disabled
        if (!isAffectedByTime)
        {
            return;
        }

        // Countdown the difficulty multiplier increase timer
        timeUntilNextStageChange -= Time.deltaTime;
        Mathf.Clamp(timeUntilNextStageChange, 0, timeUntilNextStageChange);

        // Increase difficulty multiplier when timer reaches 0
        if (timeUntilNextStageChange <= 0)
        {
            IncreaseCurrentDifficulty();
        }
    }

    public float GetCurrentDifficulty()
    {
        return currentDifficulty;
    }

    // Will be called when the player enters a new zone
    public void ReduceCurrentDifficulty(float decBy)
    {
        currentDifficulty -= decBy;

        if (currentDifficulty < 1)
        {
            currentDifficulty = 1;
        }
    }

    public void IncreaseCurrentDifficulty()
    {
        // Reset timer
        timeUntilNextStageChange = secondsUntilStageChange;

        // Increment difficulty stage
        currentStage++;

        // Adjust difficulty based on new difficulty stage
        currentDifficulty = 1 + (currentStage * difficultyMultiplierPerStage);
    }

    public void ToggleIsAffectedByTime()
    {
        isAffectedByTime = !isAffectedByTime;
    }
}