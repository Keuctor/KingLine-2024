using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int Id;
    public Sprite Icon;
    public string Name;
    public string Type;
    public bool Stackable;
    public int Value;
}


[CreateAssetMenu]
public class ItemsSO : ScriptableObject
{
    public ItemData[] Items;

    public ItemData GetItem(int mId)
    {
        return Items.FirstOrDefault(t => t.Id == mId);
    }
}