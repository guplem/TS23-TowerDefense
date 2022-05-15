using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndOfGameOnDeath : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    
    private void Awake()
    {
        if (healthController.onHealthUpdate == null)
            healthController.onHealthUpdate = new UnityEvent();
        healthController.onHealthUpdate.AddListener(CheckDeath);
    }

    public void CheckDeath()
    {
        if (healthController.health >= 0) 
            return;
        GameManager.instance.GameOver();
        healthController.onHealthUpdate.RemoveListener(CheckDeath);
    }
}
