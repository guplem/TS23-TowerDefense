using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExclusionArea : MonoBehaviour
{

    public bool hasExclusionAreaFree
    {
        get
        {
            try
            {
                if (structuresInExclusionArea.Count <= 0)
                    return true;
                foreach (StructureController structure in structuresInExclusionArea)
                {
                    if (structure == null)
                        return false;
                    
                    if (Vector3.Distance(structure.transform.position, self.transform.position) < exclusionCollider.radius)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public HashSet<StructureController> structuresInExclusionArea = new();
    [SerializeField] private StructureController self;
    [SerializeField] private SphereCollider exclusionCollider;

    private void OnTriggerEnter(Collider other)
    {
        StructureController structure = other.GetComponent<StructureController>();
        if (structure != null && structure != self)
        {
            structuresInExclusionArea.Add(structure);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StructureController structure = other.GetComponent<StructureController>();
        if (structure != null && structure != self)
        {
            structuresInExclusionArea.Remove(structure);
        }
    }
}
