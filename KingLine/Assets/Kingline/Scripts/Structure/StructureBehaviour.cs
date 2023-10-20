using UnityEngine;

public class StructureBehaviour : MonoBehaviour
{
    public int Id;
    public string Name;
    public string Description;
    public SpriteRenderer Selection;

    private Sprite icon;

    public Sprite Icon
    {
        get => icon;
        set
        {
            icon = value;
            GetComponent<SpriteRenderer>().sprite = value;
        }
    }
}