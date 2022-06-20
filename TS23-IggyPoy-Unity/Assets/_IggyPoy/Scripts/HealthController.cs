using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class HealthController : PropertyController
{
    [NonSerialized] public bool canBeDamaged = true; 
    public int health
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Min(value, maxHealth);
            onHealthUpdate?.Invoke();
            if (_currentHealth <= 0)
            {
                onDeath?.Invoke();
                Destroy(this.gameObject);
            }
        }
    }
    
    [FormerlySerializedAs("_health")] [SerializeField] private int maxHealth = 10;
    private int _currentHealth = 1;
    
    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    [NonSerialized] public UnityEvent onDeath = new();
    [NonSerialized] public UnityEvent onHealthUpdate = new();

    public void FullyHealOverTime(float healingDuration)
    {
        StartCoroutine(HealingCoroutine(healingDuration));
    }

    private IEnumerator HealingCoroutine(float totalTime)
    {
        float stepInterval = 0.1f;
        float elapsed = 0;
        while (elapsed < totalTime)
        {
            health += Mathf.CeilToInt(maxHealth / (totalTime* (1/stepInterval)));
            yield return new WaitForSeconds(stepInterval);
        }
    }
}