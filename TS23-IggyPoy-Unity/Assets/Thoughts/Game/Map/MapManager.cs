using System;
using System.Collections.Generic;
using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    /// <summary>
    /// The map of the game.
    /// </summary>
    [RequireComponent(typeof(MapGenerator))]
    public class MapManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the MapConfiguration with he settings to generate a map
        /// </summary>
        [Tooltip("Reference to the MapConfiguration with he settings to generate a map")]
        [SerializeField] public MapConfiguration mapConfiguration;
        
        /// <summary>
        /// Reference to the MapGenerator component, the manager of the generation of the map
        /// </summary>
        internal MapGenerator mapGenerator { get {
            if (_mapGenerator == null) _mapGenerator = this.GetComponentRequired<MapGenerator>();
            return _mapGenerator;
        } }
        private MapGenerator _mapGenerator;
        
        /// <summary>
        /// All the map elements present in the map.
        /// </summary>
        [NonSerialized] public List<MapElement> existentMapElements = new List<MapElement>();
        
        /// <summary>
        ///  Reference to the manager of the AI navigation
        /// </summary>
        [Tooltip("Reference to the manager of the AI navigation")]
        [SerializeField] public MapNavigationManager navigationManager;

    #region MapGeneration

        /// <summary>
        /// The previously created map is destroyed and a new FULL map (with all the creation steps) is generated.
        /// </summary>
        public void RegenerateFullMap()
        {
            mapGenerator.RegenerateFullMap();
            existentMapElements.Clear();
        }
        
        /// <summary>
        /// Generates the contents of a creation step
        /// </summary>
        public void RegenerateCreationStep(CreationStep creationStep)
        {
            mapGenerator.Regenerate(creationStep);
        }
        
        /// <summary>
        /// Deletes the currently (generated) existent map
        /// </summary>
        public void DeleteMap()
        {
            mapGenerator.DeleteMap();
            existentMapElements.Clear();
        }

    #endregion

    public MapElement GetClosestMapElementTo(Vector3 position, List<MapElement> exceptions = null)
        {
            MapElement bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            foreach(MapElement mapElement in existentMapElements)
            {
                if (exceptions != null && exceptions.Contains(mapElement))
                    continue;
                Vector3 directionToTarget = mapElement.transform.position - position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = mapElement;
                }
            }
     
            return bestTarget;
        }

        public float GetHeightAt(Vector2 location)
        {
            return mapGenerator.terrainGenerator.GetHeightAt(location);
        }
        
        public bool IsLocationUnderWater(Vector2 worldCoords)
        {
            return GetHeightAt(worldCoords) < mapConfiguration.seaHeightAbsolute;
        } 

        public TerrainType GetTerrainTypeAtLocation(Vector2 location)
        {
            throw new NotImplementedException();
        }

        public List<MapElement> SpawnMapElementsRandomly(GameObject objectToSpawn, int seed, Vector2 spawningHeightRange, int quantity, Transform parent, bool requireNavMesh, float minDistanceFromCenterToSpawn)
        {
            Debug.Log($"Spawning x{quantity} '{objectToSpawn.gameObject.name}'");
            return mapGenerator.SpawnMapElementsRandomly(objectToSpawn, seed, spawningHeightRange, quantity, parent, requireNavMesh, minDistanceFromCenterToSpawn);
        }
        
        public MapElement SpawnMapElement(GameObject objectToSpawn, Vector3 position, Quaternion rotation, Transform parent)
        {
            return mapGenerator.SpawnAsMapElement(objectToSpawn, position, rotation, parent);
        }

        public List<MapElement> SpawnMapElementsWithPerlinNoiseDistribution(GameObject objectToSpawn, int seed, Vector2 spawningHeightRange, float probability, float density, Transform parent, NoiseMapSettings noiseMapSettings, bool requireNavMesh)
        {
            return mapGenerator.SpawnMapElementsWithPerlinNoiseDistribution(objectToSpawn, seed,  spawningHeightRange, probability, density, parent, noiseMapSettings, requireNavMesh);
        }
    }

    public enum TerrainType
    {
        none = 0,
        sea,
        interior,
        interiorShoreline,
        land,
    }
}