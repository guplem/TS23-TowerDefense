using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager instance;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

}
