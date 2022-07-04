using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : StateController
{
    
    [SerializeField] private AttackController attackController;

    private void Start()
    {
        SetNewState();
    }

    public override void SetNewState()
    {
        if (attackController.target == null)
        {
            // Debug.Log("Unit state: to center");
            SetDestination(Vector3.zero); // TODO: Change for the precise location of the main structure
            //Todo: loop and play with ease, not cut
            // Debug.Log("playing walk animation (a)");
            if (!animancer.IsPlayingClip(walkAnimation))
                animancer.Play(walkAnimation, 0.25f, FadeMode.FromStart); // https://kybernetik.com.au/animancer/docs/examples/basics/playing-and-fading/#:~:text=very%20useful%20either.-,Good%20CrossFade%20from%20Start,-Fortunately%2C%20the%20optional
        }
        else if (Vector3.Distance(attackController.target.transform.position, this.transform.position) <= attackController.range)
        {
            // Debug.Log("Unit state: stop");
            StopMovement();
            //Todo: loop and play with ease, not cut
            // Debug.Log("playing attack animation");
            if (!animancer.IsPlayingClip(attackAnimation))
                animancer.Play(attackAnimation, 0.15f, FadeMode.FromStart); // https://kybernetik.com.au/animancer/docs/examples/basics/playing-and-fading/#:~:text=very%20useful%20either.-,Good%20CrossFade%20from%20Start,-Fortunately%2C%20the%20optional

        }
        else
        {
            // Debug.Log("Unit state: to target on the way");
            SetDestination(attackController.target.transform.position);
            //Todo: loop and play with ease, not cut
            // Debug.Log("playing walk animation (b)");
            if (!animancer.IsPlayingClip(walkAnimation))
                animancer.Play(walkAnimation, 0.25f, FadeMode.FromStart); // https://kybernetik.com.au/animancer/docs/examples/basics/playing-and-fading/#:~:text=very%20useful%20either.-,Good%20CrossFade%20from%20Start,-Fortunately%2C%20the%20optional
        }
    }
    
    /// <summary>
    /// Reference to the Animator handling the animations of this MapElement.
    /// </summary>
    private AnimancerComponent animancer
    {
        get
        {
            if (_animancer == null)
                _animancer = this.GetComponentRequired<AnimancerComponent>();
            return _animancer;
        }
    }
    private AnimancerComponent _animancer;
    [SerializeField] private AnimationClip walkAnimation;
    [SerializeField] private AnimationClip attackAnimation;

    
    /// <summary>
    /// Reference to the NavMeshAgent of this Unit
    /// </summary>
    private NavMeshAgent navMeshAgent
    {
        get
        {
            if (_navMeshAgent == null)
                _navMeshAgent = this.GetComponentRequired<NavMeshAgent>();
            return _navMeshAgent;
        }
    }
    private NavMeshAgent _navMeshAgent;
    
    
    private void SetDestination(Vector3 destination)
    {
        if (!enabled || !navMeshAgent.isActiveAndEnabled)
            return;
        
        navMeshAgent.destination = destination;
        navMeshAgent.isStopped = false; 
    }

    private void StopMovement()
    {
        navMeshAgent.isStopped = true;
    }

}

