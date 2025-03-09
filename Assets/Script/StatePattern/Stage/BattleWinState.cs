using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public class BattleWinState : IStageState
{
    private StageStateMachine stateMachine;

    private bool _mb_IsPrepareNextStage;

    public BattleWinState(StageStateMachine machine)
    {
        stateMachine = machine;
    }

    public void Enter()
    {
        OpenCompleteUI();
    }

    public void Exit()
    {
    }
    public void PrintState()
    {
    }

    public void Update()
    {
    }

    public void OpenCompleteUI()
    {
        var _uiDefeat = GameObject.Find($"UIPopupGameWin");
        if (_uiDefeat != null)
        {
            var _script = _uiDefeat.GetComponent<UIPopupGameDefeat>();
            if (_script != null)
            {
                _script.SetPopup();
            }
        }
    }
}