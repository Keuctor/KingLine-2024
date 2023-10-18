using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


    [Serializable]
    public class StructureInfo
    {
        public int Id;
        public string Name;
        public string Description;
        public string EnterDescription;
        public Sprite Icon;
        [SerializeField]
        private string DefaultOptions;
        public string[] Options => DefaultOptions.Split(",");
    }
    
    [CreateAssetMenu]
    public class StructureListSO : ScriptableObject
    {
        public List<StructureInfo> Structures = new List<StructureInfo>();

        public StructureInfo GetStructureInfo(int structureId)
        {
            return Structures.FirstOrDefault(t => t.Id == structureId);
        }
    }