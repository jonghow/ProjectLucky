using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePauseState : IStageState
{
    private StageStateMachine stateMachine;

    public BattlePauseState(StageStateMachine machine)
    {
        stateMachine = machine;
    }

    public void Enter()
    {
        PrintState();
        Debug.Log("🏠 메인 메뉴 상태 진입");

        // 모든 AI 스톱
        // UI 활성화, 버튼 이벤트 바인딩 등
    }

    public void Update()
    {
        // 예: 플레이어가 "게임 시작" 버튼을 눌렀다면 탐험 상태로 전환
        if (/* 버튼 입력 감지 */ true)
        {
            stateMachine.SetState(new BattleDefeatState(stateMachine));
        }
    }

    public void Exit()
    {
        Debug.Log("📴 메인 메뉴 종료");
    }
    public void PrintState()
    {
        UnityLogger.GetInstance().Log($"현재 상태는 BattlePauseState 입니다.");
    }
}