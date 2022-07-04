using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map.CreationSteps.Vegetation
{
    public class PropsGenerator : CreationStepGenerator
    {
        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int vegetationSeed =>
            _randomNumberToAlterMainSeed + GameManager.instance.seed; //IT MUST NEVER CHANGE

        private const int
            _randomNumberToAlterMainSeed =
                5151335; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)

        [SerializeField] public UnitsSpawner unitsSpawner;
        [SerializeField] private Transform mainStructureParent;
        [SerializeField] private GameObject mainStructure;
        [SerializeField] private GameObject testPathAgent;

        // ReSharper disable Unity.PerformanceAnalysis
        private void GenerateProps(bool clearPrevious)
        {
            if (clearPrevious)
                Delete();

            for (int v = 0; v < mapManager.mapConfiguration.propsSettings.mapElementsToSpawn.Length; v++)
            {
                mapManager.mapGenerator.SpawnMapElementsWithPerlinNoiseDistribution(
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].mapElementPrefab,
                    vegetationSeed,
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].spawningHeightRange,
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].probability,
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].density,
                    this.transform,
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].noiseSettings,
                    false,
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].centralAreaToAvoid,
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].maxDistanceFromCenter
                );
            }


            unitsSpawner.RegenerateNavMeshForUnits();

            if (mapManager.mapGenerator.IsSpawnablePositionOnTerrain(Vector2.zero, Vector2.up, true, out Vector3 spawnablePosition))
            {
                MapElement mainStructureSpawned = mapManager.mapGenerator.SpawnAsMapElement(mainStructure, spawnablePosition, Quaternion.Euler(new Vector3(0, Random.Range(0.0f, 360.0f), 0)), mainStructureParent);
                StructureController mainStructureSpawnedStructureController = mainStructureSpawned.GetComponentRequired<StructureController>();
                mainStructureSpawnedStructureController.team = PropertyController.Team.Player;
                mainStructureSpawnedStructureController.isPlaced = true;
                mainStructureSpawnedStructureController.energySource = mainStructureSpawned.GetComponentRequired<EnergySource>();
                UIManager.instance.HideLoadingScreen();
            }
            else
            {
                Debug.LogWarning("No location found to spawn the initial/main structure. ResetScene in UIManager being called.");
                UIManager.instance.ResetScene();
            }

            if (!AreAllPathsPossible())
            {
                Debug.LogWarning("Not all paths to the main structure are possible. ResetScene in UIManager being called.");
                UIManager.instance.ResetScene();
            }
            
            InvokeOnFinishStepGeneration();
        }

        private bool AreAllPathsPossible()
        {
            int issuesFound = 0;
            int totalPathsToTry = 20;
            List<MapElement> spawned = mapManager.SpawnMapElementsRandomly(
                testPathAgent,
                42069, // random
                new Vector2(0.0f, 0.2f), // Hardcoded. Values copied from prefab "Units" (UnitsSpawner)
                totalPathsToTry,
                this.transform,
                true,
                mapManager.mapConfiguration.mapRadius*0.78f // Hardcoded. Values copied from prefab "Units" (UnitsSpawner)
            );

            for (int i = 0; i < spawned.Count; i++)
            {
                
                NavMeshAgent spawnedAgent = spawned[i].GetComponentRequired<NavMeshAgent>();
                spawnedAgent.SetDestination(Vector3.zero);
                // print("New path calculated");
                if (spawnedAgent.pathStatus != NavMeshPathStatus.PathComplete) {
                    // Debug.LogWarning("Calculated path to check reachability of main tower is not complete. A new map should be generated.");
                    Debug.LogWarning(spawnedAgent.pathStatus + $" found. isOnNavMesh = {spawnedAgent.isOnNavMesh}. navMeshOwner = {spawnedAgent.navMeshOwner}" , spawnedAgent);
                    issuesFound ++;
                }
            }

            foreach (MapElement mapElement in spawned)
            {
                Destroy(mapElement.gameObject);
            }
            
            if (issuesFound == 0)
                Debug.Log("The main structure can be perfectly reached!!!");
            else
                Debug.LogWarning($"The main structure can be reached by {totalPathsToTry-issuesFound}/{totalPathsToTry}");
            
            return issuesFound == 0;
        }


        protected override void _DeleteStep()
        {
            mapManager.mapGenerator.DestroyAllMapElementsChildOf(this.transform);
            InvokeOnFinishStepDeletion();
        }

        protected override void _GenerateStep(bool clearPrevious)
        {
            GenerateProps(clearPrevious);
        }
    }
}