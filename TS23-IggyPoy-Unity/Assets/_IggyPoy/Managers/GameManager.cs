using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Two GameManagers are existing at the same time. Deleting the old one", this);
        }
        Destroy(instance.gameObject);
        instance = this;
    }
}
