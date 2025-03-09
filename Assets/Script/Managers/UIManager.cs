using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    Transition,
    TouchBlock,
    Max
}

public class UIManager
{
    public static UIManager Instance;

    public static UIManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new UIManager();
        }

        return Instance;
    }

    private Dictionary<UIType, List<GameObject>> dicUIObject = new Dictionary<UIType, List<GameObject>>();
    public void OpenUI(UIType type, GameObject obj)
    {
        if(dicUIObject.ContainsKey(type))
        {

        }
    }

    public GameObject _m_Canvas;
    public GameObject _m_WOCanvas;
    public GameObject _m_HUDCanvas;

    public GameObject _m_CoinCountPivot;

    public void GetCanvas(out GameObject _ret)
    {
        if(_m_Canvas == null)
            _m_Canvas = GameObject.Find("Canvas");

        _ret = _m_Canvas;
    }
    public void GetWOCanvas(out GameObject _ret)
    {
        if(_m_WOCanvas == null)
        {
            GetCanvas(out var _parent);
            _m_WOCanvas = _parent.transform.Find($"WO").gameObject;
        }

        _ret = _m_WOCanvas;
    }

    public void GetHUDCanvas(out GameObject _ret)
    {
        if (_m_HUDCanvas == null)
        {
            GetCanvas(out var _parent);
            _m_HUDCanvas = _parent.transform.Find($"HUD").gameObject;
        }

        _ret = _m_HUDCanvas;
    }

    public void GetCoinCountPivot(out GameObject _ret)
    {
        if(_m_CoinCountPivot == null)
        {
            GetHUDCanvas(out var _parent);

            GameObject _obj= _parent.transform.Find($"UIBattleStageHUD(Clone)").gameObject;
            UIBattleStageHUD _StageHud = _obj.GetComponent<UIBattleStageHUD>();
            _m_CoinCountPivot = _StageHud._mObj_CoinCountPivot;
        }

        _ret = _m_CoinCountPivot;
    }
}