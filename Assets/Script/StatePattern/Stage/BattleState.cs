using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public class BattleState : IStageState
{
    private StageStateMachine stateMachine;

    public float _mf_spareTime; // 여유시간
    public int _mi_gameDefeatEnemyCount;

    public BattleState(StageStateMachine machine)
    {
        stateMachine = machine;
    }

    public void Enter()
    {
        PrintState();
        // UI 활성화, 버튼 이벤트 바인딩 등
    }
    public void Update()
    {
        if (IsSpareTime())
            return;

        if(IsClearMaxStage())
        {
            if(SpawnerManager.GetInstance().GetIsSpawning() == false)
            {
                ChangeState(new BattleWinState(stateMachine));
                return;
            }
        }
        else
        {
            if (SpawnerManager.GetInstance().GetIsSpawning() == false)
            {
                ChangeState(new BattlePrepareNextWaveState(stateMachine));
                return;
            }
        }

        if(IsOverEnemyCount())
        {
            // 게임에서 정해진 갯수 이상이 스폰되었다면, 
            ChangeState(new BattleDefeatState(stateMachine));
            return;
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
    public void Exit()
    {
        Debug.Log(" BattleState 종료");
    }
    private bool IsSpareTime()
    {
        if (_mf_spareTime >= 1)
        {
            return false;
        }
        else
        {
            _mf_spareTime += Time.deltaTime;
            return true;
        }
    }
    private void ChangeState(IStageState _stageState)
    {
        stateMachine.SetState(_stageState);
    }
    public bool IsOverEnemyCount()
    {
        bool _ret = false;
        EntityManager.GetInstance().GetEntityList(EntityDivision.Enemy, out var _enemyList);

        return _ret = _enemyList.Count >= Defines.NormalSingleGameEnemyAllowCount ? true : false;
    }
    public void PrintState()
    {
        TimerManager.GetInstance().SetTime(Defines.DefaultStageIntervalWaveTime);
        PlayerManager.GetInstance().Command_AlertBoss(1002);
        StageCountUp();
    }

    public void StageCountUp()
    {
        SceneLoadManager.GetInstance().GetStage(out var _stage);

        if (_stage is BattleStage _battleStage)
        {
            _battleStage.AddStageValue(1);
        }
    }
}