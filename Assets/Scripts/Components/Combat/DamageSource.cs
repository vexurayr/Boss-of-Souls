using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : BaseValues
{
    [SerializeField] private float secondaryValueStart;
    private float currentSecondaryValue;

    protected override void Start()
    {
        base.Start();

        currentSecondaryValue = secondaryValueStart;

        if (isAffectedByDifficulty)
        {
            float difficulty = DifficultyManager.instance.GetCurrentDifficulty();
            currentSecondaryValue = currentSecondaryValue * difficulty;
        }
    }

    public float GetCurrentSecondaryValue()
    {
        return currentSecondaryValue;
    }
}