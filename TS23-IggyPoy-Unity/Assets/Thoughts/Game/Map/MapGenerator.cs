using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thoughts.Game.Map.CreationSteps.Terrain;
using Thoughts.Game.Map.CreationSteps.Vegetation;
using Thoughts.Utils.Maths;
using Thoughts.Utils.ThreadsManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map
{
    /// <summary>
    /// Component in charge of generating a Map
    /// </summary>
    [RequireComponent(typeof(MapManager))]
    [RequireComponent(typeof(ThreadedDataRequester))]
    public class MapGenerator : MonoBehaviour
    {
        private MapManager mapManager
        {
            get
            {
                if (_mapManager == null) _mapManager = this.GetComponentRequired<MapManager>();
                return _mapManager;
            }
        }

        private MapManager _mapManager;

        /// <summary>
        /// Reference to the ThreadedDataRequester component in charge doing threaded requests of data
        /// </summary>
        public ThreadedDataRequester threadedDataRequester
        {
            get
            {
                if (_threadedDataRequester == null)
                    _threadedDataRequester = this.GetComponentRequired<ThreadedDataRequester>();
                return _threadedDataRequester;
            }
        }

        private ThreadedDataRequester _threadedDataRequester;

        #region StepsGenerators

        [TitleGroup("Steps Generators")] [SerializeField]
        public TerrainGenerator terrainGenerator;

        [FormerlySerializedAs("vegetationGenerator")] [TitleGroup("Steps Generators")] [SerializeField]
        private PropsGenerator propsGenerator;

        #endregion

        #region Generation in Editor Generation

        [TitleGroup("Editor Generators")] [HorizontalGroup("Editor Generators/EditorStep")] [ShowInInspector]
        private CreationStep editorRegenerationStep;

        [HorizontalGroup("Editor Generators/EditorStep")]
        [Button("Regenerate step")]
        private void EditorRegenerate()
        {
            Regenerate(editorRegenerationStep);
        }

        [ButtonGroup("Editor Generators/Regeneration")]
        [Button("Delete all")]
        private void EditorDelete()
        {
            DeleteMap();
        }

        [ButtonGroup("Editor Generators/Regeneration")]
        [Button("Del. all & regenerate terrain")]
        private void EditorDeleteAndRegenerate()
        {
            DeleteMap();
            RegenerateTerrain();
        }

        [ButtonGroup("Editor Generators/Regeneration")]
        [Button("Regenerate all")]
        private void EditorRegenerateAll()
        {
            RegenerateFullMap();
        }

        [Button("Refresh Auto Update (UpdatableData links)")]
        private void ForceOnValidate()
        {
            OnValidate();
        }

        /*
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete all"))
        {
            mapGenerator.DeleteMap();
        }
        if (GUILayout.Button("Del. all & regenerate terrain"))
        {
        mapGenerator.DeleteMap();
        mapGenerator.RegenerateTerrain();
        }
                
        if (GUILayout.Button("Regenerate all"))
        {
        mapGenerator.RegenerateFullMap();
        }
        GUILayout.EndHorizontal();
                
        if (GUILayout.Button("Refresh Auto Update (UpdatableData links)"))
        {
        mapGenerator.OnValidate();
        }
        */

        #endregion


#if UNITY_EDITOR
        /*
        [Header("Behaviour")]
        [SerializeField] private bool regenerateFullOnRecompilation = false;
         
        void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            CompilationPipeline.compilationFinished         += OnAfterAssemblyReload;
            CompilationPipeline.assemblyCompilationFinished += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            CompilationPipeline.compilationFinished         -= OnAfterAssemblyReload;
            CompilationPipeline.assemblyCompilationFinished -= OnAfterAssemblyReload;
        }

        public void OnAfterAssemblyReload() // Called after recompilation / assembly reload
        {
            Debug.Log("After Assembly Reload Map Generator");

            //Regenerate map after compilation
            if (regenerateFullOnRecompilation)
                RegenerateFull();
        }
        
        public static void TT2() // Called after recompilation / assembly reload
        {
            Debug.Log("TT2");
            Debug.Log("asdsad");

            //Regenerate map after compilation
            //if (regenerateFullOnRecompilation)
                GameManager.instance.mapManager.mapGenerator.RegenerateFull();
        }
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateAssetWhenReady()
        {
            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += CreateAssetWhenReady;
                return;
            }
 
            EditorApplication.delayCall += TT2;
        }
        */
        void OnDrawGizmos()
        {
            // Ensure continuous Update calls. Needed to generate the map in the editor (issues with threads)
            if (!Application.isPlaying)
            {
                UnityEditor.SceneView.RepaintAll();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            }

            //Draws the lines to show where the base, sea and max height are
            float linesHalfSize = 1000;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(new Vector3(-linesHalfSize, 0, 0), new Vector3(linesHalfSize, 0, 0));
            Gizmos.color = Color.blue;
            float seaHeight = mapManager.mapConfiguration.seaHeightAbsolute;
            Gizmos.DrawLine(new Vector3(-linesHalfSize, seaHeight, 0), new Vector3(linesHalfSize, seaHeight, 0));
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(-linesHalfSize, mapManager.mapConfiguration.terrainHeightSettings.maxHeight, 0),
                new Vector3(linesHalfSize, mapManager.mapConfiguration.terrainHeightSettings.maxHeight, 0));
        }
#endif

        public void OnValidate()
        {
            //GENERAL
            if (mapManager.mapConfiguration != null)
            {
                mapManager.mapConfiguration.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapManager.mapConfiguration.OnValuesUpdated += RegenerateFullMap;
            }
            else
            {
                Debug.LogWarning($"mapManager.MapConfiguration in MapGenerator in {gameObject.name} is null.");
                return;
            }

            //Terrain
            if (mapManager.mapConfiguration.terrainHeightSettings != null)
            {
                mapManager.mapConfiguration.terrainHeightSettings
                    .ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapManager.mapConfiguration.terrainHeightSettings.OnValuesUpdated +=
                    RegenerateTerrain; //RegenerateFull;
            }

            if (mapManager.mapConfiguration.terrainTextureSettings != null)
            {
                mapManager.mapConfiguration.terrainTextureSettings
                    .ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapManager.mapConfiguration.terrainTextureSettings.OnValuesUpdated += RegenerateTerrainTextures;
            }

            //Props
            if (mapManager.mapConfiguration.propsSettings != null)
            {
                mapManager.mapConfiguration.propsSettings
                    .ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapManager.mapConfiguration.propsSettings.OnValuesUpdated += RegenerateProps;
            }
        }

        /// <summary>
        /// Manages the update of the TextureSettings by applying them to the map's Material
        /// </summary>
        void RegenerateTerrainTextures()
        {
            mapManager.mapConfiguration.terrainTextureSettings.ApplyToMaterial(
                mapManager.mapConfiguration.terrainHeightSettings.minHeight,
                mapManager.mapConfiguration.terrainHeightSettings.maxHeight);
        }


        /// <summary>
        /// Deletes the currently (generated) existent map
        /// </summary>
        public void DeleteMap()
        {
            Debug.Log("navigationManager.RemoveAllNavMesh");
            //mapManager.navigationManager.RemoveAllNavMesh();
            terrainGenerator.Delete(true); // Must be the first one
        }

        public Vector2Int GetRelativeChunksCoordsAt(Vector2 absoluteCoords)
        {
            int chunkCordX = Mathf.RoundToInt(absoluteCoords.x / mapManager.mapConfiguration.chunkWorldSize);
            int chunkCordY = Mathf.RoundToInt(absoluteCoords.y / mapManager.mapConfiguration.chunkWorldSize);
            return new Vector2Int(chunkCordX, chunkCordY);
        }

        public void RegenerateTerrain()
        {
            Regenerate(CreationStep.Terrain);
        }

        public void RegenerateProps()
        {
            Regenerate(CreationStep.Props);
        }

        /// <summary>
        /// The previously created map is destroyed and a new FULL map (with all the creation steps) is generated.
        /// </summary>
        public void RegenerateFullMap()
        {
            Regenerate(CreationStep.Terrain, true);
        }

        /// <summary>
        /// Regenerates the things related to the given creation step 
        /// </summary>
        /// <param name="step">The creation step that contains the things that are wanted to be regenerated</param>
        public void Regenerate(CreationStep step, bool generateNextStepOnFinish = false)
        {
            // Debug.Log($"Regenerating '{step.ToString()}'");


            switch (step)
            {
                case CreationStep.Terrain:
                    terrainGenerator.Generate(true, generateNextStepOnFinish);
                    RegenerateTerrainTextures();
                    break;

                case CreationStep.Props:
                    propsGenerator.Generate(true, generateNextStepOnFinish);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(step), step,
                        $"Trying to generate creation step with no generation process: {Enum.GetName(typeof(CreationStep), step)}");
            }
        }


        /// <summary>
        /// Makes a map element spawn.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <returns></returns>
        public MapElement SpawnMapElement(GameObject objectToSpawn, Vector3 position, Quaternion rotation,
            Transform parent)
        {
            GameObject spawnedMapElement = Instantiate(objectToSpawn, position, rotation, parent);
            spawnedMapElement.name = objectToSpawn.name;
            MapElement spawnedElement = spawnedMapElement.GetComponentRequired<MapElement>();
            mapManager.existentMapElements.Add(spawnedElement);
            return spawnedElement;
        }

        public void DestroyAllMapElementsChildOf(Transform parentOfMapElements)
        {
            //Debug.Log($"DESTROYING ALL FROM {parentOfMapElements.transform.name}");

            if (!Application.isPlaying)
            {
                parentOfMapElements.DestroyImmediateAllChildren();
                return;
            }

            foreach (Transform child in parentOfMapElements)
            {
                MapElement mapElement = child.GetComponent<MapElement>();
                if (mapElement != null)
                {
                    DestroyMapElement(mapElement);
                }
            }
        }

        public void DestroyMapElement(MapElement mapElement)
        {
            if (!mapManager.existentMapElements.Remove(mapElement))
                Debug.Log(
                    $"The MapElement {mapElement.name} was not registered in the 'existentMapElements' but it was intended to destroy it.",
                    mapElement);

            if (Application.isPlaying)
                Destroy(mapElement.gameObject);
            else
                DestroyImmediate(mapElement.gameObject);
        }

        /// <summary>
        /// Spawns a group of MapElements in the map using a pseudo random perlin noise distribution.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="seed">The seed to use to generate the perlin noise</param>
        /// <param name="spawningHeightRange">In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.</param>
        /// <param name="probability">The probability of the object being spawned at any given spot (following the perlin noise distribution)</param>
        /// <param name="density">The density of the spawning. 1 meaning all the available spots where the probability says spawn should happen will be filled. 0 means none.</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <param name="noiseMapSettings">The settings to be used for the perlin noise map</param>
        /// <param name="requireNavMesh">Must the locations where the MapElements will spawn require a valid NavMeshSurface?</param>
        public List<MapElement> SpawnMapElementsWithPerlinNoiseDistribution(GameObject objectToSpawn, int seed,
            Vector2 spawningHeightRange, float probability, float density, Transform parent,
            NoiseMapSettings noiseMapSettings, bool requireNavMesh)
        {
            List<MapElement> spawnedMapElements = new List<MapElement>();
            RandomEssentials rng = new RandomEssentials(seed);

            float[,] noise = Noise.GenerateNoiseMap((int) mapManager.mapConfiguration.mapRadius * 2,
                (int) mapManager.mapConfiguration.mapRadius * 2, noiseMapSettings, Vector2.zero, seed);
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    if (!(noise[x, y] >= 1 - probability))
                        continue;

                    if (rng.GetRandomBool(1 - density))
                        continue;

                    if (IsSpawnablePositionOnTerrain(
                        new Vector2(x - mapManager.mapConfiguration.mapRadius,
                            y - mapManager.mapConfiguration.mapRadius), spawningHeightRange, requireNavMesh,
                        out Vector3 spawnablePosition))
                        spawnedMapElements.Add(SpawnMapElement(objectToSpawn, spawnablePosition, Quaternion.identity,
                            parent));
                }
            }

            return spawnedMapElements;
        }

        /// <summary>
        /// Checks if a MapElement can be spawned or not in a given position.
        /// </summary>
        /// <param name="positionCheck">The 2D position to check if a Map</param>
        /// <param name="spawningHeightRange">The normalized height at which the object can be spawned (-1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.)</param>
        /// <param name="requireNavMesh">Must the location require a valid NavMeshSurface?</param>
        /// <param name="spawnablePosition">If the given position allows an spawn, this is the 3D position (including the height at which it can be spawned) so there is no need to recalculate it again.</param>
        /// <returns>True if the location allows the spawn of the MapElement, false otherwise.</returns>
        private bool IsSpawnablePositionOnTerrain(Vector2 positionCheck, Vector2 spawningHeightRange,
            bool requireNavMesh, out Vector3 spawnablePosition)
        {
            spawnablePosition = Vector3.zero;

            //float raySecureOffset = 0.5f;
            float rayOriginHeight = mapManager.mapConfiguration.terrainHeightSettings.maxHeight; // + raySecureOffset;
            float rayDistance = rayOriginHeight; // + raySecureOffset;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(positionCheck.ToVector3NewY(rayOriginHeight), Vector3.down, out hit, rayDistance))
            {
                if (((1 << hit.collider.gameObject.layer) & terrainGenerator.terrainLayerMask) ==
                    0) // To compare Layer with LayerMaks //Todo: add this as extensions http://answers.unity.com/answers/422476/view.html
                {
                    return false; // The hit object does not have a terrain LayerMask
                }

                float aboveSeaLevelHeight = mapManager.mapConfiguration.terrainHeightSettings.maxHeight *
                                            (1 - mapManager.mapConfiguration.seaHeightNormalized);
                float underSeaLevelHeight = mapManager.mapConfiguration.terrainHeightSettings.maxHeight *
                                            mapManager.mapConfiguration.seaHeightNormalized;
                float relativeHitHeight = Single.NegativeInfinity; // [-1,1] once calculated

                // The impact happened under the sea
                if (hit.distance > aboveSeaLevelHeight) // + raySecureOffset)
                    relativeHitHeight = -1 / underSeaLevelHeight *
                                        (hit.distance /*-raySecureOffset*/ - aboveSeaLevelHeight);

                // The impact happened above the sea
                else
                    relativeHitHeight = 1 - 1 / aboveSeaLevelHeight * hit.distance; //-raySecureOffset;

                if (relativeHitHeight > spawningHeightRange.y || relativeHitHeight < spawningHeightRange.x)
                    return false;

                spawnablePosition = hit.point;
                if (requireNavMesh)
                {
                    NavMeshHit navMeshHit;
                    if (NavMesh.SamplePosition(spawnablePosition, out navMeshHit, 1.0f, NavMesh.AllAreas))
                    {
                        spawnablePosition = navMeshHit.position;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Randomly spawns in the map a given number of MapElements.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="seed">The seed to use to generate the perlin noise</param>
        /// <param name="spawningHeightRange">In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.</param>
        /// <param name="quantity">The amount of MapElements to spawn</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <param name="requireNavMesh">Must the locations where the MapElements will spawn require a valid NavMeshSurface?</param>
        public List<MapElement> SpawnMapElementsRandomly(GameObject objectToSpawn, int seed,
            Vector2 spawningHeightRange, int quantity, Transform parent, bool requireNavMesh)
        {
            List<MapElement> spawnedMapElements = new List<MapElement>();
            int totalCountToAvoidInfiniteLoop = 5000 * quantity;
            int spawnedCount = 0;

            RandomEssentials randomEssentials = new RandomEssentials(seed);

            while (spawnedCount < quantity)
            {
                totalCountToAvoidInfiniteLoop--;
                if (totalCountToAvoidInfiniteLoop < 0)
                {
                    Debug.LogWarning(
                        $"Skipped the spawning of x{quantity - spawnedCount}/{{quantity}} {objectToSpawn.name}. No spawnable positions were found.");
                    break;
                }

                Vector2 checkPosition = randomEssentials.GetRandomVector2(-mapManager.mapConfiguration.mapRadius,
                    mapManager.mapConfiguration.mapRadius);

                if (IsSpawnablePositionOnTerrain(checkPosition, spawningHeightRange, requireNavMesh,
                    out Vector3 spawnablePosition))
                {
                    spawnedMapElements.Add(SpawnMapElement(objectToSpawn, spawnablePosition, Quaternion.identity,
                        parent));
                    spawnedCount++;
                }
            }

            return spawnedMapElements;
        }
    }

    /// <summary>
    /// Listing of all the possible creation steps that will appear at the beginning of each game
    /// </summary>
    public enum CreationStep
    {
        Terrain,
        Props,
    }
}