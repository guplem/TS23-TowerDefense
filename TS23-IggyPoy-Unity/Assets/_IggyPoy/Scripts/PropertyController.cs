using UnityEngine;

public class PropertyController : MonoBehaviour
{
    public Team team
    {
        get => _team;
        set { _team = value; }
    }

    private Team _team = Team.Neutral;

    public enum Team
    {
        Neutral,
        Player, 
        Enemy
    }
    
    
}
