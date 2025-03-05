using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public partial class PlayerManager
{
    public Action<int> _onCB_ChangeFreshNess;

    private int _mi_Freshness; 

    public void AddFreshness(int _freshNess)
    {
        _mi_Freshness += _freshNess;
        _onCB_ChangeFreshNess?.Invoke(_mi_Freshness);
    }
    public int GetFreshness() => _mi_Freshness;
    public bool IsEnougnFreshness(int _freshness) => _mi_Freshness >= _freshness;
    public void UseFreshness(int _useFreshness)
    {
        _mi_Freshness -= _useFreshness;
        _onCB_ChangeFreshNess?.Invoke(_mi_Freshness);
    }
}