using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public partial class PlayerManager
{
    public Action<int> _onCB_ChangeGold;
    public Action<int> _onCB_ChangeDia;
    public Action<int> _onCB_ChangeSupply;

    private int _mi_Gold; 

    public void AddFreshness(int _gold)
    {
        _mi_Gold += _gold;
        _onCB_ChangeGold?.Invoke(_mi_Gold);
    }
    public int GetFreshness() => _mi_Gold;
    public bool IsEnougnFreshness(int _freshness) => _mi_Gold >= _freshness;
    public void UseFreshness(int _useFreshness)
    {
        _mi_Gold -= _useFreshness;
        _onCB_ChangeGold?.Invoke(_mi_Gold);
    }

    private int _mi_Dia;

    public void AddDia(int _dia)
    {
        _mi_Dia += _dia;
        _onCB_ChangeDia?.Invoke(_mi_Dia);
    }
    public int GetDia() => _mi_Dia;
    public bool IsEnougnDia(int _dia) => _mi_Dia >= _dia;
    public void UseDia(int _usedia)
    {
        _mi_Dia -= _usedia;
        _onCB_ChangeDia?.Invoke(_mi_Dia);
    }

    int _mi_Supply;
    public void AddSupply(int _supply)
    {
        _mi_Supply += _supply;
        _onCB_ChangeSupply?.Invoke(_mi_Supply);
    }
    public bool EnableSupply(int _supply)
    {
        return _supply <= _mi_Supply;
    }

    public bool IsMaxSupply()
    {
        return _mi_Supply >= Defines.NormalSingleGameSupplyMaxCount;
    }

    public int GetSupply() => _mi_Supply;
    

    
}