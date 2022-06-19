using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndOfGameOnDeath : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    private bool done = false;
    
    private void Awake()
    {
        if (healthController.onDeath == null)
            healthController.onDeath = new UnityEvent();
        healthController.onDeath.AddListener(CheckDeath);
    }

    public void CheckDeath()
    {
        if (done) return;
        //Debug.Log($"Checking death. Remaining health = {healthController.health}");
        if (healthController.health > 0) 
            return;
        GameManager.instance.GameOver();
        healthController.onDeath.RemoveListener(CheckDeath);
        done = true;
    }

    private void OnDestroy()
    {
        CheckDeath();
    }
}
