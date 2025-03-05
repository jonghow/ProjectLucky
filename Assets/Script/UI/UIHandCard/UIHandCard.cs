using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEditor;

public class UIHandCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] RectTransform _mRT_Transform;
    Vector3 _mv3_StartDragPosition;
    Vector3 _mv3_StartDragOffset;

    [SerializeField] HandCardItem _m_HandCardItem;
    [SerializeField] int _mi_CardID;

    [SerializeField] TextMeshProUGUI _mTxt_Name;
    [SerializeField] Image _Img_Icon;
    [SerializeField] TextMeshProUGUI _mTxt_Effect;

    public Action _mCB_CardUse;

    public void OnEnable()
    {
        _mRT_Transform = this.GetComponent<RectTransform>();
        _mv3_StartDragPosition = Vector3.zero;
        _mv3_StartDragOffset = Vector3.zero;
    }

    public void SetCard(int _cardID, long _uniqueID)
    {
        _mi_CardID = _cardID;
        GameDataManager.GetInstance().GetGameDBDrawHandCard(_mi_CardID, out var _cardData);

        _m_HandCardItem = new HandCardItem(_mi_CardID, _uniqueID);
        OnUpdateUI();
    }
    public void OnUpdateUI()
    {
        _mTxt_Name.text = _m_HandCardItem.GetCardName();
        //_Img_Icon.sprite = 
        _mTxt_Effect.text = _m_HandCardItem.GetUpgradeValue();
    }
    public void ReleaseUI()
    {
        _mCB_CardUse = null;
    }

    public void OnDisable()
    {
        ReleaseUI();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        //_mRT_Transform.position = eventData.position;
        //_mv3_StartDragPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_mRT_Transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //_mRT_Transform.localPosition = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerManager.GetInstance().SetSelectedShadow(this._m_HandCardItem);
        PlayerManager.GetInstance().SetSelectedHandCardItem(this._m_HandCardItem);
        PlayerManager.GetInstance().GetSelectedShadow().OnVisualize();

        InputManager.GetInstance().PopInputState();
        ProcPushInputState();
    }

    public void ProcPushInputState()
    {
         HandCardType _eHandCardType = _m_HandCardItem.GetCardType();

        switch (_eHandCardType)
        {
            case HandCardType.EntityCountUp:
                break;
            case HandCardType.EntityGradeUp:
                break;
            case HandCardType.ReduceCookTime:
                break;
            case HandCardType.RedeceOverHeatingTime:
                break;
            case HandCardType.BuildStruct:
                InputManager.GetInstance().PushInputState(InputState.StructureBuildState);
                break;
            case HandCardType.CookMealKit:
                InputManager.GetInstance().PushInputState(InputState.SelectCookCardState);
                // 밀키트 붓는거는 State 만들자
                break;
            case HandCardType.SpawnEntity:
                InputManager.GetInstance().PushInputState(InputState.SelectSpawnCardState);
                break;
            default:
                break;
        }
    }

    private void LogInfo()
    {
        UnityLogger.GetInstance().Log($"[UIHandCard]OnPointerClick Click!");
        UnityLogger.GetInstance().Log($"Card ID : {_m_HandCardItem.GetCardID()}");
        UnityLogger.GetInstance().Log($"Card Name : {_m_HandCardItem.GetCardName()}");
        UnityLogger.GetInstance().Log($"Card UpgradeType : {_m_HandCardItem.GetCardType().ToString()}");
        UnityLogger.GetInstance().Log($"Card Value : {_m_HandCardItem.GetUpgradeValue()}");
    }


}

