using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
using Thoughts.Game.Map;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class UnitsSpawner : MonoBehaviour
{
    /// <summary>
    /// The map of the game.
    /// <para>A component in a GameObject</para>
    /// </summary>
    [Header("Game Elements")] [SerializeField]
    public MapManager mapManager;

    /// <summary>
    ///  Reference to the manager of the AI navigation
    /// </summary>
    [Tooltip("Reference to the manager of the AI navigation")] [SerializeField]
    public MapNavigationManager navigationManager;

    /// <summary>
    /// In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.
    /// </summary>
    [Tooltip("In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.")]
    [MinMaxSlider(-1,1)]
    public Vector2 spawningHeightRange;
    
    [SerializeField] private GameObject[] unitsToSpawn;

    /// <summary>
    /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
    /// </summary>
    private int unitsSeed => _randomNumberToAlterMainSeed + mapManager.mapConfiguration.seed; //IT MUST NEVER CHANGE

    private const int
        _randomNumberToAlterMainSeed =
            345678; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)


    public void DestroyAllUnits()
    {
        mapManager.mapGenerator.DestroyAllMapElementsChildOf(this.transform);
    }
    
    
    // TODO: Delete, only used to debug in inspector
    [ContextMenu("Spawn New Units")]
    public void InspectorSpawnUnits()
    {
        SpawnUnits(false);
    }

    public void SpawnUnits(bool deletePreviousUnits)
    {
        if (deletePreviousUnits)
            DestroyAllUnits();

        if (unitsToSpawn.Length > 1)
        {
            Debug.LogWarning("The spawning of more than one type has not been implemented"); // Warning from the game "Thoughts"
        }

        List<MapElement> spawned = mapManager.SpawnMapElementsRandomly(
            unitsToSpawn[0],
            unitsSeed,
            spawningHeightRange,
            1, // TODO: Proper quantity and spawning
            this.transform,
            true
        );

        foreach (MapElement mapElement in spawned)
        {
            NavMeshAgent nmAgent = mapElement.gameObject.GetComponentRequired<NavMeshAgent>();
            nmAgent.destination = Vector3.zero;
            nmAgent.isStopped = false;
            mapElement.GetComponentRequired<PropertyController>().team = PropertyController.Team.Enemy;
        }
        
    }

    public void RegenerateNavMeshForUnits()
    {
        NavMeshSurface navMeshSurface = navigationManager.SetupNewNavMeshFor(
            unitsToSpawn[0].GetComponentRequired<NavMeshAgent>(),
            mapManager.mapConfiguration, true);
    }
}