using System;
using System.Collections;
using System.Linq;
using Thoughts.Game.Map;
using UnityEngine;
using Console = System.Console;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameData gameData;
    public GameConfiguration gameConfiguration;

    public UnitsSpawner unitsSpawner
    {
        get
        {
            if (_unitsSpawner == null)
                _unitsSpawner = mapManager.gameObject.GetComponentInChildren<UnitsSpawner>();
            return _unitsSpawner;
        }
    }

    public String gameDataString => "This game is cool";

    private UnitsSpawner _unitsSpawner = null;

    /// <summary>
    /// The map of the game.
    /// <para>A component in a GameObject</para>
    /// </summary>
    [Header("Game Elements")] [SerializeField]
    public MapManager mapManager;

    /// <summary>
    /// Should the map be automatically fully generated on entering play mode or should the creation steps (one by one) be used?
    /// </summary>
    [Tooltip(
        "Should the map be automatically fully generated on entering play mode or should the creation steps (one by one) be used?")]
    [SerializeField]
    public bool fullyGenerateMapOnPlay = false;

    [SerializeField] private float spawnClockInterval = 5f; // In seconds
    private bool gameOver = false;
    [NonSerialized] public bool startedEnemiesSpawning = true;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Two GameManagers are existing at the same time. Deleting the old one", this);
            Destroy(instance.gameObject);
        }

        instance = this;

        _unitsSpawner = mapManager.gameObject.GetComponentInChildren<UnitsSpawner>();
        //constructionController = this.gameObject.GetComponentRequired<ConstructionController>();
    }

    private void Start()
    {
        StartNewGame();
        UIManager.instance.FullRefresh();
    }

    /// <summary>
    /// Starts a new game by deleting the previously generated world.
    /// </summary>
    public void StartNewGame()
    {
        // Delete the previously generated world
        if (!fullyGenerateMapOnPlay)
            mapManager.DeleteMap();
        else
            mapManager.RegenerateFullMap();

        gameOver = false;
        startedEnemiesSpawning = false;
        StartCoroutine(UnitsSpawnClock());
    }


    public void GameOver()
    {
        if (gameOver) Debug.LogError("Game over already set!");
        Debug.LogWarning(" ====== GAME OVER ====== ");
        gameOver = true;
    }

    public void StartSpawning()
    {
        GameManager.instance.startedEnemiesSpawning = true;
        UIManager.instance.FullRefresh();
    }
    
    IEnumerator UnitsSpawnClock()
    {
        while (!gameOver)
        {
            if (startedEnemiesSpawning)
            {
                // Debug.Log("Spawning enemies");
                unitsSpawner.SpawnUnits(false);
            }

            yield return new WaitForSeconds(spawnClockInterval);
            gameData.timeSinceSpawnStarted += spawnClockInterval;
        }
        Debug.Log("Stopping enemies spawn clock");
    }
}