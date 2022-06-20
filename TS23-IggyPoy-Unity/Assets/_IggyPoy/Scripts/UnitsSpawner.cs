using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Space]
    [SerializeField] private float functionDelayBetweenWaves = 25; // Not in seconds, (25 is a wave every 2 minutes aprox) but it is the "t" in this formula: https://www.geogebra.org/classic/vyrqqpxh
    [SerializeField] public UnitsSpawnConfiguration[] unitsToSpawn;
    [SerializeField] private float percentageDistanceFromCenterToSpawnUnit = 0.78f;

    /// <summary>
    /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
    /// </summary>
    private int unitsSeed => _randomNumberToAlterMainSeed + mapManager.mapConfiguration.seed + Mathf.CeilToInt(GameManager.instance.gameData.timeSinceSpawnStarted); //IT MUST NEVER CHANGE

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

    public void SpawnUnit(GameObject unit)
    {
        List<MapElement> spawned = mapManager.SpawnMapElementsRandomly(
            unit,
            unitsSeed,
            spawningHeightRange,
            1,
            this.transform,
            true,
            mapManager.mapConfiguration.mapRadius*percentageDistanceFromCenterToSpawnUnit
        );
        SetupNewUnits(spawned);
    }

    public void SpawnUnits(bool deletePreviousUnits)
    {
        if (deletePreviousUnits)
            DestroyAllUnits();

        foreach (UnitsSpawnConfiguration spawnConfiguration in unitsToSpawn)
        {
            int qtty = spawnConfiguration.GetQuantity(functionDelayBetweenWaves, GameManager.instance.gameData.timeSinceSpawnStarted);
            if (qtty <= 0)
                continue;
            List<MapElement> spawned = mapManager.SpawnMapElementsRandomly(
                spawnConfiguration.unit,
                unitsSeed,
                spawningHeightRange,
                qtty,
                this.transform,
                true,
            mapManager.mapConfiguration.mapRadius*percentageDistanceFromCenterToSpawnUnit
            );

            SetupNewUnits(spawned);
        }
        
    }

    private static void SetupNewUnits(List<MapElement> spawned)
    {
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
            unitsToSpawn[0].unit.GetComponentRequired<NavMeshAgent>(),
            mapManager.mapConfiguration, true);
    }
}

[System.Serializable]
public class UnitsSpawnConfiguration
{
    public float difficultyIncrement = 0.1f; //a
    public float spawnDelay = 0; //d
    public GameObject unit;

    // https://www.geogebra.org/classic/vyrqqpxh
    public int GetQuantity(float delayBetweenWaves, float timeSinceStart)
    {
        //f(x)=sin(((x)/(t))) x a+a x-d
        //f(x)=sin(((timeSinceStart)/(delayBetweenWaves))) timeSinceStart difficultyIncrement+difficultyIncrement timeSinceStart-spawnDelay
        float functionResult = Mathf.Sin(((timeSinceStart) / (delayBetweenWaves))) * timeSinceStart * difficultyIncrement + difficultyIncrement * timeSinceStart - spawnDelay;
        return Mathf.Max(0,Mathf.CeilToInt(functionResult/3));
    }
}