using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 팩토리의 강화 정보를 보고싶다면 DataVisualizer를 붙여주면 인스펙터에서 확인할 수 있습니다.
/// </summary>
public class MealFactoryVisualizer : MonoBehaviour
{
    [SerializeField] MealFactoryData _m_FactoryOriginData;
    [SerializeField] MealFactoryData _m_FactoryAdvancedData;

    public void OnEnable()
    {
        var _EntityMealFactoryController = this.GetComponent<EntityMealFactoryController>();
        _m_FactoryOriginData = _EntityMealFactoryController.GetOriginMealFactoryData();
        _m_FactoryAdvancedData = _EntityMealFactoryController.GetAdvanceMealFactoryData();
    }

    public void OnDisable()
    {
        _m_FactoryOriginData = null;
        _m_FactoryAdvancedData = null;
    }
}

