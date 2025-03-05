using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;

public class HandCardManager
{
    private static HandCardManager Instance;
    public static HandCardManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new HandCardManager();
        }

        return Instance;
    }

    public Action _mCB_DrawHandCard;
    public Action _mCB_UseHandCard;

    public void CommandRerollCard()
    {
        int _count = InventoryManager.GetInstance().GetHandCardItems().Count;
        InventoryManager.GetInstance().ClearHandCardItem();

        CommandDrawCard(_count);
    }

    public void CommandDrawCard(int _drawCardCount)
    {
        if (!IsEnoughCardSpace()) return;

        List<int> _Lt_CardIndex = OnDrawRandomCardDataList(_drawCardCount);
        UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();

        // 임시
        for (int i  = 0; i < _Lt_CardIndex.Count; ++i)
        {
            var _cardDataID = _Lt_CardIndex[i];
            var uUID = uUIDGenerator.Generate();
            HandCardItem _handCardItem = new HandCardItem(_cardDataID, uUID);
            InventoryManager.GetInstance().AddItem(uUID, _handCardItem);
        }

        _mCB_DrawHandCard?.Invoke();
    }

    public bool IsEnoughCardSpace()
    {
        return InventoryManager.GetInstance().GetHandCardItems().Count < 7;
    }

    public bool EmptyCardInventory()
    {
        return InventoryManager.GetInstance().GetHandCardItems().Count <= 0;
    }

    public void CommandGetCardByID(int _cardID)
    {
        if (InventoryManager.GetInstance().GetHandCardItems().Count >= 7) return;

        UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
        var uUID = uUIDGenerator.Generate();
        HandCardItem _handCardItem = new HandCardItem(_cardID, uUID);
        InventoryManager.GetInstance().AddItem(uUID, _handCardItem);

        _mCB_DrawHandCard?.Invoke();
    }

    private List<int> OnDrawRandomCardDataList(int _drawCardCount)
    {
        List<int> _Lt_RetHandCardID = new List<int>();

        for (int i = 0; i < _drawCardCount; ++i)
        {
            //int _retIndex = OnSuffleCardData();
            int _retIndex = OnGetBuildStructType();

            _Lt_RetHandCardID.Add(_retIndex);
        }

        return _Lt_RetHandCardID;
    }

    private int OnGetBuildStructType()
    {
        // 솥과 개체카드만 줍니다.

        List<int> _Lt_Numbers = new List<int>();

        GameDataManager.GetInstance().GetGameDBDrawHandCardDatas(out var datas);

        for(int i = 0; i < 3; ++i)
        {
            foreach (var pair in datas)
            {
                var data = pair.Value;

                if(data.UpgradeCardType == GlobalGameDataSpace.HandCardType.BuildStruct)
                {
                    if(data.UpgradeCardValue == 1)
                        _Lt_Numbers.Add(data._mi_ID);
                    // 도마는 안준다.
                }

                else if (data.UpgradeCardType == GlobalGameDataSpace.HandCardType.SpawnEntity)
                    _Lt_Numbers.Add(data._mi_ID);
            }
        }

        int count = 15;
        for (int i = 0; i < count; ++i)
        {
            int prevIdx = UnityEngine.Random.Range(0, _Lt_Numbers.Count);
            int nextIdx = UnityEngine.Random.Range(0, _Lt_Numbers.Count);

            int _temp = _Lt_Numbers[prevIdx];
            _Lt_Numbers[prevIdx] = _Lt_Numbers[nextIdx];
            _Lt_Numbers[nextIdx] = _temp;
        }

        return _Lt_Numbers == null || _Lt_Numbers.Count <= 0 ? 0 : _Lt_Numbers[0];
    }

    private int OnSuffleCardData()
    {
        // 30번 셔플
        List<int> _Lt_Numbers = new List<int>();

        GameDataManager.GetInstance().GetGameDBDrawHandCardDatas(out var datas);

        foreach (var pair in datas)
        {
            var data = pair.Value;

            if (data.UpgradeCardType == GlobalGameDataSpace.HandCardType.BuildStruct ||
                data.UpgradeCardType == GlobalGameDataSpace.HandCardType.CookMealKit)
                _Lt_Numbers.Add(data._mi_ID);
        }

        int count = 30;
        for(int i = 0; i < count; ++i)
        {
            int prevIdx = UnityEngine.Random.Range(0, _Lt_Numbers.Count);
            int nextIdx = UnityEngine.Random.Range(0, _Lt_Numbers.Count);

            int _temp = _Lt_Numbers[prevIdx];
            _Lt_Numbers[prevIdx] = _Lt_Numbers[nextIdx];
            _Lt_Numbers[nextIdx] = _temp;
        }

        return _Lt_Numbers == null || _Lt_Numbers.Count <= 0 ? 0 : _Lt_Numbers[0];
    }

    public void UseHandCard()
    {
        var _item = PlayerManager.GetInstance().GetSelectedHandCardItem();
        var _uid = _item.GetCardUID();

        InventoryManager.GetInstance().RemoveItem(_uid);
        PlayerManager.GetInstance().ClearSelectedHandCardItem();

        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.NormalState);

        _mCB_UseHandCard?.Invoke();
    }
}
