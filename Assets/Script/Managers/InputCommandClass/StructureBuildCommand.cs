using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using System.Threading;

public class StructureBuildCommand : InputCommandBase
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
        BuildStructure();
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

    public void BuildStructure()
    {
        if(_m_selectedNavigationElement == null)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"BuildStructure", $"_m_selectedNavigationElement is NULL");
            return;
        }

        if(Input.GetMouseButton(0))
        {
            if(IsEnableBuildPlace())
            {
                //UnityLogger.GetInstance().Log($"[StructureBuildCommand]BuildStructure Success");

                var _handCard = PlayerManager.GetInstance().GetSelectedHandCardItem();
                if (_handCard.GetCardType() != HandCardType.BuildStruct) return;

                ApplyBuildSpaceNav();
                // 설치한 부분의 Navigation을 갱신한다.

                Vector2Int _n2_NavIdx = _m_selectedNavigationElement._mv2_Index;
                Vector3 _v3_position = _m_selectedNavigationElement._mv3_Pos;

                int buildID = int.Parse(_handCard.GetUpgradeValue());

                if(buildID == 3) // 식당
                {
                    var spawner = new StoreFactoryEntityFactory();
                    _ = spawner.CreateEntity(buildID, _v3_position, (_createEntity) => 
                    {
                        _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                        _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                    });
                }
                else if (buildID == 2)
                {
                    var spawner = new CombineFactoryEntityFactory();
                    _ = spawner.CreateEntity(buildID, _v3_position, (_createEntity) =>
                    {
                        _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                        _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                    });
                }
                else
                {
                    var spawner = new MealFactoryEntityFactory();
                    _ = spawner.CreateEntity(buildID, _v3_position, (_createEntity) =>
                    {
                        _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                        _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                    });
                }

                // 사용 처리
                HandCardManager.GetInstance().UseHandCard();
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

    public bool IsEnableBuildPlace()
    {
        bool _ret = true;

        if(_m_selectedNavigationElement == null) return _ret = false;

        Vector2Int _mv2_Origin = _m_selectedNavigationElement._mv2_Index;

        var _handCard = PlayerManager.GetInstance().GetSelectedHandCardItem();
        if (_handCard.GetCardType() != HandCardType.BuildStruct) return _ret = false;

        int buildID = int.Parse(_handCard.GetUpgradeValue());
        GameDataManager.GetInstance().GetGameDBBuildingInfo(buildID, out var _buildingData);

        GridDirectionGroup[] _buildDirectionGroup = _buildingData._meAr_BuildGrid;

        for(int i = 0; i < _buildDirectionGroup.Length; ++i)
        {
            Vector2Int _dirConvert = BuildGridHelper.ConvertDirToNavIndex(_buildDirectionGroup[i]);
            Vector2Int _calcedIndex = _mv2_Origin + _dirConvert;

            if(!MapManager.GetInstance().IsValidPos(_calcedIndex))
            {
                _ret = false;
                break;
            }

            if(!MapManager.GetInstance().IsEnablePos(_calcedIndex))
            {
                _ret = false;
                break;
            }
        }

        return _ret;
    }

    public void ApplyBuildSpaceNav()
    {
        Vector2Int _mv2_Origin = _m_selectedNavigationElement._mv2_Index;

        var _handCard = PlayerManager.GetInstance().GetSelectedHandCardItem();
        if (_handCard.GetCardType() != HandCardType.BuildStruct) return;

        int buildID = int.Parse(_handCard.GetUpgradeValue());
        GameDataManager.GetInstance().GetGameDBBuildingInfo(buildID, out var _buildingData);

        GridDirectionGroup[] _buildDirectionGroup = _buildingData._meAr_BuildGrid;

        MapManager.GetInstance().SetDisableNavigationElement(_mv2_Origin, _buildDirectionGroup);
    }
}