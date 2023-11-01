using UnityEngine;
using UnityEngine.Serialization;

public class StructureBehaviour : MonoBehaviour
{
    public int Id;
    public string Name;
    public string Description;
    public SpriteRenderer Selection;

    public TargetStructureView TargetStructureView;
    
    private Sprite icon;

    public Sprite Icon
    {
        get
        {
            if (icon == null)
            {
                icon = GetComponent<SpriteRenderer>().sprite;
            }
            return icon;
        }
        set
        {
            icon = value;
            GetComponent<SpriteRenderer>().sprite = value;
        }
    }
}