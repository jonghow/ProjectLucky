using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class PooledObjectWorldHealBarTag : PooledBase, IPoolBase
{
    [SerializeField] RectTransform _mRectTr_SelectedArrow;
    [SerializeField] Entity _m_CachedOwnerEntity;

    private float _mf_maxHp;
    private float _mf_CurHp;

    private float _mf_HPRate => (_mf_CurHp / _mf_maxHp);
    private float _mf_HPRatePercent => (_mf_CurHp / _mf_maxHp) * 100f;

    [SerializeField] Image _m_Img_HpBar;
    [SerializeField] TextMeshProUGUI _m_Img_HpValue;
    public Gradient healthGradient; // 체력에 따른 그라데이션
    public void SetHP()
    {
        Regist();

        _mf_CurHp = _m_CachedOwnerEntity.Info.HP;
        _mf_maxHp = _m_CachedOwnerEntity.Info.MaxHP;

        UIManager.GetInstance().GetWOCanvas(out var _parent);
        this.gameObject.transform.SetParent(_parent.transform);

        _m_Img_HpValue.text = $"{(int)_mf_CurHp}"; // 소수 한자리수 
        OnUpdateHpBar();
    }
    private void OnUpdateHpBar()
    {
        _m_Img_HpBar.fillAmount = _mf_HPRate;
        _m_Img_HpBar.color = healthGradient.Evaluate(_mf_HPRate);
    }

    public void Awake()
    {
        _mRectTr_SelectedArrow = GetComponent<RectTransform>();
    }
    public Transform GetTransform() => _mRectTr_SelectedArrow;
    public void SetOwnerEntity(Entity _entity)
    {
        _m_CachedOwnerEntity = _entity;

        _m_CachedOwnerEntity.Controller._onCB_HitProcess -= SetHP;
        _m_CachedOwnerEntity.Controller._onCB_HitProcess += SetHP;

        OnUpdateItem();
    }
    public void OnUpdateItem()
    {
        //_m_SpriteRenderer.sprite = 
    }
    private void Update()
    {
        if(_m_CachedOwnerEntity != null)
        {
            Vector3 _ownerPos = _m_CachedOwnerEntity.Controller.Pos3D;

            Transform _arrowPoint = _m_CachedOwnerEntity.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "HeadArrow");
            Vector3 _pointPos = _arrowPoint.transform.position;
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(_pointPos);
            //_screenPos.y += _manipluas;

            this._mRectTr_SelectedArrow.position = _screenPos;
        }
        else
        {
            this._mRectTr_SelectedArrow.localPosition = new Vector3(99999f, 99999f, 99999f);
        }
    }
    public void Regist()
    {
        this.gameObject.SetActive(true);

        _me_PooledType = PooledObject.WO;
        _me_PooledInnerType = PooledObjectInner.WO_WorldHealBarTag;
    }
    public void Release()
    {
        _m_CachedOwnerEntity.Controller._onCB_HitProcess -= SetHP;

        var _IPooledBase = this as IPoolBase;
        PoolingManager.GetInstance().CollectObject(_me_PooledType, _me_PooledInnerType, ref _IPooledBase);
    }
}