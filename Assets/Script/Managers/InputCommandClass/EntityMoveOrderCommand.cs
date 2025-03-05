using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using System.Threading;

public class EntityMoveOrderCommand : InputCommandBase
{
    private Camera _m_MainCamera;
    [SerializeField] Entity _m_CachedSelectedEntity;

    public override void Initialize()
    {
        _m_MainCamera = Camera.main;
    }

    public override void Detect()
    {
        if (_m_MainCamera == null)
        {
            _m_MainCamera = Camera.main;
        }

        _m_CachedSelectedEntity = PlayerManager.GetInstance().GetSelectedEntity();

        if (_m_CachedSelectedEntity == null)
            return;

        Order();
    }

    public void Order()
    {
        if (Input.GetMouseButtonDown(1) && IsPlayerEntity())
        {
            Vector2 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int _NavigationIndex = GetAboveNavigationElement();

            _m_CachedSelectedEntity.Controller.GetMoveAgent(out var _moveAgent);
            _moveAgent.CommandMove(_NavigationIndex);

            ShowWorldPing(_mousePos);
        }
    }

    public Vector2Int GetAboveNavigationElement()
    {
        MapManager.GetInstance().GetNavigationElements(out var _elements);

        float _restrictSize = 1.5f; // 1f 이상의 오브젝트는 모두 Pass

        Vector2 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);

        foreach (var elementPair in _elements)
        {
            NavigationElement _element = elementPair.Value;

            if (MathUtility.CheckOverV2SqrMagnitudeDistance(_mousePos, _element._mv3_Pos, _restrictSize))
                continue;

            Vector2 _point = _mousePos;
            Vector2 _center = _element._mv3_Pos;
            List<Vector2> _Lt_Vertice;
            MathUtility.GetNavigationVertice(_center, out _Lt_Vertice);

            if (!MathUtility.CheckInVertice(_point, _Lt_Vertice))
                continue;

            return _element._mv2_Index;
        }

        return Vector2Int.zero;
    }

    public bool IsPlayerEntity()
    {
        bool _ret = false;

        if(_m_CachedSelectedEntity != null)
            _ret = _m_CachedSelectedEntity.Controller is EntityUserContoller;

        return _ret;
    }

    public void ShowWorldPing(Vector2 _pos)
    {
        PoolingManager.GetInstance().GetPooledObject(PooledObject.WO, PooledObjectInner.WO_OrderDisplay_Move, out var _ret);
        var _pooledObject = _ret as PooledObjectOrderDisplay;
        _pooledObject.SetData(_pos);
    }
}