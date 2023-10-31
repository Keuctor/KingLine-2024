using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu]
public class PrefabsSO : ScriptableObject
{
    [FormerlySerializedAs("StructureInfoUI")]
    public TargetStructureView TargetStructureView;

    public ItemSelectPopup ItemSelectPopup;
    
    public CharacterView CharacterView;
}