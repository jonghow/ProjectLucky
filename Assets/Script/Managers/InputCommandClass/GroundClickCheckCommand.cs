using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using System.Threading;

public class GroundClickCheckCommand : InputCommandBase
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

        GroundClickCheck();
    }

    public void GroundClickCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            List<Tuple<long, Entity>> _entities;
            EntityManager.GetInstance().GetEntityList(new EntityDivision[3] { EntityDivision.Player, EntityDivision.Enemy , EntityDivision.MealFactory}, out _entities);

            float _magDistance = 1f; // 최소 거리

            Entity _m_NearEntity = null;
            if (_entities != null)
            {
                foreach (var _entityPair in _entities)
                {
                    Entity _curEntity = _entityPair.Item2;
                    EntityContoller _controller = _curEntity.Controller;

                    if (Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos) <= _magDistance)
                    {
                        _m_NearEntity = _curEntity;
                        _magDistance = Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos);
                    }
                }
            }

            if (_m_NearEntity == null)
                ReleaseState();
        }
    }

    public void ReleaseState()
    {
        PlayerManager.GetInstance().ClearSelectedEntity();
        SetSelectedArrow();
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.NormalState);
    }

    public void SetSelectedArrow()
    {
        var gObj = GameObject.Find("SelectedArrow");
        var component = gObj.GetComponent<SelectedArrow>();
        component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
    }
}