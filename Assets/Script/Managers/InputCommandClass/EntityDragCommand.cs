using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;

public class EntityDragCommand : InputCommandBase
{
    private Camera _m_MainCamera;
    private EntityContoller _m_selectedController;
    private Vector3 _dragStartPos;

    private NavigationElement _m_selectedNavigationElement;
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

        if (Input.GetMouseButtonDown(0)) // 마우스 클릭
        {
            // 실제로 쓸 코드
            //Vector2 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            //List<Tuple<long, Entity>> _entities;
            //EntityManager.GetInstance().GetEntityList(EntityDivision.Player, out _entities);

            //float _magDistance = 1f; // 최소 거리

            //if (_entities != null)
            //{
            //    foreach(var _entityPair in _entities)
            //    {
            //        Entity _curEntity = _entityPair.Item2;
            //        EntityContoller _controller = _curEntity.Controller;

            //        if (Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos) <= _magDistance)
            //        {
            //            _m_selectedController = _controller;
            //            _magDistance = Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos);
            //            _dragStartPos = _m_selectedController._mv2_Pos;
            //        }
            //    }
            //}

            // 편의상 에디터에서 쓰는 코드
            Vector2 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            List<Tuple<long, Entity>> _entities;
            EntityManager.GetInstance().GetEntityList(new EntityDivision[2] { EntityDivision.Player, EntityDivision.Enemy } , out _entities);

            float _magDistance = 1f; // 최소 거리

            if (_entities != null)
            {
                foreach (var _entityPair in _entities)
                {
                    Entity _curEntity = _entityPair.Item2;
                    EntityContoller _controller = _curEntity.Controller;

                    if (Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos) <= _magDistance)
                    {
                        _m_selectedController = _controller;
                        _magDistance = Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos);
                        _dragStartPos = _m_selectedController._mv2_Pos;
                    }
                }
            }
        }

        if (Input.GetMouseButton(0) && _m_selectedController != null) // 드래그 중
        {
            FollowObject();
            CheckOntheNavigation();
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 떼면 드래그 해제
        {
            ReleaseOntheNavigation();
            ReleaseSelectObject();
        }
    }

    public void FollowObject()
    {
        Vector2 mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
        _m_selectedController.transform.position = mousePos;
    }

    public void CheckOntheNavigation()
    {
        MapManager.GetInstance().GetNavigationElements(out var _elements);

        float _restrictSize = 1.5f; // 1f 이상의 오브젝트는 모두 Pass

        foreach (var elementPair in _elements)
        {
            NavigationElement _element = elementPair.Value;

            if (MathUtility.CheckOverV2SqrMagnitudeDistance(_m_selectedController.transform.position, _element._mv3_Pos, _restrictSize))
                continue;

            Vector2 _point = _m_selectedController._mv3_Pos;
            Vector2 _center = _element._mv3_Pos;
            List<Vector2> _Lt_Vertice;
            MathUtility.GetNavigationVertice(_center, out _Lt_Vertice);

            if (!MathUtility.CheckInVertice(_point, _Lt_Vertice))
                continue;

            _m_selectedNavigationElement = _element;
            MapManager.GetInstance().SelectedElement = _m_selectedNavigationElement;
        }
    }

    public void ReleaseSelectObject()
    {
        _m_selectedController = null;
    }
    public void ReleaseOntheNavigation()
    {
        if (_m_selectedController == null)
            return;
        else if ( _m_selectedNavigationElement == null)
        {
            _m_selectedController.transform.position = _dragStartPos;
            return;
        }
        else
        {
            _m_selectedController.transform.position = _m_selectedNavigationElement._mv3_Pos;

            _m_selectedController.GetMoveAgent(out var _moveAgent);
            _moveAgent.GetPathFinder(out var _entityPathFinder);
            _entityPathFinder.SetNavigationElement(_m_selectedNavigationElement);
            _moveAgent.SetStartPoint(_entityPathFinder.GetCurrentIndex());
        }
    }
}