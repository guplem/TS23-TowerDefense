using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfHealingOverTime : MonoBehaviour
{
    [SerializeField] private float timeToFullyHeal = 180;
    private HealthController healingObject;

    private void Start()
    {
        healingObject = gameObject.GetComponentRequired<HealthController>();
        StartCoroutine(HealingCoroutine());
    }
    
    private IEnumerator HealingCoroutine()
    {
        float stepInterval = 1f;
        while (true)
        {
            healingObject.health += Mathf.CeilToInt(healingObject.maxHealth / (timeToFullyHeal* (1/stepInterval)));
            yield return new WaitForSeconds(stepInterval);
        }
    }
}
