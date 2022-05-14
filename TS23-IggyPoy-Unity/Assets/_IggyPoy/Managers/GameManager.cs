using System;
using Thoughts.Game.Map;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameData gameData = new GameData(); //{ get; private set; }

    /// <summary>
    /// Should the map be automatically fully generated on entering play mode or should the creation steps (one by one) be used?
    /// </summary>
    [Tooltip("Should the map be automatically fully generated on entering play mode or should the creation steps (one by one) be used?")]
    [SerializeField] public bool fullyGenerateMapOnPlay = false;
    
    /// <summary>
    /// The map of the game.
    /// <para>A component in a GameObject</para>
    /// </summary>
    [Header("Game Elements")]
    [SerializeField] public MapManager mapManager;
    
    public GamePhase currentGamePhase {
        get => _currentGamePhase;
        set {
            if (value != _currentGamePhase)
            {
                _currentGamePhase = value;
                UIManager.instance.Refresh();
            }
        }
    }

    public GamePhase _currentGamePhase = GamePhase.Construction;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Two GameManagers are existing at the same time. Deleting the old one", this);
            Destroy(instance.gameObject);
        }

        instance = this;
    }

    private void Start()
    {
        UIManager.instance.Refresh();
        StartNewGame();
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
    }

    public void SwitchPhase()
    {
        if (currentGamePhase == GamePhase.Construction)
            StartDefensePhase();
        else
            StartConstructionPhase();
    }

    public void StartDefensePhase()
    {
        currentGamePhase = GamePhase.Defense;
    }

    public void StartConstructionPhase()
    {
        currentGamePhase = GamePhase.Construction;
    }

    public enum GamePhase
    {
        Construction,
        Defense,
    }
    
}