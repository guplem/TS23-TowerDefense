using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTimeIfNotCloseToCenter : MonoBehaviour
{
    [SerializeField] private float time = 30;
    [SerializeField] private float minDistToCenterToKillRelativeToRadius = 0.6f;
    private float distToKill => GameManager.instance.mapManager.mapConfiguration.mapRadius * minDistToCenterToKillRelativeToRadius;
    
    void Start()
    {
        Invoke(nameof(CustomDestroy), time);
    }

    void CustomDestroy()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > distToKill)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (GameManager.instance != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(Vector3.zero, distToKill);   
        }
    }
}
