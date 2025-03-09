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

    }

    public void Exit()
    {
    }
    public void PrintState()
    {
    }

    public void Update()
    {
        if (IsAllDeadEnemy())
            OpenCompleteUI();
    }

    public bool IsAllDeadEnemy()
    {
        EntityManager.GetInstance().GetEntityList(EntityDivision.Enemy, out var _Lt);

        return _Lt .Count <= 0;
    }

    public void OpenCompleteUI()
    {
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.UIOpenState);

        var _uiDefeat = GameObject.Find($"UIPopupGameWin");
        if (_uiDefeat != null)
        {
            var _script = _uiDefeat.GetComponent<UIPopupGameClear>();
            if (_script != null)
            {
                _script.SetPopup();
            }
        }
    }
}