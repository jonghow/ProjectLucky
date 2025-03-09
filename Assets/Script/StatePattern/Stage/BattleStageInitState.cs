using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public class BattleStageInitState : IStageState
{
    private StageStateMachine stateMachine;

    public BattleStageInitState(StageStateMachine machine)
    {
        stateMachine = machine;
    }

    public void Enter()
    {
        PrintState();
        SoundManager.GetInstance().PlayBGM($"SoundBGM_Stage");
    }



    public void Update()
    {
        stateMachine.SetState(new BattleReadyState(stateMachine));
    }

    public void Exit()
    {
    }

    public void PrintState()
    {
        UnityLogger.GetInstance().Log($"현재 상태는 BattleStageInitState 입니다.");

        PlayerManager.GetInstance().Command_AlertBoss(1002);
    }
}