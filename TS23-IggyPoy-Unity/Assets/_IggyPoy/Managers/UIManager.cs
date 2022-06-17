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
    
}
