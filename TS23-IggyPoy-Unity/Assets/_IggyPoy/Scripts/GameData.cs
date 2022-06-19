[System.Serializable]
public class GameData
{
    public int resources
    {
        get => _resources;
        set { _resources = value; UIManager.instance.FullRefresh(); } // TODO: Change for a refresh only of the needed section
    }
    private int _resources = 750;
}
