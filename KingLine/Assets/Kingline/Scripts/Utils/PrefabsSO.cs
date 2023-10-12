using UnityEngine;
using UnityEngine.Serialization;

namespace Kingline.Scripts.Utils
{
    [CreateAssetMenu]
    public class PrefabsSO : ScriptableObject
    {
        [FormerlySerializedAs("PlaceInfoUI")]
        public StructureInfoUI StructureInfoUI;
    }
}