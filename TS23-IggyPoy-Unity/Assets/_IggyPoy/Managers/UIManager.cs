using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TMP_Text currentGamePhase;
    [SerializeField] private GameObject constructionMenu;
    
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Possible residual UIManager.", instance);
        
        instance = this;
    }

    public void FullRefresh()
    {
        currentGamePhase.text = GameManager.instance.currentGamePhase.ToString();
        constructionMenu.SetActive(GameManager.instance.currentGamePhase == GameManager.GamePhase.Construction);
    }
}
