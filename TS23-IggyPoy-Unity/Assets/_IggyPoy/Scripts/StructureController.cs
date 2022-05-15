using UnityEngine;

public class StructureController : HealthController
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject blueprint;

    public void ShowBlueprint()
    {
        visuals.SetActive(false);
        blueprint.SetActive(true);
    }
    
    public void ShowVisuals()
    {
        visuals.SetActive(true);
        blueprint.SetActive(false);
    }
}
