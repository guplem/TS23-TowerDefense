using System;
using UnityEngine;

public class UnitController : HealthController
{
    [Tooltip("In seconds")]
    [SerializeField] protected float cooldown = 1;
    [Tooltip("Damage or healing or whatever")]
    [SerializeField] protected int effect = 1;
    
}
