using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;


public class InventoryManager
{
    public static InventoryManager Instance;
    public static InventoryManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = System.Activator.CreateInstance<InventoryManager>();
            //DontDestroyOnLoad(Instance);
            Instance.InitInventory();
        }

        return Instance;
    }

    public Dictionary<long, HandCardItem> _mDict_HandCardItems; // Key : UID , Value : HandCardItem
    public int _mLt_LimitCount;

    public void InitInventory()
    {
        if (_mDict_HandCardItems != null) return;
        _mDict_HandCardItems = new Dictionary<long, HandCardItem>();

        _mLt_LimitCount = 10;
    }
    public bool AddItem(long _uniqueID, HandCardItem _item)
    {
        if (_mDict_HandCardItems.Count + 1 > _mLt_LimitCount)
            return false;

        if (_mDict_HandCardItems.ContainsKey(_uniqueID) == true)
            return false;

        _mDict_HandCardItems.Add(_uniqueID, _item);
        return true;
    }
    public void RemoveItem(long _uniqueID)
    {
        if (!_mDict_HandCardItems.ContainsKey(_uniqueID))
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"RemoveItem", $"아이템의 카테고리가 존재하지 않습니다. 확인이 필요합니다.");
            return;
        }

        _mDict_HandCardItems.Remove(_uniqueID);
    }

    public void ClearHandCardItem() => _mDict_HandCardItems.Clear();
    public HandCardItem GetHandCardItem(long _uniqudID) => _mDict_HandCardItems[_uniqudID];
    public Dictionary<long, HandCardItem> GetHandCardItems() => _mDict_HandCardItems;
}