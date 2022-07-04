using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class MapElement : MonoBehaviour
{
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AttackController attackController;

    private float pitchModification = 0.3f;
    
    public void PlayWalkAudio()
    {
        walkAudioSource.pitch = Random.Range(1-pitchModification, 1+pitchModification);
        walkAudioSource.Play();
    }

    public void PlayAttackAudio()
    {
        attackAudioSource.pitch = Random.Range(1-pitchModification, 1+pitchModification);
        attackAudioSource.Play();
        if (attackController != null)
            attackController.AttackTarget();
    }
    
    
    /////////////////// DEBUG ///////////////////
    
    [ContextMenu("Go to center")]
    public void GoToCenter()
    {
        SetDestination(Vector3.zero);
    }
    private void SetDestination(Vector3 destination)
    {
        NavMeshAgent navMeshAgent = gameObject.GetComponentRequired<NavMeshAgent>();
        if (!enabled || !navMeshAgent.isActiveAndEnabled)
            return;
        navMeshAgent.destination = destination;
        navMeshAgent.isStopped = false; 
    }

    [ContextMenu("Check Path")]
    public void CheckPath()
    {
        NavMeshAgent spawnedAgent = gameObject.GetComponentRequired<NavMeshAgent>();
        spawnedAgent.SetDestination(Vector3.zero);
        
        if (spawnedAgent.pathStatus != NavMeshPathStatus.PathComplete) {
            Debug.Log("PATH IS: " + spawnedAgent.pathStatus + $". isOnNavMesh = {spawnedAgent.isOnNavMesh}. navMeshOwner = {spawnedAgent.navMeshOwner}" , spawnedAgent);
        }
        else
        {
            Debug.Log($"PATH IS FOUND AND VALID. Status = {spawnedAgent.pathStatus}");
        }
    }
    
    // public void CheckPath() // Recomended way, didn't work
    // {
    //     NavMeshPath navMeshPath = new NavMeshPath();
    //     NavMeshAgent spawnedAgent = gameObject.GetComponentRequired<NavMeshAgent>();
    //     spawnedAgent.CalculatePath(Vector3.zero, navMeshPath);
    //     if (navMeshPath.status != NavMeshPathStatus.PathComplete) {
    //         Debug.Log("PATH IS: " + navMeshPath.status + $". isOnNavMesh = {spawnedAgent.isOnNavMesh}. navMeshOwner = {spawnedAgent.navMeshOwner}" , spawnedAgent);
    //     }
    //     else
    //     {
    //         Debug.Log("PATH IS FOUND AND VALID");
    //     }
    // }
}