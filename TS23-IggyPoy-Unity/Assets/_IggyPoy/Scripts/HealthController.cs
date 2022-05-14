public class HealthController : PropertyController
{
    public int health
    {
        get => _health;
        private set { _health = value; }
    }

    private int _health = 10;
}
