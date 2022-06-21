using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private AttackController attackController;

    // Update is called once per frame
    void Update()
    {
        if (attackController.target != null)
        {
            transform.LookAt(attackController.target.transform);
        }
    }
}
