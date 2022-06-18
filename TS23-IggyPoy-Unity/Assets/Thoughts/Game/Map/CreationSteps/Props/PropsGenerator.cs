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
            _randomNumberToAlterMainSeed + mapManager.mapConfiguration.seed; //IT MUST NEVER CHANGE

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
                    false
                );
            }


            unitsSpawner.RegenerateNavMeshForUnits();

            if (mapManager.mapGenerator.IsSpawnablePositionOnTerrain(Vector2.zero, Vector2.up, true, out Vector3 spawnablePosition))
            {
                MapElement mainStructureSpawned = mapManager.mapGenerator.SpawnAsMapElement(mainStructure, spawnablePosition, Quaternion.identity, mainStructureParent);
                StructureController mainStructureSpawnedStructureController = mainStructureSpawned.GetComponentRequired<StructureController>();
                mainStructureSpawnedStructureController.team = PropertyController.Team.Player;
                mainStructureSpawnedStructureController.isPlaced = true;
            }
            else
            {
                Debug.LogError("No location found to spawn the initial/main structure");
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