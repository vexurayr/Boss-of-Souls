using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIController : MonoBehaviour
{
    #region Variables
    // All potential states in the finite state machine
    public enum AIState
    {
        Idle,
        RandomMovement,
        Chase,
        Flee,
        FaceNoise,
        SeekNoise,
        SeekAndAttack,
        DistanceAttack
    };

    // Set stats like how far away the AI can see or how close they need to be to attack
    [SerializeField] protected float aIFOV;
    [SerializeField] protected float eyesightDistance;
    [SerializeField] protected float earshotDistance;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float fleeDistance;
    [Range(0f, 1f)][SerializeField] protected float percentHealthToFlee;
    [Tooltip("Leave 0 to have no restriction.")][SerializeField] protected float maxDistanceFromSpawn;
    [SerializeField] protected float delayBetweenRandomMovement;
    [SerializeField] protected float timeToAgro;
    [SerializeField] protected float distanceFromPlayerToDisable;
    [SerializeField] protected GameObject raycastLocation;
    [SerializeField] protected AIState startState;

    // Last time the AI changed states
    protected float lastTimeStateChanged;

    // State the fsm is currently in
    protected AIState currentState;

    // AI controller's target, likely the player
    protected GameObject target;

    // Used for random movement & random observe
    protected Vector3 randomLocation;

    protected Vector3 startingLocation;

    // Used for Face Noise and Seek Noise
    protected GameObject noiseLocation;

    // Value between 0 and timeToAgro
    protected float currentAlertness;

    protected float currentDelayBetweenRandomMovement = 0;
    private NavMeshAgent navMeshAgent;

    #endregion Variables

    #region MonoBehaviours
    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startingLocation = gameObject.transform.position;
        randomLocation = GetRandomDirection();
    }

    #endregion MonoBehaviours

    #region Transitions
    // Option to override later for AI with different personalities
    public virtual void ChangeState(AIState newState)
    {
        // Change current state
        currentState = newState;

        // Saves the time this state changed
        lastTimeStateChanged = Time.time;
    }

    #endregion Transitions

    #region States
    public void Idle()
    { }

    // Polymorphism at its finest
    public void Seek(Vector3 targetVector)
    {
        navMeshAgent.destination = targetVector;
    }

    public void Seek(Transform targetTransform)
    {
        Seek(targetTransform.position);
    }

    public void Seek(GameObject target)
    {
        Seek(target.transform.position);
    }

    public void SeekPointBeforeTarget(float minDistance)
    {
        // Gets vector to target
        Vector3 vectorToTarget = target.transform.position - gameObject.transform.position;
        float targetDistance = Vector3.Distance(vectorToTarget, gameObject.transform.position);

        // Travel percentage of vector
        if (targetDistance < minDistance)
        {
            // Half vector length
            vectorToTarget = vectorToTarget * 0.5f;

            Seek(vectorToTarget);
        }
        // Travel vector with flat reduction
        else
        {
            vectorToTarget = vectorToTarget.normalized;

            // Slightly shorter vector
            vectorToTarget = vectorToTarget * (targetDistance - minDistance);

            Seek(vectorToTarget);
        }
    }

    public void Flee()
    {
        // Gets vector to target
        Vector3 vectorToTarget = target.transform.position - gameObject.transform.position;

        // Calculates vector away from target (opposite direction of going to target)
        Vector3 vectorAwayFromTarget = -vectorToTarget;

        // Find how far away the AI will travel
        // Compare how close the player is at the beginning of the flee to always be the flee distance away from the player
        // at the end of the flee
        float targetDistance = Vector3.Distance(vectorAwayFromTarget, gameObject.transform.position);
        float percentOfFleeDistance = targetDistance / fleeDistance;

        Vector3 fleeVector = vectorAwayFromTarget.normalized * percentOfFleeDistance;
        
        // Seek the point the AI needs to flee to
        Seek(gameObject.transform.position + fleeVector);
    }

    // Will need to be changed based on the AI pawn
    public virtual void Attack()
    {}

    // Virtual so that other scripts can override this for AI personalities
    public virtual void SeekAndAttack()
    {
        // Continually chases and attack target
        Seek(target);

        // Only attacks if in range and is lined up with the player
        RaycastHit targetToHit;
        Ray rayToTarget;

        if (!raycastLocation)
        {
            rayToTarget = new Ray(gameObject.transform.position, gameObject.transform.forward);
        }
        else
        {
            rayToTarget = new Ray(raycastLocation.transform.position, raycastLocation.transform.forward);
        }

        if (Physics.Raycast(rayToTarget, out targetToHit, attackDistance))
        {
            // Ray hit the player
            if (targetToHit.collider == target.GetComponent<Collider>())
            {
                Attack();
            }
        }
    }

    public void RandomMovement()
    {
        // This will prevent the AI from constantly looking for a new position to travel to
        if (HasReachedRandomLocation() || randomLocation == null)
        {
            ResetDelayBetweenRandomMovement();
            
            randomLocation = GetRandomDirection();
        }
        
        if (currentDelayBetweenRandomMovement <= 0)
        {
            Seek(randomLocation);
        }
        
        CountdownDelayBetweenRandomMovement();
    }

    // Will have to be changed based on the AI pawn
    public void FaceNoise()
    { }

    public void SeekNoise()
    {
        // This will prevent the agent from moving when it reaches the source of the noise
        if (!IsDistanceLessThan(noiseLocation, 1))
        {
            navMeshAgent.destination = noiseLocation.transform.position;
        }
    }

    public void DistanceAttack()
    {
        // This agent will always try to remain attack distance away from the player as it attacks
        // +/- 1 so that the agent isn't jittery while trying to remain in one place
        if (IsDistanceLessThan(target, attackDistance - 1))
        {
            
        }
        else if (!IsDistanceLessThan(target, attackDistance + 1))
        {
            
        }

        // Only attacks if in range and is lined up with the player
        RaycastHit targetToHit;
        Ray rayToTarget;

        if (!raycastLocation)
        {
            rayToTarget = new Ray(gameObject.transform.position, gameObject.transform.forward);
        }
        else
        {
            rayToTarget = new Ray(raycastLocation.transform.position, raycastLocation.transform.forward);
        }

        if (Physics.Raycast(rayToTarget, out targetToHit, attackDistance))
        {
            // Ray hit the player
            if (targetToHit.collider == target.GetComponent<Collider>())
            {
                Attack();
            }
        }
    }

    #endregion States

    #region Targeting Options
    public virtual void Target(GameObject newTarget)
    {
        target = newTarget;
    }

    public virtual void TargetPlayer()
    {
        // Must be instance of game manager and list of players, and have players in the list
        if (GameManager.instance != null)
        {
            if (GameManager.instance.GetCurrentPlayerController() != null)
            {
                Target(GameManager.instance.GetCurrentPlayerController().gameObject);
            }
        }
    }

    #endregion Targeting Options

    #region Other Methods
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public Vector3 GetRandomDirection()
    {
        if (maxDistanceFromSpawn > 0)
        {
            // Pick a random direction in a circle around the start location
            float radius = Random.Range(0, maxDistanceFromSpawn);
            float theta = Random.Range(0, Mathf.PI * 2);

            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);

            Vector3 direction = new Vector3(x, 0, z);
            direction += startingLocation;

            return direction;
        }
        else
        {
            // Pick a random direction in a circle around the current location
            float radius = Random.Range(0, 10);
            float theta = Random.Range(0, Mathf.PI * 2);

            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);

            Vector3 direction = new Vector3(x, 0, z);
            direction += gameObject.transform.position;

            return direction;
        }
    }

    public void CountdownDelayBetweenRandomMovement()
    {
        currentDelayBetweenRandomMovement -= Time.deltaTime;

        currentDelayBetweenRandomMovement = Mathf.Clamp(currentDelayBetweenRandomMovement, 0, delayBetweenRandomMovement);
    }

    public void ResetDelayBetweenRandomMovement()
    {
        currentDelayBetweenRandomMovement = delayBetweenRandomMovement;
    }

    #endregion Other Methods

    #region HelperFunctions
    public bool IsDistanceLessThan(Vector3 target, float distance)
    {
        if (target == null)
        {
            return false;
        }

        // Checks distance between 2 vectors
        if (Vector3.Distance(gameObject.transform.position, target) < distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsDistanceLessThan(GameObject target, float distance)
    {
        return IsDistanceLessThan(target.transform.position, distance);
    }

    public bool CanSeeTarget()
    {
        if (!target)
        {
            return false;
        }

        // Gets the vector between self and target
        Vector3 selfToTargetVector = target.transform.position - gameObject.transform.position;

        // Gets angle between that vector and the direction self is facing
        float angleToTarget = Vector3.Angle(selfToTargetVector, gameObject.transform.forward);

        // Angle is less than AI POV and distance to target is less than eyesight distance
        // meaning the player is within the cone the AI can see
        if (angleToTarget < aIFOV && IsDistanceLessThan(target, eyesightDistance))
        {
            // Now must check if the player is in line of sight
            // Sends ray from self in direction of target location
            RaycastHit targetToHit;
            Vector3 rayPosition;
            if (!raycastLocation)
            {
                rayPosition = gameObject.transform.position;
            }
            else
            {
                rayPosition = raycastLocation.transform.position;
            }

            Ray rayToTarget = new Ray(rayPosition, selfToTargetVector);

            Debug.DrawRay(rayToTarget.origin, selfToTargetVector);

            // Ray is able to hit something
            if (Physics.Raycast(rayToTarget, out targetToHit, eyesightDistance))
            {
                // Check if ray hit target
                if (targetToHit.collider == target.GetComponent<Collider>())
                {
                    //Debug.Log("I can see the " + obj + "!");
                    return true;
                }
                // Ray didn't hit target
                else
                {
                    //Debug.Log("Ray didn't hit target");
                    return false;
                }
            }
            // Ray didn't hit anything
            else
            {
                //Debug.Log("Ray didn't hit anything");
                return false;
            }
        }
        // Target was not in cone of vision
        else
        {
            //Debug.Log("Target was not in cone of vision");
            return false;
        }
    }

    public bool HasReachedRandomLocation()
    {
        if (IsDistanceLessThan(randomLocation, 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion HelperFunctions
}