using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameData gameData = new GameData(); //{ get; private set; }

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