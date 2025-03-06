using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using System.Linq;
using System.ComponentModel;

public class SelectCircle : MonoBehaviour
{
    [SerializeField] RectTransform _mRectTr_SelectedCircle;
    [SerializeField] Transform _m_Pivot;
    [SerializeField] Image _mImg_Range;

    [SerializeField] EntitiesGroup _m_CachedOwnerEntity;

    [SerializeField] float _manipluas;

    float _m_defaultWidth = 100f;

    public Vector3 GetPivotPos3D()
    {
        return _m_Pivot.transform.position;
    }

    public void Awake()
    {
        _manipluas = 90f;
        _mRectTr_SelectedCircle = GetComponent<RectTransform>();
        _m_Pivot.gameObject.SetActive(false); 
    }

    public Transform GetTransform() => _mRectTr_SelectedCircle;
    public void SetOwnerEntity(EntitiesGroup _entity)
    {
        _m_CachedOwnerEntity = _entity;

        if (_entity != null)
        {
            UpdateRangeScale();
            _m_Pivot.gameObject.SetActive(true);
        }
        else
        {
            ResetScale();
            _m_Pivot.gameObject.SetActive(false);
        }

        OnUpdateItem();
    }

    public void UpdateRangeScale()
    {
        var rectTr = _mImg_Range.GetComponent<RectTransform>();

        float _fRange = _m_CachedOwnerEntity.GetEntityAttackRange();

        float _newScale = (_fRange * 2)*_m_defaultWidth ; // 내 기준 중점 좌표에서부터 시작이라 상하 2배
        rectTr.sizeDelta = new Vector2(_newScale, _newScale);
    }

    public void ResetScale()
    {
        var rectTr = _mImg_Range.GetComponent<RectTransform>();
        rectTr.sizeDelta = new Vector2(_m_defaultWidth, _m_defaultWidth);
    }

    public void OnUpdateItem()
    {
    }

    private void Update()
    {
        if(_m_CachedOwnerEntity != null)
        {
            Vector3 _ownerPos = _m_CachedOwnerEntity.Pos3D;
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(_ownerPos);

            this._mRectTr_SelectedCircle.position = _screenPos;
        }
        else
        {
            this._mRectTr_SelectedCircle.localPosition = new Vector3(99999f, 99999f, 99999f);
        }
    }

    public void OnClickCombine()
    {
        UnityLogger.GetInstance().Log($"OnClickCombine");

    }

    public void OnClickTrash()
    {
        UnityLogger.GetInstance().Log($"OnClickTrash");

        int _jobID = _m_CachedOwnerEntity.ID;
        long _uid = _m_CachedOwnerEntity.UniqueID;

        EntityManager.GetInstance().NewRemoveGroup(EntityDivision.Player,_jobID, _uid);

        _m_CachedOwnerEntity = null;
        PlayerManager.GetInstance().SetSelectedEntity(null);
        SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
    }
}