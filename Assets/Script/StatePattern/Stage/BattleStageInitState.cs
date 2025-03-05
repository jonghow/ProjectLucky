using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        OpenGuideUI();

        //Debug.Log("🏠 메인 메뉴 상태 진입");
        // UI 활성화, 버튼 이벤트 바인딩 등
    }

    public void OpenGuideUI()
    {
        var _uiDefeat = GameObject.Find($"UIPopupTutorial");
        if (_uiDefeat != null)
        {
            var _script = _uiDefeat.GetComponent<UIPopupTutorial>();
            if (_script != null)
            {
                _script.SetPopup(0);
            }
        }
    }

    public void Update()
    {
        HandCardManager.GetInstance().CommandGetCardByID(9); 
        // 식당 카드
        // 초기 세팅

        stateMachine.SetState(new BattleReadyState(stateMachine));
    }

    public void Exit()
    {
    }

    public void PrintState()
    {
        UnityLogger.GetInstance().Log($"현재 상태는 BattleStageInitState 입니다.");
    }
}