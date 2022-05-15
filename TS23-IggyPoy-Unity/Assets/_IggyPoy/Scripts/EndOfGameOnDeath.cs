using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfGameOnDeath : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    
    private void Awake()
    {
        healthController.onHealthUpdate.AddListener(CheckDeath);
    }

    private void CheckDeath()
    {
        if (healthController.health >= 0) 
            return;
        GameManager.instance.GameOver();
        healthController.onHealthUpdate.RemoveListener(CheckDeath);
    }
}
