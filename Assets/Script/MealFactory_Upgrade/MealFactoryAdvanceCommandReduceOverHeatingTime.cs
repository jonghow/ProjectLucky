using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
///  �� ���丮 ���� ���� �ð� ����
/// </summary> 
 public class MealFactoryAdvanceCommandReduceOverHeatingTime : MealFactoryAdvanceCommandBase
{
    public MealFactoryAdvanceCommandReduceOverHeatingTime(int _commandID) : base(_commandID)
    {
    }

    public override void Downgrade(EntityMealFactoryController _factory)
    {
        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"Downgrade", $"�̱��� �Լ��Դϴ�.");
    }

    public override void Upgrade(EntityMealFactoryController _factory)
    {
        _factory.UpdateReduceOverHeatingTime(_mf_CommandValue);
    }
}