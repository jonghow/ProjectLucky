using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
///  밀 팩토리 요리 시간 감소
/// </summary> 
 public class MealFactoryAdvanceCommandReduceCookTime : MealFactoryAdvanceCommandBase
{
    public MealFactoryAdvanceCommandReduceCookTime(int _commandID) : base(_commandID)
    {
    }

    public override void Downgrade(EntityMealFactoryController _factory)
    {
        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"Downgrade", $"미구현 함수입니다.");
    }

    public override void Upgrade(EntityMealFactoryController _factory)
    {
        _factory.UpdateReduceCookTime(_mf_CommandValue);
    }
}