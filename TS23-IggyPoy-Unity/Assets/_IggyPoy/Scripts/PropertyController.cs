using UnityEngine;

public class PropertyController : MonoBehaviour
{
    public int team
    {
        get => _team;
        private set { _team = value; }
    }

    private int _team = -1;
}
