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
                    mapManager.mapConfiguration.propsSettings.mapElementsToSpawn[v].centralAreaToAvoid
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
            }
            else
            {
                Debug.LogWarning("No location found to spawn the initial/main structure. ResetScene in UIManager being called.");
                UIManager.instance.ResetScene();
            }

            InvokeOnFinishStepGeneration();
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