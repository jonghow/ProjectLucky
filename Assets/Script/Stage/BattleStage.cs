using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using System;

public class BattleStage : StageBase
{
    private int _mi_StageValue;
    private int _mi_MaxStageValue;
    public void AddStageValue(int _value)
    {
        _mi_StageValue += _value;
        _onCB_ChangeStage?.Invoke(_mi_StageValue);
    }
    public int GetStageValue() => _mi_StageValue;
    public bool IsClearMaxStage() => _mi_StageValue >= _mi_MaxStageValue;

    public Action<int> _onCB_ChangeStage;

    public override async void InitStage()
    {
        base.InitStage();
        _mLt_useCommand = await InputSystemInitialized.Value;
        _isLoadingComplete = true;
        // 초기 인풋 매니저 세팅

        _mi_StageValue = 0;
        _mi_MaxStageValue = 3;
        // 초기 스테이지 데이터 세팅

        _m_StageStateMachine = new StageStateMachine();
        _m_StageStateMachine.SetState(new BattleStageInitState(_m_StageStateMachine));
        // 배틀 스테이트 머신 생성 _ 초기 카드 지급
    }

    public AsyncLazy<List<InputCommandBase>> InputSystemInitialized = new AsyncLazy<List<InputCommandBase>>(async () =>
    {
        bool _isCompleteInputMgr = false;

        // 처음에 InputManager의 상태를 확인하여, 값에 따라 대기
        await UniTask.WaitUntil(() =>
        {
            _isCompleteInputMgr = InputManager.GetInstance() != null;
            return _isCompleteInputMgr; // true일 때 종료
        });

        InputManager.GetInstance().PushInputState(InputState.NormalState);
        return InputManager.GetInstance().GetNowUseInputCommandList(); // 최종적으로 _isCompleteInputMgr 값을 반환
    });

    private StageStateMachine _m_StageStateMachine;
    public void GetStageMachine(out StageStateMachine _machine) =>  _machine = _m_StageStateMachine;

    public void Update()
    {
        if (_isLoadingComplete == false)
            return;

        if(_m_StageStateMachine != null)
        {
            _m_StageStateMachine.Update();
        }

        // Check Input
        if (IsUseInputCommand())
        {
            for (int i = 0; i < _mLt_useCommand.Count; ++i)
            {
                _mLt_useCommand[i].Detect();
            }
        }

        // Check Any .. Ex) GameRule Etc..
    }
}
