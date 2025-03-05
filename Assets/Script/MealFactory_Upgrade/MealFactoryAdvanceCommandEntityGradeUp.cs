using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
///  밀 팩토리 생산 라인 등급 업
/// </summary> 
 public class MealFactoryAdvanceCommandEntityGradeUp : MealFactoryAdvanceCommandBase
{
    public MealFactoryAdvanceCommandEntityGradeUp(int _commandID) : base(_commandID)
    {
    }

    public override void Downgrade(EntityMealFactoryController _factory)
    {
        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"Downgrade", $"미구현 함수입니다.");
    }

    public override void Upgrade(EntityMealFactoryController _factory)
    {
        _factory.UpdateEntityGradeUp(_mi_CommandValue);
    }
}