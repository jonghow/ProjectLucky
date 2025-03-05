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
        if (IsClearMaxStage())
        {
            PlayerEntityAIStop();
            OpenClearUI();
        }
        else
        {
            _mb_IsPrepareNextStage = true;
            PrintState();

            GiveBonusCard();
            StageCountUp();
            PlayerEntityAIStop();
            HandCardManager.GetInstance().CommandDrawCard(1);
            LoadNextSpawner();
        }

        // UI 활성화, 버튼 이벤트 바인딩 등
    }

    public void OpenClearUI()
    {
        var _uiDefeat = GameObject.Find($"UIPopupGameClear");
        if (_uiDefeat != null)
        {
            var _script = _uiDefeat.GetComponent<UIPopupGameClear>();
            if (_script != null)
            {
                _script.SetPopup();
            }
        }
    }

    public bool IsClearMaxStage()
    {
        SceneLoadManager.GetInstance().GetStage(out var _stage);

        if (_stage is BattleStage _battleStage)
        {
            if (_battleStage.IsClearMaxStage())
                return true;
        }

        return false;
    }

    public void Update()
    {
        // 다음 스포너 로딩
        if(!_mb_IsPrepareNextStage)
            stateMachine.SetState(new BattleReadyState(stateMachine));
    }

    private void PlayerEntityAIStop()
    {
        List<System.Tuple<long, Entity>> _playerList;
        EntityManager.GetInstance().GetEntityList(EntityDivision.Player, out _playerList);

        for (int i = 0; i < _playerList.Count; i++)
        {
            _playerList[i].Item2.Controller.TurnOffAI();
        }
    }
    private void StageCountUp()
    {
        SceneLoadManager.GetInstance().GetStage(out var _stage);

        if (_stage is BattleStage _battleStage)
        {
            _battleStage.AddStageValue(1);
        }
    }
    private void LoadNextSpawner()
    {
        SpawnerManager.GetInstance().ClearSpawner();

        SceneLoadManager.GetInstance().GetStage(out var _stageBase);

        if (_stageBase is BattleStage _battleStage)
        {
            int _stageBaseID = 1000;
            int _stageStep = _battleStage.GetStageValue();
            UnityLogger.GetInstance().Log($"스포너 로딩 시작 ID {_stageBaseID + _stageStep}");

            _ = SpawnerManager.GetInstance().LoadSpawner(_stageBaseID , _stageStep, () => { 
                _mb_IsPrepareNextStage = false;
                UnityLogger.GetInstance().Log($"스포너 로딩 완료 ID {_stageBaseID + _stageStep}");
            });
        }
    }

    private void GiveBonusCard()
    {
        SceneLoadManager.GetInstance().GetStage(out var _stage);

        if (_stage is BattleStage _battleStage)
        {
            int _step = _battleStage.GetStageValue();

            if(_step % 5 == 0)
            {
                HandCardManager.GetInstance().CommandDrawCard(1);

                if(_step == 5)
                {
                    HandCardManager.GetInstance().CommandGetCardByID(6);
                    OpenGuideUI();
                }
            }
        }
    }

    private void OpenGuideUI()
    {
        var _uiDefeat = GameObject.Find($"UIPopupTutorial");
        if (_uiDefeat != null)
        {
            var _script = _uiDefeat.GetComponent<UIPopupTutorial>();
            if (_script != null)
            {
                _script.SetPopup(1);
            }
        }
    }

    public void Exit()
    {
        Debug.Log("📴 메인 메뉴 종료");
    }
    public void PrintState()
    {
        UnityLogger.GetInstance().Log($"현재 상태는 BattleWinState 입니다.");
    }
}