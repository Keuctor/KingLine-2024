using UnityEngine;

public class SpriteLoader
{

    public static Sprite LoadSprite(string name)
    {
        return Resources.Load<Sprite>("ItemIcons/"+name);
    }
}