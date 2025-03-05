using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;

public class SelectedShadow : MonoBehaviour
{
    [SerializeField] Dictionary<GridDirectionGroup, GameObject>_m_dict_Visualizers;

    [SerializeField] GameObject _m_Visualizer_Elements;
    [SerializeField] GameObject _m_Visualizer_Layout;

    [SerializeField] Transform _mTr_SelectedShadow;

    HandCardItem _m_SelectedHandItem;

    public void Awake()
    {
        _mTr_SelectedShadow = GetComponent<Transform>();
        _m_dict_Visualizers = new Dictionary<GridDirectionGroup, GameObject>();

        SetVisualizer();

        OffVisualizers();
    }

    public void SetVisualizer()
    {
        List<Vector2> _grids = BuildGridHelper.GetBuildGrid(true);

        for(int i = 0; i < _grids.Count; ++i)
        {
            GridDirectionGroup _eGrid = (GridDirectionGroup)i;
            GameObject _newObj = GameObject.Instantiate(_m_Visualizer_Elements);

            if (!_m_dict_Visualizers.ContainsKey(_eGrid))
            {
                _newObj.SetActive(true);
                _m_dict_Visualizers.Add(_eGrid, _newObj);

                _newObj.transform.position = _grids[i];
                _newObj.transform.SetParent(_m_Visualizer_Layout.transform);
                _newObj.transform.name = $"Visualizer_{_eGrid.ToString()}";
            }
        }
    }

    public Transform GetTransform() => _mTr_SelectedShadow;
    public void SetSelectedHandItem(HandCardItem _item)
    {
        _m_SelectedHandItem = _item;

        OnVisualize();
    }

    public void OnVisualize()
    {
        if (_m_SelectedHandItem == null) return;

        OffVisualizers();
        UpgradeType();
    }

    public void UpgradeType()
    {
        HandCardType _e_UpgradeType = _m_SelectedHandItem.GetCardType();

        switch (_e_UpgradeType)
        {
            case HandCardType.EntityCountUp:
                break;
            case HandCardType.EntityGradeUp:
                break;
            case HandCardType.ReduceCookTime:
                break;
            case HandCardType.RedeceOverHeatingTime:
                break;
            case HandCardType.BuildStruct:
                OnUpdateBuildStruct();
                break;
            case HandCardType.SpawnEntity:
                OnUpdateSpawnEntity();
                break;
            default:
                break;
        }
    }

    private void OnUpdateBuildStruct()
    {
        int buildID = int.Parse(_m_SelectedHandItem.GetUpgradeValue());

        GameDataManager.GetInstance().GetGameDBBuildingInfo(buildID, out var _BuildingData);

        for (int i = 0; i < _BuildingData._meAr_BuildGrid.Length; ++i)
        {
            if (_m_dict_Visualizers.TryGetValue(_BuildingData._meAr_BuildGrid[i], out var values))
            {
                values.gameObject.SetActive(true);
            }
        }
    }

    public void OnUpdateSpawnEntity()
    {
        GridDirectionGroup _origin = GridDirectionGroup.O;

        if (_m_dict_Visualizers.TryGetValue(_origin, out var values))
        {
            values.gameObject.SetActive(true);
        }
    }

    public void OffVisualizers()
    {
        foreach (var item in _m_dict_Visualizers)
        {
            item.Value.SetActive(false);
        } 
    }
}