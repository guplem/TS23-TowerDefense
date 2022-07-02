using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimToTarget : MonoBehaviour
{
    [SerializeField] private AttackController attController;
    [Space]
    [SerializeField] private float speed = 400;
    [Space]
    [SerializeField] private bool lock_x = false;
    [SerializeField] private bool lock_y = false;
    [SerializeField] private bool lock_z = false;

    void Update()
    {
        if (attController == null || attController.target == null)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(attController.target.transform.position - transform.position);
        float step = speed * Time.deltaTime;
        // transform.rotation = Quaternion.Euler(new Vector3(lock_x ? 0f : targetRotation.eulerAngles.x, lock_y ? 0f : targetRotation.eulerAngles.y, lock_z ? 0f : targetRotation.eulerAngles.z));

        Quaternion targetFilteredRotation = Quaternion.Euler(new Vector3(lock_x ? 0f : targetRotation.eulerAngles.x, lock_y ? 0f : targetRotation.eulerAngles.y, lock_z ? 0f : targetRotation.eulerAngles.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetFilteredRotation, step);
    }
}
