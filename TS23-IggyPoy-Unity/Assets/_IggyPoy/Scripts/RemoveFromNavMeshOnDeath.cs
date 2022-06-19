using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RemoveFromNavMeshOnDeath : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    private bool done = false;
    [SerializeField] private GameObject navMeshColliderObject;
    
    private void Awake()
    {
        if (healthController.onDeath == null)
            healthController.onDeath = new UnityEvent();
        healthController.onDeath.AddListener(CheckDeath);
    }

    public void CheckDeath()
    {
        if (done) return;
        //Debug.LogWarning($"RegenerateNavMeshForUnits - Checking death. Remaining health = {healthController.health}.");
        if (healthController.health > 0) 
            return;
        navMeshColliderObject.SetActive(false);
        GameManager.instance.unitsSpawner.RegenerateNavMeshForUnits();
        healthController.onDeath.RemoveListener(CheckDeath);
        done = true;
    }

    private void OnDestroy()
    {
        CheckDeath();
    }
}
