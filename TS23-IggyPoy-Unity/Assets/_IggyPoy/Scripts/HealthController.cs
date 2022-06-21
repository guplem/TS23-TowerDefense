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
            int newHealth = Mathf.Min(value, maxHealth);
            if (_currentHealth == newHealth)
                return;

            _currentHealth = newHealth;
            onHealthUpdate?.Invoke();
            if (_currentHealth <= 0)
            {
                onDeath?.Invoke();
                Destroy(this.gameObject);
            }
        }
    }
    
    [FormerlySerializedAs("_health")] [SerializeField]
    public int maxHealth = 10;
    private int _currentHealth = 1;

    protected void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    [NonSerialized] public UnityEvent onDeath = new();
    [NonSerialized] public UnityEvent onHealthUpdate = new();

    #region Check To Know if this gameObject has the component "EnergySource"

    public bool isEnergySource
    {
        get
        {
            if (_isEnergySource == HasSelfEnergySource.unknown)
            {
                EnergySource found = gameObject.GetComponent<EnergySource>();
                if (found == null)
                    _isEnergySource = HasSelfEnergySource.no;
                else
                    _isEnergySource = HasSelfEnergySource.yes;
            }

            return _isEnergySource == HasSelfEnergySource.yes;
        }
    }
    private HasSelfEnergySource _isEnergySource = HasSelfEnergySource.unknown;
    private enum HasSelfEnergySource
    {
        unknown, 
        yes,
        no
    }

    #endregion

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
            elapsed += stepInterval;
            yield return new WaitForSeconds(stepInterval);
        }
    }
}