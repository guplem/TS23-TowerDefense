using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateController : HealthController
{
    public abstract void SetNewState();
}
