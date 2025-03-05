using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using System.Linq;

public class SelectedArrow : MonoBehaviour
{
    [SerializeField] RectTransform _mRectTr_SelectedArrow;
    [SerializeField] Image _m_Img_Arrow;

    [SerializeField] Entity _m_CachedOwnerEntity;

    [SerializeField] float _manipluas;

    Color _mColor_Normal;
    Color _mColor_TransParent;

    public void Awake()
    {
        _manipluas = 90f;
        _mRectTr_SelectedArrow = GetComponent<RectTransform>();
        _mColor_Normal = new Color(1f, 1f, 1f, 1f);
        _mColor_TransParent = new Color(1f, 1f, 1f, 0f);
        OffAlpha();
    }

    public Transform GetTransform() => _mRectTr_SelectedArrow;
    public void SetOwnerEntity(Entity _entity)
    {
        _m_CachedOwnerEntity = _entity;

        if (_entity != null)
            OnAlpha();
        else
            OffAlpha();

        OnUpdateItem();
    }

    public void OnAlpha()
    {
        _m_Img_Arrow.color = _mColor_Normal;
    }
    public void OffAlpha()
    {
        _m_Img_Arrow.color = _mColor_TransParent;
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
}