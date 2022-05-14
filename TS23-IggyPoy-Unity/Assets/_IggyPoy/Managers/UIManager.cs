using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TMP_Text currentGamePhase;
    
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Possible residual UIManager.", instance);
        
        instance = this;
    }

    public void Refresh()
    {
        currentGamePhase.text = GameManager.instance.currentGamePhase.ToString();
    }
}
