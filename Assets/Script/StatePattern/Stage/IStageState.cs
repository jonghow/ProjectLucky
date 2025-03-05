using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStageState
{
    void Enter();
    void Update();
    void Exit();

    void PrintState();
}