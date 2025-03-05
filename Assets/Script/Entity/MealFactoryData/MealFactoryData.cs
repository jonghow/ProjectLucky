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

    private float _mf_ReduceOverHeatingTime; // 과열 시간 감소량
    private float _mf_MaxOverHeatingTime; // 최대 가열 시간 감소량

    private float _mf_ReduceCookTime; // 요리 시간 감소량
    private float _mf_MaxReduceCookTime; // 최대 요리 시간 감소량

    private int _mi_LimitCount;     // 생산 제한량 
    private int _mi_MaxLimitCount;      // 최대 생산 값  

    public void UpdateEntityLimitCount(int _count)
    {
        if (!CheckEnableUpdateEntityCount(_count))
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UpdateEntityLimitCount", $"최대 생산량 증가치보다 많아, 업그레이드를 실패했습니다.");
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
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UpdateEntityLimitCount", $"최대 생산량 증가치보다 많아, 업그레이드를 실패했습니다.");
            return;
        }

        _mf_ReduceCookTime = Mathf.Clamp(_mf_ReduceCookTime + _reducePercent, 0, _mf_MaxReduceCookTime);
    }
    private bool CheckEnableUpdateReducePercentCookTime(float _reducePercent) => _mf_ReduceCookTime + _reducePercent <= _mf_MaxReduceCookTime;
    public void UpdateReducePercentOverheatingTime(float _reducePercent)
    {
        if (!CheckEnableUpdateReducePercentOverheatingTime(_reducePercent))
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UpdateEntityLimitCount", $"최대 생산량 증가치보다 많아, 업그레이드를 실패했습니다.");
            return;
        }

        _mf_ReduceOverHeatingTime = Mathf.Clamp(_mf_ReduceOverHeatingTime + _reducePercent, 0, _mf_MaxOverHeatingTime);
    }
    private bool CheckEnableUpdateReducePercentOverheatingTime(float _reducePercent) => _mf_ReduceOverHeatingTime + _reducePercent <= _mf_ReduceOverHeatingTime;

    public HandCardItem GetHandCardItem() => _m_HandCradItem;
}

