using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;
    
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Possible residual WorldManager.", instance);
        
        instance = this;
    }

    public void Generate()
    {
        throw new System.NotImplementedException();
    }
}
