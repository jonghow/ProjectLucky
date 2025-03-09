using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
public class BattleReadyState : IStageState
{
    private StageStateMachine stateMachine;

    public BattleReadyState(StageStateMachine machine)
    {
        stateMachine = machine;
    }

    public void Enter()
    {
        PrintState();
    }

    public void Update()
    {
        if (!TimerManager.GetInstance().IsComplateTime())
        {
            TimerManager.GetInstance().UpdateTime(Time.deltaTime);
        }
        else
        {
            ChangeState(new BattleState(stateMachine));
        }
    }

    private void ChangeState(IStageState _stageState)
    {
        stateMachine.SetState(_stageState);
    }

    public void Exit()
    {
        Debug.Log("📴 메인 메뉴 종료");
        SpawnerManager.GetInstance().CommandAllSpawnStart();
    }

    public void PrintState()
    {
        TimerManager.GetInstance().SetTime(Defines.DefaultInitWaitTime);

        // Rival AI ON
        RivalPlayerTurnONAI();
    }

    public void RivalPlayerTurnONAI()
    {
        UnityEngine.Debug.Log($"[BattleReadyState] RivalPlayerTurnONAI");

        RivalPlayerAIManager.GetInstance().GetRavalPlayer(out var _rival);
        //_rival.TurnOnAI();
    }
}