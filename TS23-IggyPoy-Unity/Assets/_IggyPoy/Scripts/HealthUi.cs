using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class HealthUi : MonoBehaviour
{
    [FormerlySerializedAs("structure")] [SerializeField] private HealthController hpController;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private RectTransform healthBar;

    private void Awake()
    {
        HideUI();
    }

    private void OnEnable()
    {
        hpController.onHealthUpdate.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        hpController.onHealthUpdate.RemoveListener(UpdateUI);
    }

    private void UpdateUI()
    {
        DisplayUIFor(4);
    }
    
    public void DisplayUIFor(float timeDisplayed)
    {
        float newX = (hpController.health + 0.0f) / hpController.maxHealth;
        // Debug.Log("UPDATING HEALTH UI TO: " + newX + " / " + structure.gameObject.name, this);
        CancelInvoke();
        healthUI.SetActive(true);
        healthBar.anchorMax = new Vector2(newX, 1);
        // healthBar.SetAnchors(new MinMax01(0, 0, newX, 1));
        Invoke(nameof(HideUI), timeDisplayed);
    }

    private void HideUI()
    {
        CancelInvoke();
        healthUI.SetActive(false);
    }
}
