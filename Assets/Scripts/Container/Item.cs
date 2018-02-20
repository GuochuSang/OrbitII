using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Universe 
{
public class Item 
{
    const string itemFolder = "Items";
    static Dictionary<string,Sprite> itemSprites = new Dictionary<string, Sprite>();

    public static Sprite GetSprite(string itemName)
    {
        if (!itemSprites.ContainsKey(itemName))
            LoadSprites(itemName);
        return itemSprites[itemName];
    }
    static void LoadSprites(string itemName)
    {
        Sprite sp = Resources.Load<Sprite>(itemFolder + "\\" + itemName);
        itemSprites.Add(itemName, sp);
    }
}
}