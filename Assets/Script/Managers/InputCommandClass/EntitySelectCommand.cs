using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;

/// <summary>
/// Mercenary Entity 를 선택하는 커맨드입니다.
/// </summary>
public class EntitySelectCommand : InputCommandBase
{
    private Camera _m_MainCamera;
    public override void Initialize()
    {
        _m_MainCamera = Camera.main;
    }

    public override void Detect()
    {
        if(_m_MainCamera == null)
        {
            _m_MainCamera = Camera.main;
        }

        if (Input.GetMouseButtonDown(0)) // 마우스 클릭
        {
            // 편의상 에디터에서 쓰는 코드
            Vector2 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            List<Tuple<long, Entity>> _entities;
            EntityManager.GetInstance().GetEntityList(new EntityDivision[3] { EntityDivision.Player, EntityDivision.Enemy , EntityDivision.MealFactory} , out _entities);

            float _magDistance = 0.5f; // 최소 거리

            if (_entities != null)
            {
                foreach (var _entityPair in _entities)
                {
                    Entity _curEntity = _entityPair.Item2;
                    EntityContoller _controller = _curEntity.Controller;

                    if (Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos) <= _magDistance)
                    {
                        PlayerManager.GetInstance().SetSelectedEntity(_curEntity);
                        _magDistance = Vector2.SqrMagnitude(_controller._mv2_Pos - _mousePos);
                    }
                }
            }

            // 
            if (PlayerManager.GetInstance().GetSelectedEntity() != null)
            {
                if (PlayerManager.GetInstance().GetSelectedEntity()._me_Division == EntityDivision.Player ||
                    PlayerManager.GetInstance().GetSelectedEntity()._me_Division == EntityDivision.Enemy)
                {
                    SetSelectedArrow();
                    ChangeInputStateToSelectEntity();
                }
                else if (PlayerManager.GetInstance().GetSelectedEntity()._me_Division == EntityDivision.MealFactory)
                {
                    SetSelectedArrow();
                    ChangeInputStateToSelectStructure();
                }
                else { }
            }
        }
    }

    public void ChangeInputStateToSelectEntity()
    {
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.SelectEntityState);
    }

    public void ChangeInputStateToSelectStructure()
    {
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.SelectStructureState);
    }

    public void SetSelectedArrow()
    {
        var gObj = GameObject.Find("SelectedArrow");
        var component = gObj.GetComponent<SelectedArrow>();
        component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
    }
}