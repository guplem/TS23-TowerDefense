using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;

public class EnergySource : MonoBehaviour
{
    private static HashSet<EnergySource> energySources = new();
    [SerializeField] private float range = 5;
    private HashSet<StructureController> attatchedStructures = new();
    [NonSerialized] public StructureController structure;
    [SerializeField] private DecalProjector energySourceRangeDecalProjector;
    [SerializeField] private Material constructionDecalMaterial;
    [SerializeField] private Material builtDecalMaterial;

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

    static public EnergySource closesToBlueprintStructure = null;
    
    private void Update()
    {
        if (ConstructionController.instance.hasSelectedStructureToBuild)
        {
            if (ConstructionController.instance.placeHolderBuilding != structure && structure.constructionTime <= 0)
                if (closesToBlueprintStructure == null || Vector3.Distance(closesToBlueprintStructure.transform.position, ConstructionController.instance.placeHolderBuilding.transform.position) > Vector3.Distance(transform.position, ConstructionController.instance.placeHolderBuilding.transform.position))
                    closesToBlueprintStructure = this;
            
            energySourceRangeDecalProjector.material = structure.constructionTime <= 0 ? builtDecalMaterial : constructionDecalMaterial;
            energySourceRangeDecalProjector.gameObject.SetActive(closesToBlueprintStructure == this || structure.constructionTime > 0);
            energySourceRangeDecalProjector.size = new Vector3(range*2, range*2, 50);
        }
        else
        {
            energySourceRangeDecalProjector.gameObject.SetActive(false);
        }
    }

    public static EnergySource GetBestFor(Vector3 location, EnergySource exclude)
    {
        EnergySource best = null;
        try
        {
            float bestDistance = float.PositiveInfinity;
            foreach (EnergySource candidate in energySources)
            {
                if (candidate == exclude)
                    continue;
                float candidateDistance = Vector3.Distance(location, candidate.transform.position);
                if (candidateDistance > candidate.range)
                    continue;
                if (candidateDistance < bestDistance && candidate.structure.constructionTime <= 0)
                {
                    best = candidate;
                    bestDistance = candidateDistance;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error trying to find the best energy source in the location {location}. {e.Message}\n{e.StackTrace}");
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