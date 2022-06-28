using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    [SerializeField] private GameObject startSpawningButton;
    [SerializeField] private TMP_Text gameDataStringText;
    [SerializeField] private TMP_Text resourcesText;
    private ConstructionError oldReason;
    [Header("Cursors")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D distanceError;
    [SerializeField] private Texture2D resourcesError;
    [SerializeField] private Texture2D energyError;
    [SerializeField] private Texture2D generalError;
    [SerializeField] private Texture2D locationError;

    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Possible residual UIManager.", instance);
        
        instance = this;
    }

    public void FullRefresh()
    {
        startSpawningButton.SetActive(!GameManager.instance.startedEnemiesSpawning);
        gameDataStringText.text = GameManager.instance.gameDataString;
        resourcesText.text = GameManager.instance.gameData.resources.ToString();
        
    }

    
    
    private void Update()
    {
        ConstructionError reason = ConstructionController.instance.GetReasonWhyCantBeBuilt();
        if (reason == oldReason) return;
        oldReason = reason;
        switch (reason)
        {
            case ConstructionError.None:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case ConstructionError.NotSelected:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case ConstructionError.Distance:
                Cursor.SetCursor(distanceError, Vector2.zero, CursorMode.Auto);
                break;
            case ConstructionError.Resources:
                Cursor.SetCursor(resourcesError, Vector2.zero, CursorMode.Auto);
                break;
            case ConstructionError.Energy:
                Cursor.SetCursor(energyError, Vector2.zero, CursorMode.Auto);
                break;
            case ConstructionError.Other:
                Cursor.SetCursor(generalError, Vector2.zero, CursorMode.Auto);
                break;
            case ConstructionError.Location:
                Cursor.SetCursor(locationError, Vector2.zero, CursorMode.Auto);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
