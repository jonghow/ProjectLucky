using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
public class HandCardData
{
    [SerializeField] private int _mi_CardID;
    public int CardID { get { return _mi_CardID; } }
    [SerializeField] string _mStr_CardName;
    public string CardName { get { return _mStr_CardName; } }

    [SerializeField] HandCardType _me_UpgradeCardType;
    public HandCardType UpgradeCardType { get { return _me_UpgradeCardType; } }

    [SerializeField] string _mStr_UpgradeValue;
    public string UpgradeValue { get { return _mStr_UpgradeValue; } }
    public void UpdateHandCardID()
    {
        UpdateItemDatas();
    }
    private void UpdateItemDatas()
    {
        GameDataManager.GetInstance().GetGameDBDrawHandCard(_mi_CardID, out var _data);

        _mStr_CardName = _data.CardName;
        _mStr_UpgradeValue = _data.UpgradeCardValue.ToString();
        _me_UpgradeCardType = _data.UpgradeCardType;
    }

    #region ClassConstructor

    public HandCardData() { }
    public HandCardData(int _cardID)
    {
        _mi_CardID = _cardID;
    }

    #endregion
}
public class HandCardItem
{
    private HandCardData _m_HandCardData;
    public HandCardItem(int _cardID, long _uid)
    {
        var handCardData = new HandCardData(_cardID);
        handCardData.UpdateHandCardID();

        _ml_HandCardUID = _uid;
        this._m_HandCardData = handCardData;
    }

    long _ml_HandCardUID;
    public long GetCardUID() => _ml_HandCardUID;
    public int GetCardID() => _m_HandCardData == null ? 0 : _m_HandCardData.CardID;
    public string GetCardName() => _m_HandCardData == null ? string.Empty : _m_HandCardData.CardName;
    public string GetUpgradeValue() => _m_HandCardData == null ? string.Empty : _m_HandCardData.UpgradeValue;
    public HandCardType GetCardType() => _m_HandCardData == null ? 0 : _m_HandCardData.UpgradeCardType;
}

