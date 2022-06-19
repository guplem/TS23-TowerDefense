using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RecalculateNavMeshOnDeath : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    private bool done = false;
    
    private void Awake()
    {
        if (healthController.onHealthUpdate == null)
            healthController.onHealthUpdate = new UnityEvent();
        healthController.onHealthUpdate.AddListener(CheckDeath);
    }

    public void CheckDeath()
    {
        if (done) return;
        Debug.LogWarning($"RegenerateNavMeshForUnits - Checking death. Remaining health = {healthController.health}.");
        if (healthController.health > 0) 
            return;
        GameManager.instance.unitsSpawner.RegenerateNavMeshForUnits();
        healthController.onHealthUpdate.RemoveListener(CheckDeath);
        done = true;
    }

    private void OnDestroy()
    {
        CheckDeath();
    }
}
