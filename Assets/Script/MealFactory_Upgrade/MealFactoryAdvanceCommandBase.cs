using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

public interface IMealFactoryAdvanceCommand
{
    public abstract void Upgrade(EntityMealFactoryController _factory);
    public abstract void Downgrade(EntityMealFactoryController _factory);
}

public class MealFactoryAdvanceCommandBase : IMealFactoryAdvanceCommand
{
    protected string _mStr_CommandValue;

    protected int _mi_CommandValue;
    protected float _mf_CommandValue;

    public MealFactoryAdvanceCommandBase(int _commandID)
    {
        _mStr_CommandValue = $"1";
        _mi_CommandValue = Int32.Parse(_mStr_CommandValue);
        _mf_CommandValue = float.Parse(_mStr_CommandValue);
    }

    public virtual void Upgrade(EntityMealFactoryController _factory) { }
    public virtual void Downgrade(EntityMealFactoryController _factory) { }
}

