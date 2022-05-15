using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager instance { get; private set; }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

}
