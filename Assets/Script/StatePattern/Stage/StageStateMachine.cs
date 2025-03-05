using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStateMachine 
{
    private IStageState m_ICurrentState;

    public void SetState(IStageState newState)
    {
        if (m_ICurrentState != null)
            m_ICurrentState.Exit();

        m_ICurrentState = newState;
        m_ICurrentState.Enter();
    }

    public void Update()
    {
        if (m_ICurrentState != null)
            m_ICurrentState.Update();
    }

    public void GetCurrentState(out IStageState _ret) => _ret = m_ICurrentState;
}