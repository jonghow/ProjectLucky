using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePrepareNextWaveState : IStageState
{
    private StageStateMachine stateMachine;

    bool _isPrepareNextSpawner;

    public BattlePrepareNextWaveState(StageStateMachine machine)
    {
        stateMachine = machine;
    }

    public void Enter()
    {
        _isPrepareNextSpawner = false;
        LoadingNextSpawner();
    }

    public void LoadingNextSpawner()
    {
        SpawnerManager.GetInstance().ClearSpawner(); 

        SceneLoadManager.GetInstance().GetStage(out var _stage);

        if(_stage is BattleStage _battleStage)
        {
            int _nextStageStep = _battleStage.GetStageValue() + 1;

            UnityLogger.GetInstance().Log($"SpanwerID = {1000 + _nextStageStep}");

            _ = SpawnerManager.GetInstance().LoadSpawner(1000,_nextStageStep, () => {
                _isPrepareNextSpawner = true;
            });    
        }
    }

    public void Update()
    {
        if (!_isPrepareNextSpawner)
            return;

        ChangeState(new BattleState(stateMachine));
    }

    private void ChangeState(IStageState _stageState)
    {
        stateMachine.SetState(_stageState);
    }

    public void Exit()
    {
        SpawnerManager.GetInstance().CommandAllSpawnStart();
    }
    public void PrintState()
    {
    }
}