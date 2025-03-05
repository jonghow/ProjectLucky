using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
///  �� ���丮 ���� ���� ��� ��
/// </summary> 
 public class MealFactoryAdvanceCommandEntityGradeUp : MealFactoryAdvanceCommandBase
{
    public MealFactoryAdvanceCommandEntityGradeUp(int _commandID) : base(_commandID)
    {
    }

    public override void Downgrade(EntityMealFactoryController _factory)
    {
        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"Downgrade", $"�̱��� �Լ��Դϴ�.");
    }

    public override void Upgrade(EntityMealFactoryController _factory)
    {
        _factory.UpdateEntityGradeUp(_mi_CommandValue);
    }
}