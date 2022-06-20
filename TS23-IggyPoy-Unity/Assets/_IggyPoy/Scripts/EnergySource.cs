using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class EnergySource : MonoBehaviour
{
    private static HashSet<EnergySource> energySources = new();
    [SerializeField] private float range = 5;
    private HashSet<StructureController> attatchedStructures = new();
    [NonSerialized] public StructureController structure;

    private void OnEnable()
    {
        energySources.Add(this);
    }

    private void OnDisable()
    {
        energySources.Remove(this);
        HashSet<StructureController> cachedAttatchedStructures = new();
        cachedAttatchedStructures.AddRange(attatchedStructures);
        foreach (StructureController structureController in cachedAttatchedStructures)
        {
            structureController.energySource = null;
        }
    }

    private void Awake()
    {
        structure = gameObject.GetComponentRequired<StructureController>();
    }

    public static EnergySource GetBestFor(Vector3 location, EnergySource exclude)
    {
        EnergySource best = null;
        float bestDistance = float.PositiveInfinity;
        foreach (EnergySource candidate in energySources)
        {
            if (candidate == exclude)
                continue;
            float candidateDistance = Vector3.Distance(location, candidate.transform.position);
            if (candidateDistance > candidate.range)
                continue;
            if (candidateDistance < bestDistance)
            {
                best = candidate;
                bestDistance = candidateDistance;
            }
        }

        return best;
    }

    public void Detach(StructureController structure)
    {
        attatchedStructures.Remove(structure);
    }

    public void Attatch(StructureController structure)
    {
        attatchedStructures.Add(structure);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}