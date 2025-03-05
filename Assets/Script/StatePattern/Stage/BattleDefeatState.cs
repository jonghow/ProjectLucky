using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDefeatState : IStageState
{
    private StageStateMachine stateMachine;

    public BattleDefeatState(StageStateMachine machine)
    {
        stateMachine = machine;
    }

    public void Enter()
    {
        PrintState();
        Debug.Log("게임에서 졌습니다.");
        OpenDefeatUI();

        // UI 활성화, 버튼 이벤트 바인딩 등
    }

    public void OpenDefeatUI()
    {
        var _uiDefeat = GameObject.Find($"UIPopupGameDefeat");
        if (_uiDefeat != null)
        {
            var _script = _uiDefeat.GetComponent<UIPopupGameDefeat>();
            if (_script != null)
            {
                _script.SetPopup();
            }
        }
    }

    public void Update()
    {
        //// 예: 플레이어가 "게임 시작" 버튼을 눌렀다면 탐험 상태로 전환
        //if (/* 버튼 입력 감지 */ true)
        //{
        //    stateMachine.SetState(new BattleState(stateMachine));
        //}
    }

    public void Exit()
    {
        Debug.Log("📴 메인 메뉴 종료");
    }
    public void PrintState()
    {
        UnityLogger.GetInstance().Log($"현재 상태는 BattleDefeatState 입니다.");
    }
}