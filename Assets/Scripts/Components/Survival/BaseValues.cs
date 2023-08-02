using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseValues : MonoBehaviour
{
    #region Variables
    [SerializeField] protected float startValue;
    [SerializeField] protected float maxValue;
    [SerializeField] protected float drainMult;
    [SerializeField] protected float drainDelay;
    [SerializeField] protected float regainMult;
    [SerializeField] protected float regainDelay;
    [SerializeField] protected bool isAffectedByDifficulty;
    [SerializeField] protected bool isDebugging;

    protected float currentValue;
    protected bool isDrainingValueOverTime;
    protected bool isRegainingValueOverTime;
    protected float currentDrainMult;
    protected float currentRegainMult;

    #endregion Variables

    #region MonoBehaviours
    protected virtual void Start()
    {
        currentValue = startValue;
        currentDrainMult = drainMult;
        currentRegainMult = regainMult;
        isDrainingValueOverTime = false;
        isRegainingValueOverTime = false;

        if (isAffectedByDifficulty)
        {
            float difficulty = DifficultyManager.instance.GetCurrentDifficulty();
            maxValue = maxValue * difficulty;
            currentValue = currentValue * difficulty;
        }
    }

    protected virtual void Update()
    {
        if (isDrainingValueOverTime)
        {
            DecCurrentValueOverTime();
        }

        if (isRegainingValueOverTime)
        {
            IncCurrentValueOverTime();
        }
    }

    #endregion MonoBehaviours

    #region GetSet
    public virtual float GetCurrentValue()
    {
        return currentValue;
    }

    public virtual void SetCurrentValue(float newValue)
    {
        currentValue = newValue;
    }

    public virtual float GetMaxValue()
    {
        return maxValue;
    }

    public virtual void SetMaxValue(float newValue)
    {
        maxValue = newValue;
    }

    public virtual void SetIsDrainingValueOverTime(bool newState)
    {
        isDrainingValueOverTime = newState;
    }

    public virtual void SetIsRegainingValueOverTime(bool newState)
    {
        isRegainingValueOverTime = newState;
    }

    public virtual float GetDrainMult()
    {
        return currentDrainMult;
    }

    public virtual void SetDrainMult(float newMult)
    {
        currentDrainMult = newMult;
    }

    public virtual float GetRegainMult()
    {
        return currentRegainMult;
    }

    public virtual void SetRegainMult(float newMult)
    {
        currentRegainMult = newMult;
    }

    #endregion GetSet

    #region DecreaseCurrentValue
    public virtual void DecCurrentValue(float decAmount)
    {
        currentValue = currentValue - decAmount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }

    public virtual void DecCurrentValueOverTime()
    {
        currentValue -= currentDrainMult * Time.deltaTime;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }

    #endregion DecreaseCurrentValue

    #region IncreaseCurrentValue
    public virtual void IncCurrentValue(float incAmount)
    {
        currentValue = currentValue + incAmount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }

    public virtual void IncCurrentValueOverTime()
    {
        currentValue += currentRegainMult * Time.deltaTime;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }

    #endregion IncreaseCurrentValue

    #region ChangeMultipliers
    public virtual void IncDrainMult(float incAmount)
    {
        currentDrainMult += incAmount;
    }

    public virtual void DecDrainMult(float decAmount)
    {
        currentDrainMult -= decAmount;
    }

    public virtual void ResetDrainMult()
    {
        currentDrainMult = drainMult;
    }

    public virtual void IncRegainMult(float incAmount)
    {
        currentRegainMult += incAmount;
    }

    public virtual void DecRegainMult(float decAmount)
    {
        currentRegainMult -= decAmount;
    }

    public virtual void ResetRegainMult()
    {
        currentRegainMult = regainMult;
    }

    #endregion ChangeMultipliers

    #region IEnumerators
    public virtual IEnumerator DrainValueDelay()
    {
        yield return new WaitForSeconds(drainDelay);

        isDrainingValueOverTime = true;
    }

    public virtual IEnumerator RegainValueDelay()
    {
        yield return new WaitForSeconds(regainDelay);

        isRegainingValueOverTime = true;
    }

    #endregion IEnumerators

    #region Helpers
    public bool IsCurrentValueAtMaxValue()
    {
        if (currentValue >= maxValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}