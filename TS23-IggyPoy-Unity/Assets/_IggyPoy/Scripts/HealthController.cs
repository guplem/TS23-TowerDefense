using UnityEngine.Events;

public class HealthController : PropertyController
{
    public int health
    {
        get => _health;
        set { _health = value; }
    }

    private int _health = 10;

    public UnityEvent onHealthUpdate;
}
