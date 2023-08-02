using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Health : BaseValues
{
    #region Variables
    [SerializeField] private Spawner.SpawnerType spawnedFrom;
    [SerializeField] private string playSoundFromDamage;
    [SerializeField] private string playSoundOnDeath;

    #endregion Variables

    #region MonoBehaviours
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isDebugging)
        {
            Debug.Log("Current Health: " + currentValue);
        }
    }

    #endregion MonoBehaviours

    #region OverrideFunctions
    public override void DecCurrentValue(float damage)
    {
        base.DecCurrentValue(damage);

        PlaySoundAfterTakingDamage();

        if (currentValue <= 0)
        {
            Die();
        }
    }

    public override void DecCurrentValueOverTime()
    {
        base.DecCurrentValueOverTime();

        if (currentValue <= 0)
        {
            Die();
        }
    }

    #endregion OverrideFunctions

    public virtual void Die()
    {
        AudioManager.instance.PlaySound2D(playSoundOnDeath);

        if (spawnedFrom == Spawner.SpawnerType.Player)
        {
            SpawnerManager.instance.IncrementPlayersLeft();
        }

        Destroy(gameObject);
    }

    public virtual void PlaySoundAfterTakingDamage()
    {
        AudioManager.instance.PlaySound3D(playSoundFromDamage, transform);
    }
}