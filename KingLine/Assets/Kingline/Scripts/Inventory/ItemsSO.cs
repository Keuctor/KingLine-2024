using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Item
{
    public int Id;
    public Sprite Icon;
    public string Name;
}



[CreateAssetMenu]
public class ItemsSO : ScriptableObject
{
    public Item[] Items;

    public Item GetItem(int mId)
    {
        return Items.FirstOrDefault(t => t.Id == mId);
    }
}