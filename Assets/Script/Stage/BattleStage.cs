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
        // �ʱ� ��ǲ �Ŵ��� ����

        _mi_StageValue = 0;
        _mi_MaxStageValue = 3;
        // �ʱ� �������� ������ ����

        _m_StageStateMachine = new StageStateMachine();
        _m_StageStateMachine.SetState(new BattleStageInitState(_m_StageStateMachine));
        // ��Ʋ ������Ʈ �ӽ� ���� _ �ʱ� ī�� ����
    }

    public AsyncLazy<List<InputCommandBase>> InputSystemInitialized = new AsyncLazy<List<InputCommandBase>>(async () =>
    {
        bool _isCompleteInputMgr = false;

        // ó���� InputManager�� ���¸� Ȯ���Ͽ�, ���� ���� ���
        await UniTask.WaitUntil(() =>
        {
            _isCompleteInputMgr = InputManager.GetInstance() != null;
            return _isCompleteInputMgr; // true�� �� ����
        });

        InputManager.GetInstance().PushInputState(InputState.NormalState);
        return InputManager.GetInstance().GetNowUseInputCommandList(); // ���������� _isCompleteInputMgr ���� ��ȯ
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
