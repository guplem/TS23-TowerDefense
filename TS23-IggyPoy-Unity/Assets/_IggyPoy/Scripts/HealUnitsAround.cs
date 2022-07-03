using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealUnitsAround : MonoBehaviour
{

    [SerializeField] private float delayBetweenHeals = 2;
    [SerializeField] private int healingPerSecondPerUnit = 10;
    [SerializeField] private float healingRadius = 10;
    [SerializeField] private GameObject healingEffectPrefab;
    [SerializeField] private AudioSource healingAudioSource;
    
    void Start()
    {
        StartCoroutine(HealingCoroutine());
    }

    private IEnumerator HealingCoroutine()
    {
        int maxColliders = 100;
        Collider[] hitColliders = new Collider[maxColliders];
        while (true)
        {
            HashSet<UnitController> unitsToHeal = new();
            int numColliders = Physics.OverlapSphereNonAlloc(this.transform.position, healingRadius, hitColliders);
            for (int i = 0; i < numColliders; i++)
            {
                Collider hitCollider = hitColliders[i];
                if (hitCollider.isTrigger)
                    continue;
                UnitController controller = hitCollider.GetComponent<UnitController>();
                if (controller != null)
                    if (controller.maxHealth > controller.health)
                        unitsToHeal.Add(controller);
            }

            if (unitsToHeal.Count > 0)
            {
                Destroy(Instantiate(healingEffectPrefab, transform.position, transform.rotation, null), 2.5f);
                healingAudioSource.Play();
                
                unitsToHeal.DebugLog(", ", "Healing " + unitsToHeal.Count + " units: ");

                foreach (UnitController unitController in unitsToHeal)
                {
                    unitController.health += Mathf.RoundToInt(healingPerSecondPerUnit * delayBetweenHeals);
                }
            }
            
            yield return new WaitForSeconds(delayBetweenHeals);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, healingRadius);
    }
}
