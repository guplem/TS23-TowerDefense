using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthUi : MonoBehaviour
{
    [SerializeField] private HealthController structure;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private RectTransform healthBar;

    private void Awake()
    {
        HideUI();
    }

    private void OnEnable()
    {
        structure.onHealthUpdate.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        structure.onHealthUpdate.RemoveListener(UpdateUI);
    }

    private void UpdateUI()
    {
        DisplayUIFor(5);
    }
    
    private void DisplayUIFor(float timeDisplayed)
    {
        float newX = (structure.health + 0.0f) / structure.maxHealth;
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
