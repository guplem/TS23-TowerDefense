using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarHelperUI : MonoBehaviour
{
    private float maxDistance = 110;
    private float sizeAtMaxDistance = 1.5f;
    private float minDistance = 33;

    [SerializeField] private MonoBehaviour[] toDisable;
    [SerializeField] private GameObject[] toSetInactive;
    [SerializeField] private RectTransform toResize;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        float distance = Vector3.Distance(cam.transform.position, this.transform.position);
        if (distance > minDistance)
        {
            foreach (MonoBehaviour component in toDisable) component.enabled = true;
            foreach (GameObject go in toSetInactive) go.SetActive(true);
            float currentSize = (distance/maxDistance) * sizeAtMaxDistance;
            toResize.SetWidth(currentSize);
            toResize.SetHeight(currentSize);
        }
        else
        {
            foreach (MonoBehaviour component in toDisable) component.enabled = false;
            foreach (GameObject go in toSetInactive) go.SetActive(false);
        }
    }
}
