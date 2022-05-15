using UnityEngine;

[System.Serializable]
public class GameConfiguration
{
    [SerializeField] public SerializableDictionary<GameManager.Structure, GameObject> structures;
}