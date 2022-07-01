using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeDecalVisualizationUI : MonoBehaviour
{
    [SerializeField] private GameObject decal;
    
    public void DisplayUIFor(float timeDisplayed)
    {
        CancelInvoke();
        decal.SetActive(true);
        Invoke(nameof(HideUI), timeDisplayed);
    }

    private void HideUI()
    {
        CancelInvoke();
        decal.SetActive(false);
    }
}
