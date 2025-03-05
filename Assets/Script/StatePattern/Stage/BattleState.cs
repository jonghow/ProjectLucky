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

        if (SpawnerManager.GetInstance().GetIsSpawning() == true)
            return;
        // 스폰 중이라면 배틀 중

        if(IsOverEnemyCount())
        {
            ChangeState(new BattleDefeatState(stateMachine));
            // 게임에서 정해진 갯수 이상이 스폰되었다면, 
        }


        if (IsAllDeadEnemy())
        {
            ChangeState(new BattleWinState(stateMachine));
        }
        // 적군이 모두 스폰되었고 다 죽었다면 내가 이겼다.
    }
    public void Exit()
    {
        Debug.Log(" BattleState 종료");
    }
    private bool IsDestoryRestaurant()
    {

        return false;
        //EntityManager.GetInstance().GetEntityList(EntityDivision.MealFactory, out var _mealFactoryList);

        //for (int i = 0; i < _mealFactoryList.Count; i++)
        //{
            
        //}
    }
    private bool IsSpareTime()
    {
        if (_mf_spareTime >= 10)
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

    public bool IsAllDeadEnemy()
    {
        bool _ret = false;

        EntityManager.GetInstance().GetEntityList(EntityDivision.Enemy, out var _enemyList);
        return _ret = _enemyList.Count == 0 ? true : false;
    }

    public bool IsOverEnemyCount()
    {
        bool _ret = false;
        EntityManager.GetInstance().GetEntityList(EntityDivision.Enemy, out var _enemyList);

        return _ret = _enemyList.Count >= Defines.NormalSingleGameEnemyAllowCount ? true : false;
    }

    public void PrintState()
    {
        Debug.Log(" BattleState 시작");
    }
}