using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

[Serializable]
public class MealFactoryData 
{
    private HandCardItem _m_HandCradItem;

    private float _mf_NeedCookTime;
    private float _mf_AccCookTime;

    private float _mf_ReduceOverHeatingTime; // ���� �ð� ���ҷ�
    private float _mf_MaxOverHeatingTime; // �ִ� ���� �ð� ���ҷ�

    private float _mf_ReduceCookTime; // �丮 �ð� ���ҷ�
    private float _mf_MaxReduceCookTime; // �ִ� �丮 �ð� ���ҷ�

    private int _mi_LimitCount;     // ���� ���ѷ� 
    private int _mi_MaxLimitCount;      // �ִ� ���� ��  

    public void UpdateEntityLimitCount(int _count)
    {
        if (!CheckEnableUpdateEntityCount(_count))
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UpdateEntityLimitCount", $"�ִ� ���귮 ����ġ���� ����, ���׷��̵带 �����߽��ϴ�.");
            return;
        }

        _mi_LimitCount = Mathf.Clamp(_mi_LimitCount + _count, 0, _mi_MaxLimitCount);
    }
    private bool CheckEnableUpdateEntityCount(int _count) => _mi_LimitCount + _count <= _mi_MaxLimitCount;
    public int GetEntityCreateLimitCount() => _mi_LimitCount;
    public void UpdateReducePercentCookTime(float _reducePercent)
    {
        if (!CheckEnableUpdateReducePercentCookTime(_reducePercent))
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UpdateEntityLimitCount", $"�ִ� ���귮 ����ġ���� ����, ���׷��̵带 �����߽��ϴ�.");
            return;
        }

        _mf_ReduceCookTime = Mathf.Clamp(_mf_ReduceCookTime + _reducePercent, 0, _mf_MaxReduceCookTime);
    }
    private bool CheckEnableUpdateReducePercentCookTime(float _reducePercent) => _mf_ReduceCookTime + _reducePercent <= _mf_MaxReduceCookTime;
    public void UpdateReducePercentOverheatingTime(float _reducePercent)
    {
        if (!CheckEnableUpdateReducePercentOverheatingTime(_reducePercent))
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UpdateEntityLimitCount", $"�ִ� ���귮 ����ġ���� ����, ���׷��̵带 �����߽��ϴ�.");
            return;
        }

        _mf_ReduceOverHeatingTime = Mathf.Clamp(_mf_ReduceOverHeatingTime + _reducePercent, 0, _mf_MaxOverHeatingTime);
    }
    private bool CheckEnableUpdateReducePercentOverheatingTime(float _reducePercent) => _mf_ReduceOverHeatingTime + _reducePercent <= _mf_ReduceOverHeatingTime;

    public HandCardItem GetHandCardItem() => _m_HandCradItem;
}

