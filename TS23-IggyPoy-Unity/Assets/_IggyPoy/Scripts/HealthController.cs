using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : PropertyController
{
    [NonSerialized] public bool canBeDamaged = true; 
    public int health
    {
        get => _health;
        set
        {
            _health = value;
            onHealthUpdate?.Invoke();
            if (_health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private int _health = 10;

    [NonSerialized] public UnityEvent onHealthUpdate = new();
}