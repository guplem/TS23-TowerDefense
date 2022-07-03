using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    [SerializeField] private GameObject startSpawningButton;
    [SerializeField] private TMP_Text gameDataStringText;
    [SerializeField] private TMP_Text resourcesText;
    private ConstructionError oldReason;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject loadingScreen;
    [Header("Cursors")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D distanceError;
    [SerializeField] private Texture2D resourcesError;
    [SerializeField] private Texture2D energyError;
    [SerializeField] private Texture2D generalError;
    [SerializeField] private Texture2D locationError;
    [SerializeField] private TMP_Text timer;
    [Header("Sounds")]
    [SerializeField] private AudioClip openPauseMenuClip;
    [SerializeField] private AudioClip closePauseMenuClip;
    [SerializeField] private AudioClip loadingFinishedClip;

    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Possible residual UIManager.", instance);
        
        instance = this;
        pauseMenu.SetActive(false);
    }

    public void FullRefresh()
    {
        startSpawningButton.SetActive(!GameManager.instance.startedEnemiesSpawning);
        gameDataStringText.text = GameManager.instance.gameDataString;
        resourcesText.text = GameManager.instance.gameData.resources.ToString();
    }

    private void Update()
    {
        bool timerWorking = !GameManager.instance.gameOver && GameManager.instance.startedEnemiesSpawning;
        // Debug.Log($"timerWorking = {timerWorking}, gameOver = {GameManager.instance.gameOver}, startedEnemiesSpawning = {GameManager.instance.startedEnemiesSpawning}");
        timer.gameObject.SetActive(timerWorking);
        if (timerWorking)
            timer.text = GameManager.instance.timeFormatted;
        
        Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.playerControls.Player.MousePosition.ReadValue<Vector2>());

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5000, ConstructionController.instance.structureLayers))
        {
            StructureController structure = hit.collider.gameObject.GetComponent<StructureController>();
            if (structure != null && structure.healthUi != null && structure.isPlaced)
            {
                structure.healthUi.DisplayUIFor(0.1f);
            }
            if (structure != null && structure.attackController != null && structure.attackRangeDecalProjector != null && structure.isPlaced && structure.attackRangeDecalUi)
            {
                structure.attackRangeDecalUi.DisplayUIFor(0.1f);
            }
        }
        
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

    public void DisplayPauseMenu()
    {
        GameManager.instance.generalAudioSource.PlayClip(openPauseMenuClip);
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        GameManager.instance.generalAudioSource.PlayClip(closePauseMenuClip);
        pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex) ;
    }

    public void SwitchPauseMenu()
    {
        if (pauseMenu.activeSelf)
            HidePauseMenu();
        else
            DisplayPauseMenu();
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        GameManager.instance.generalAudioSource.PlayClip(loadingFinishedClip);
        loadingScreen.SetActive(false);
    }
}
