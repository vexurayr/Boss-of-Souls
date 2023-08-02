using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    //[SerializeField] private Text healthText;

    public void RefreshHealthUI(float currentHealth, float maxHealth)
    {
        float scale = currentHealth / maxHealth;

        healthBar.transform.localScale = new Vector3(scale, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //healthText.text = currentHealth.ToString("F0") + "/" + maxHealth.ToString("F0");
    }
}