using UnityEngine;

[SelectionBase]
public class MapElement : MonoBehaviour
{
    [SerializeField] private string Name = "No name";
    [TextArea]
    [SerializeField]
    private string Description = "No description";
}