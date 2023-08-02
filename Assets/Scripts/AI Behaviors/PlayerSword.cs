using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSword : AIController
{
    #region Variables
    [SerializeField] private Weapon sword;
    [SerializeField] private SurvivalUI survivalUI;

    private Health health;

    #endregion Variables

    #region MonoBehaviours
    protected override void Start()
    {
        base.Start();

        health = GetComponent<Health>();

        StartingState();
    }

    private void Update()
    {
        if (target == null)
        {
            TargetPlayer();
            return;
        }

        // Disables AI decision making when its too far from the player
        if (!IsDistanceLessThan(GameManager.instance.GetCurrentPlayerController().gameObject, distanceFromPlayerToDisable))
        {
            return;
        }

        RefreshHealthUI();
        MakeDecisions();
    }

    #endregion MonoBehaviours

    #region FSM
    public void StartingState()
    {
        ChangeState(startState);
    }

    public void MakeDecisions()
    {
        switch (currentState)
        {
            case AIState.Idle:
                // Does the actions of the state
                Idle();
                
                TargetPlayer();

                // Check for transitions
                if (target)
                {
                    ChangeState(AIState.Chase);
                }
                else if (GetComponent<Health>().GetCurrentValue() / GetComponent<Health>().GetMaxValue() <= percentHealthToFlee)
                {
                    ChangeState(AIState.Flee);
                }

                break;

            case AIState.Chase:
                // Do state actions
                if (target == null)
                {
                    ChangeState(AIState.Idle);
                }
                else
                {
                    Seek(target);
                }
                
                // Check state transitions
                if (GetComponent<Health>().GetCurrentValue() / GetComponent<Health>().GetMaxValue() <= percentHealthToFlee)
                {
                    ChangeState(AIState.Flee);
                }
                else if (IsDistanceLessThan(target, attackDistance))
                {
                    ChangeState(AIState.SeekAndAttack);
                }

                break;
            case AIState.SeekAndAttack:
                if (target == null)
                {
                    ChangeState(AIState.Idle);
                }
                else
                {
                    SeekAndAttack();
                }
                
                if (!IsDistanceLessThan(target, attackDistance))
                {
                    ChangeState(AIState.Chase);
                }

                break;
            case AIState.Flee:
                if (target == null)
                {
                    ChangeState(AIState.Idle);
                }
                else
                {
                    Flee();
                }
                
                if (!IsDistanceLessThan(target, fleeDistance))
                {
                    ChangeState(AIState.Idle);
                }

                break;
            default:
                Debug.LogError("The switch could not determine the current state.");

                break;
        }
    }

    #endregion FSM

    #region OverrideFunctions
    public override void Attack()
    {
        sword.PrimaryAction();
    }

    #endregion OverrideFunctions

    private void RefreshHealthUI()
    {
        survivalUI.RefreshHealthUI(health.GetCurrentValue(), health.GetMaxValue());
    }
}