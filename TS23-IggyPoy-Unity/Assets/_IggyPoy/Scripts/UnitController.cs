using System;
using UnityEngine;

public class UnitController : HealthController
{
    [Tooltip("In seconds")]
    [SerializeField] protected float cooldown;
    [Tooltip("Damage or healing or whatever")]
    [SerializeField] protected int effect;
    
}
