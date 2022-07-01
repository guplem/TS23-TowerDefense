using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimToTarget : MonoBehaviour
{
    [SerializeField] private AttackController attController;
    [Space]
    [SerializeField] private float speed = 4;
    [Space]
    [SerializeField] private bool lock_x = false;
    [SerializeField] private bool lock_y = false;
    [SerializeField] private bool lock_z = false;

    void Update()
    {
        if (attController == null || attController.target == null)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(attController.target.transform.position - transform.position);
        //transform.LookAt(attController.target.transform);
        
        // Quaternion rotation = transform.rotation;

        // if (lock_x)
        //     rotation.x = 0;
        // if (lock_y)
        //     rotation.y = 0;
        // if (lock_z)
        //     rotation.z = 0;

        //transform.rotation = rotation;

        transform.rotation = Quaternion.Euler(new Vector3(lock_x ? 0f : targetRotation.eulerAngles.x, lock_y ? 0f : targetRotation.eulerAngles.y, lock_z ? 0f : targetRotation.eulerAngles.z));
        
        // transform.rotation = Quaternion.Euler(new Vector3(lock_x ? 0f : transform.rotation.eulerAngles.x, lock_y ? 0f : transform.rotation.eulerAngles.y, lock_z ? 0f : transform.rotation.eulerAngles.z) );
    }
}
