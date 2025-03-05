using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// ���丮�� ��ȭ ������ ����ʹٸ� DataVisualizer�� �ٿ��ָ� �ν����Ϳ��� Ȯ���� �� �ֽ��ϴ�.
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

