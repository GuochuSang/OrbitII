using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Manager;
namespace Universe 
{
public class Container : ISaveable
{
    // 问题, 添加resource
    public enum Item
    {
        WATER, MONEY,
    }

    public const int GRID_SIZE = 10;// 每格最大数量

    ID id;

    // 库存, 记录每种物品分别有多少 (通过这个可以计算出占用的格子数量)
    Dictionary<string,float> items;
    public Dictionary<string,float> Items
    {
        get
        {
            Dictionary<string,float> tempItems = new Dictionary<string, float>(items);
            return tempItems;
        }
        private set{}
    }
    // 一共有多少个格子
    int gridCount;
    public int GridCount
    {
        get{ return gridCount;}
        private set{ }
    }

    #region 新建|保存|加载|删除
    // 通过 id 加载箱子
    public Container(ID id)
    {
        this.id = id;
        id.Init();
        SaveManager.Instance.Load(this,id);
    }
    // 创建新的箱子
        public Container(string ownerName,int gridCount,out ID id)
        {
            this.gridCount = gridCount;
            this.items = new Dictionary<string, float>();

            this.id = new ID();
            this.id.className = "Container";
            this.id.sceneName = ownerName;
            this.id.idy = gridCount;
            // 如果同型号飞船建立了多个同大小的箱子, 会线性增加获得id的速度...但只要记得删除破损飞船的箱子, 应该不成问题
            while(ID.ids.Contains(this.id))
            {
                this.id.idx++;
            }
            this.id.Init();

            id = this.id;
        }
        public Container(string ownerName,int gridCount)
        {
            this.gridCount = gridCount;
            this.items = new Dictionary<string, float>();

            this.id = new ID();
            this.id.className = "Container";
            this.id.sceneName = ownerName;
            this.id.idy = gridCount;
            // 如果同型号飞船建立了多个同大小的箱子, 会线性增加获得id的速度...但只要记得删除破损飞船的箱子, 应该不成问题
            while(ID.ids.Contains(this.id))
            {
                this.id.idx++;
            }
            this.id.Init();
        }
    //保存箱子
    public ID Save()
    {
        SaveManager.Instance.Save(this, id);
        return id;
    }
    // 删除这个箱子, 移除箱子数据
    public bool Delete()
    {
        return SaveManager.Instance.RemoveIDWithData(this.id);
    }
    public static bool Delete(ID id)
    {
        if (!id.className.Equals("Container"))
            Debug.Log("You Should Not Delete "+id.className + " As A Container ! ! !");
        return SaveManager.Instance.RemoveIDWithData(id);
    }
    #endregion

    #region ISaveable
    public SaveData toSaveData()
    {
        ContainerSaveData data = new ContainerSaveData();
        data.gridCount = this.gridCount;
        data.items = this.items;
        return data;
    }
    public void fromSaveData(SaveData saveData)
    {
        ContainerSaveData data = (ContainerSaveData)saveData;
        this.gridCount = data.gridCount;
        this.items = data.items;
    }
    #endregion

    /// <summary>
    /// 返回溢出的数量
    /// </summary>

    public float AddItem(string item,float amount)
    {
        if (amount.Equals(0f))
            return 0f;
        if (!items.ContainsKey(item))
            items.Add(item, 0f);

        float amountCanAdd = 0f;
        // 剩余的空格子
        amountCanAdd += (gridCount - GetAllItemsCount())*GRID_SIZE;
        // 当前没装满的格子
            amountCanAdd += (Mathf.Ceil(items[item]/(float)GRID_SIZE))*GRID_SIZE- items[item];
            if (amountCanAdd < 0)
                Debug.Log("GridC : " + gridCount + "\n ALl Count : "+GetAllItemsCount() + "\nAmountCanAdd : "+amountCanAdd+"\nItemAmount :"+items[item] );    
        if (amountCanAdd > amount)
        {
            items[item] += amount;
            return 0f;
        }
        else
        {
            items[item] += amountCanAdd;
            return amount - amountCanAdd;
        }
    }
    public float RemoveItem(string item,float amount)
    {
        if (!items.ContainsKey(item))
            return amount;
        if (items[item] > amount)
        {
            items[item] -= amount;
            return 0f;
        }
        else
        {
            amount -= items[item];
            items.Remove(item);
            return amount;
        }
    }
    public bool ContainsItem(string item)
    {
        return items.ContainsKey(item);
    }
    // 某样物品是否拥有大于value的库存
    public bool ContainsItemValue(string item, float value)
    {
        return ContainsItem(item) && items[item] >= value;
    }

    // 所有物品当前占用了多少格子
    public int GetAllItemsCount()
    {
        int allCount = 0;
        foreach (KeyValuePair<string, float> kv in items)
        {
            allCount += GetItemCount(kv.Key);
        }
        return allCount;
    }
    // 某件物品当前占用了多少格子
    public int GetItemCount(string item)
    {
        float countf = items[item] / (float)GRID_SIZE;
        int count = Mathf.CeilToInt(countf);
        return count;
    }
}
    [System.Serializable]
public class ContainerSaveData : SaveData
{
    public int gridCount;
    public Dictionary<string,float> items;
}
}