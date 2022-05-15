public class HealthController : PropertyController
{
    public int health
    {
        get => _health;
        set { _health = value; }
    }

    private int _health = 10;
}
