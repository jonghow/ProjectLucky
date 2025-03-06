using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using System.Threading;

public class SpawnEntityCommand : InputCommandBase
{
    private Camera _m_MainCamera;
    [SerializeField] SelectedShadow _m_SelectedShadow;

    private NavigationElement _m_selectedNavigationElement;
    private Vector2Int _mv2_HoverGrid;

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

        if (_m_SelectedShadow == null)
            _m_SelectedShadow = PlayerManager.GetInstance().GetSelectedShadow();

        CalcNaviIndex();
        SpawnEntity();
    }

    public void CalcNaviIndex()
    {
        MapManager.GetInstance().GetNavigationElements(out var _elements);

        float _restrictSize = 1.5f; // 1f 이상의 오브젝트는 모두 Pass
        Vector2 mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 mouseScreenPos = Input.mousePosition;

        if (mouseScreenPos.x < 0 || mouseScreenPos.y < 0 ||
        mouseScreenPos.x > Screen.width || mouseScreenPos.y > Screen.height)
        {
            return; // 화면 밖이면 처리 안 함
        }

        foreach (var elementPair in _elements)
        {
            NavigationElement _element = elementPair.Value;

            if (MathUtility.CheckOverV2SqrMagnitudeDistance(mousePos, _element._mv3_Pos, _restrictSize))
                continue;

            Vector2 _point = mousePos;
            Vector2 _center = _element._mv3_Pos;
            List<Vector2> _Lt_Vertice;
            MathUtility.GetNavigationVertice(_center, out _Lt_Vertice);

            if (!MathUtility.CheckInVertice(_point, _Lt_Vertice))
                continue;

            _m_selectedNavigationElement = _element;
            MapManager.GetInstance().SelectedElement = _m_selectedNavigationElement;
            _m_SelectedShadow.transform.position = _m_selectedNavigationElement._mv3_Pos;
        }
    }

    public void SpawnEntity()
    {
        if(_m_selectedNavigationElement == null)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"SpawnEntity", $"_m_selectedNavigationElement is NULL");
            return;
        }

        if(Input.GetMouseButton(0))
        {
            if(IsEnableSpawnPlace())
            {
                //UnityLogger.GetInstance().Log($"[StructureBuildCommand]BuildStructure Success");

                var _handCard = PlayerManager.GetInstance().GetSelectedHandCardItem();
                if (_handCard.GetCardType() != HandCardType.SpawnEntity) return;

                Vector2Int _n2_NavIdx = _m_selectedNavigationElement._mv2_Index;
                Vector3 _v3_position = _m_selectedNavigationElement._mv3_Pos;

                int spawnID = int.Parse(_handCard.GetUpgradeValue());

                UserEntityFactory _spawner = new UserEntityFactory();
                _= _spawner.CreateEntity(spawnID, _v3_position, (obj) => 
                {

                });

                HandCardManager.GetInstance().UseHandCard();
                // 사용 처리

                PlayerManager.GetInstance().GetSelectedShadow().OffVisualizers();

                InputManager.GetInstance().PopInputState();
                InputManager.GetInstance().PushInputState(InputState.NormalState);
            }
            else
            {
                UnityLogger.GetInstance().Log($"설치 불가능한 위치입니다.");
            }
        }
    }

    public bool IsEnableSpawnPlace()
    {
        bool _ret = true;

        if(_m_selectedNavigationElement == null) return _ret = false;

        Vector2Int _mv2_Origin = _m_selectedNavigationElement._mv2_Index;

        var _handCard = PlayerManager.GetInstance().GetSelectedHandCardItem();
        if (_handCard.GetCardType() != HandCardType.SpawnEntity) return _ret = false;

        if (!MapManager.GetInstance().IsValidPos(_mv2_Origin))
            return _ret = false;

        if (!MapManager.GetInstance().IsEnablePos(_mv2_Origin))
            return _ret = false;

        return _ret;
    }
}