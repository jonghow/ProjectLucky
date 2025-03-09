using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;


public class RivalPlayerAIManager
{
    public static RivalPlayerAIManager Instance;
    public static RivalPlayerAIManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new RivalPlayerAIManager();
        }

        return Instance;
    }

    int _mi_Gold;
    public int _mi_Dia;

    int _mi_Supply;

    RivalPlayerAI _RivalPlayer;

    public RivalPlayerAIManager()
    {
        _mi_Gold = 250;
        _mi_Dia = 0;
        _mi_Supply = 0;
    }

    public void AddGold(int _gold)
    {
        _mi_Gold += _gold;
    }
    public bool EnableUseGold(int _gold)
    {
        return _gold <= _mi_Gold;
    }
    public void UseGold(int _gold)
    {
        _mi_Gold -= _gold;
    }
    public void AddDia(int _gold)
    {
        _mi_Dia += _gold;
    }
    public bool EnableUseDia(int _gold)
    {
        return _gold <= _mi_Dia;
    }
    public void UseDia(int _gold)
    {
        _mi_Dia -= _gold;
    }
    public void AddSupply(int _gold)
    {
        _mi_Supply += _gold;
    }
    public bool EnableSupply(int _gold)
    {
        return _gold <= _mi_Supply;
    }
    public int GetSupply() => _mi_Supply;

    public bool IsMaxSupply()
    {
        return _mi_Supply >= Defines.NormalSingleGameSupplyMaxCount;
    }

    public void GetRavalPlayer(out RivalPlayerAI _ret)
    {
        if(_RivalPlayer == null)
        {
            GameObject _obj = GameObject.Find("RivalPlayerAI");
            _RivalPlayer = _obj.GetComponent<RivalPlayerAI>();
        }
        _ret = _RivalPlayer;
    }


}