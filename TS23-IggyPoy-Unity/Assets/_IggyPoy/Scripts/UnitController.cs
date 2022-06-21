using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : StateController
{
    
    [SerializeField] private AttackController attackController;
    
    public override void SetNewState()
    {
        if (attackController.target == null)
        {
            // Debug.Log("Unit state: to center");
            SetDestination(Vector3.zero); // TODO: Change for the precise location of the main structure
            //Todo: loop and play with ease, not cut
            // Debug.Log("playing walk animation (a)");
            animancer.Play(walkAnimation, 0.25f, FadeMode.FromStart); // https://kybernetik.com.au/animancer/docs/examples/basics/playing-and-fading/#:~:text=very%20useful%20either.-,Good%20CrossFade%20from%20Start,-Fortunately%2C%20the%20optional

        }
        else if (Vector3.Distance(attackController.target.transform.position, this.transform.position) <= attackController.range)
        {
            // Debug.Log("Unit state: stop");
            StopMovement();
            //Todo: loop and play with ease, not cut
            // Debug.Log("playing attack animation");
            animancer.Play(attackAnimation, 0.15f, FadeMode.FromStart); // https://kybernetik.com.au/animancer/docs/examples/basics/playing-and-fading/#:~:text=very%20useful%20either.-,Good%20CrossFade%20from%20Start,-Fortunately%2C%20the%20optional

        }
        else
        {
            // Debug.Log("Unit state: to target on the way");
            SetDestination(attackController.target.transform.position);
            //Todo: loop and play with ease, not cut
            // Debug.Log("playing walk animation (b)");
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

    private void SetDestination(Vector3 destination)
    {
        gameObject.GetComponentRequired<NavMeshAgent>().destination = destination;
        gameObject.GetComponentRequired<NavMeshAgent>().isStopped = false; 
    }
    
    private void StopMovement()
    {
        gameObject.GetComponentRequired<NavMeshAgent>().isStopped = true; 
    }
    
}

