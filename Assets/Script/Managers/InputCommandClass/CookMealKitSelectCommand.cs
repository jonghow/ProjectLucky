using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using System.Threading;

public class CookMealKitSelectCommand : InputCommandBase
{
    private Camera _m_MainCamera;
    [SerializeField] HandCardItem _m_CachedSelectedHandCardItem;

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

        _m_CachedSelectedHandCardItem = PlayerManager.GetInstance().GetSelectedHandCardItem();

        if (_m_CachedSelectedHandCardItem == null)
            return;

        CookMealKitClickCheck();
    }

    public void CookMealKitClickCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            List<Tuple<long, Entity>> _entities;
            EntityManager.GetInstance().GetEntityList(new EntityDivision[1] { EntityDivision.MealFactory }, out _entities);

            float _magDistance = 1f; // �ּ� �Ÿ�

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

            if (_m_NearEntity.Controller is EntityMealFactoryController _fController && _fController.GetMealKitInfo() != null)
            {
                UnityLogger.GetInstance().Log($"�ܿ� �̹� ������ ����!");
            }

            if (_m_NearEntity != null && _m_NearEntity.Controller is EntityMealFactoryController _factoryContoller&& _factoryContoller.GetMealKitInfo() == null)
            {
                int _cardID = _m_CachedSelectedHandCardItem.GetCardID();
                GameDataManager.GetInstance().GetGameDBDrawHandCard(_cardID, out var _ret);
                _factoryContoller.RegistMealKit((int)_ret.UpgradeCardValue);

                HandCardManager.GetInstance().UseHandCard();
                // ��� ó��
            }
            else
            {
                ReleaseState();
            }
        }
    }

    public void ReleaseState()
    {
        PlayerManager.GetInstance().ClearSelectedHandCardItem();
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.NormalState);
    }
}