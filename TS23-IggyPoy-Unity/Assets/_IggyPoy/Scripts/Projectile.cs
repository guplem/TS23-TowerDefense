using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private HealthController target;
    private int damage;
    private PoolEssentials projectilePool;

    private Vector3 targetPosition => target.transform.position + Vector3.up * 1.5f;
    private float maxDetonationDistance => 0.01f;

    public void SetTarget(HealthController target, float speed, int damage, PoolEssentials projectilePool)
    {
        this.speed = speed;
        this.target = target;
        this.damage = damage;
        this.projectilePool = projectilePool;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPosition) < maxDetonationDistance)
        {
            target.health -= damage;
            Debug.Log($"ATTACKED {target.gameObject} with a projectile causing {damage} damage. Now {target.health} hp are still remaining.");
            projectilePool.Disable(gameObject);
        }
    }
}
