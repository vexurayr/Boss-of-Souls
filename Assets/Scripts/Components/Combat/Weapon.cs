using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Variables
    [SerializeField] private DamageSource damageSource;
    [SerializeField] private string primaryActionTriggerName;
    [SerializeField] private string secondaryActionTriggerName;
    //[SerializeField] private float animationSpeed;
    [SerializeField] private Collider hitbox;
    [SerializeField] private string audioFileName;
    [SerializeField] private bool isDestroyedOnContact;

    private float primaryDamage;
    private float secondaryDamage;
    private Animator animator;
    private bool hasDealtDamageRecently = false;
    private float timeToTakeDamageAgain = 1f;

    #endregion Variables

    #region MonoBehaviours
    private void Start()
    {
        if (GetComponent<Animator>())
        {
            animator = GetComponent<Animator>();

            //animator.speed = animationSpeed;
        }

        primaryDamage = damageSource.GetCurrentValue();
        secondaryDamage = damageSource.GetCurrentSecondaryValue();
    }

    #endregion MonoBehaviours

    // This function is awful
    // Debug.Log reports this activating 1-5 times by the same punch (collider active one frame)
    // While DecCurrentValue is always called twice
    public virtual void OnTriggerEnter(Collider collider)
    {
        if (hasDealtDamageRecently)
        {
            return;
        }

        // Check if other collider has a health component
        if (collider.gameObject.GetComponent<Health>())
        {
            hasDealtDamageRecently = true;

            StartCoroutine("PreventMultihit");

            if (GetComponent<Animator>())
            {
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                string clipName = clipInfo[0].clip.name;

                if (clipName == "Left Hand Light" || clipName == "Right Hand Light")
                {
                    collider.gameObject.GetComponent<Health>().DecCurrentValue(primaryDamage);
                }
                else
                {
                    collider.gameObject.GetComponent<Health>().DecCurrentValue(secondaryDamage);
                }
            }
            else
            {
                collider.gameObject.GetComponent<Health>().DecCurrentValue(primaryDamage);
            }
        }

        if (isDestroyedOnContact)
        {
            Destroy(gameObject);
        }
    }

    #region Actions
    public bool PrimaryAction()
    {
        // If the animation is ready to play
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            AudioManager.instance.PlaySound3D(audioFileName, transform);

            animator.SetTrigger(primaryActionTriggerName);

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SecondaryAction()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            AudioManager.instance.PlaySound3D(audioFileName, transform);

            animator.SetTrigger(secondaryActionTriggerName);

            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion Actions

    public void AddDamageMultiplier(float multiplier)
    {
        primaryDamage = primaryDamage * multiplier;
        secondaryDamage = secondaryDamage * multiplier;
    }

    private IEnumerator PreventMultihit()
    {
        yield return new WaitForSeconds(timeToTakeDamageAgain);
        hasDealtDamageRecently = false;
    }
}