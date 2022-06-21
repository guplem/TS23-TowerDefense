using System;
using System.Collections;
using System.Collections.Generic;
using LeTai.Asset.TranslucentImage;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private GameObject structure;
    private StructureController structureController;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image image;
    
    private void Awake()
    {
        structureController = structure.GetComponentRequired<StructureController>();
        costText.text = structureController.cost.ToString();
        image.sprite = structureController.icon;
        var source = Camera.main.GetComponent<TranslucentImageSource>();
    }

    public void SelectStructureToBuild()
    {
        ConstructionController.instance.SelectStructureToBuild(structure);
    }
}
