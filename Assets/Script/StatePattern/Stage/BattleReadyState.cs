using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        //Debug.Log("🏠 메인 메뉴 상태 진입");
        // UI 활성화, 버튼 이벤트 바인딩 등
    }

    public void Update()
    {
    }

    public void Exit()
    {
        Debug.Log("📴 메인 메뉴 종료");
    }

    public void PrintState()
    {
        UnityLogger.GetInstance().Log($"현재 상태는 BattleReadyState 입니다.");
    }
}