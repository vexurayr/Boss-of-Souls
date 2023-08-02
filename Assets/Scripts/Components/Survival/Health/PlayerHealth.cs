using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerHealth : Health
{
    #region OverrideFunctions
    public override void DecCurrentValue(float damage)
    {
        base.DecCurrentValue(damage);

        if (currentValue <= 0)
        {
            MenuManager.instance.ShowDeathScreen();
            
            GameManager.instance.SetCurrentPlayerController(null);

            Camera.main.GetComponent<AudioListener>().enabled = true;

            Die();
        }
    }

    public override void DecCurrentValueOverTime()
    {
        base.DecCurrentValueOverTime();

        if (currentValue <= 0)
        {
            MenuManager.instance.ShowDeathScreen();
            
            GameManager.instance.SetCurrentPlayerController(null);

            Camera.main.GetComponent<AudioListener>().enabled = true;

            Die();
        }
    }

    #endregion OverrideFunctions
}