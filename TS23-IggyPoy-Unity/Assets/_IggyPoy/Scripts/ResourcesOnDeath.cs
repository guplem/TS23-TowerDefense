using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourcesOnDeath : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private int resourcesDelta;
    private bool done = false;

    private void Awake()
    {
        if (healthController.onDeath == null)
            healthController.onDeath = new UnityEvent();
        healthController.onDeath.AddListener(AddResources);
    }

    public void AddResources()
    {
        if (done) return;
        //Debug.Log($"Checking death. Remaining health = {healthController.health}");
        if (healthController.health > 0) 
            return;
        GameManager.instance.gameData.resources += resourcesDelta;
        healthController.onDeath.RemoveListener(AddResources);
        done = true;
    }

    private void OnDestroy()
    {
        AddResources();
    }
}
