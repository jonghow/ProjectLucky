using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;

public static class BattleRoutine 
{
    // 고정데미지 프로그레서
    // 연산데미지 프로그레서

    /* 
     * 데미지 연산의 순서, 
     * 1. 절대 방어 ( 횟수 방어 ) 
     * 2. 쉴드 게이지 ( 쉴드 게이지 )
     * 3. 체력 
    */

    public static void OnUpdateBattleRoutine(long shooterID, long _effectorID)
    {



    }

    public static void OnHitUpdateBattleRoutine(long shooterID, long _effectorID)
    {
        Entity _shooterEntity;
        EntityManager.GetInstance().GetEntity(shooterID, out _shooterEntity);

        Entity _effectorEntity;
        EntityManager.GetInstance().GetEntity(_effectorID, out _effectorEntity);

        int _retDamage = _shooterEntity.Info.Status.STR;
        // 임시로 캐릭터의 힘 값을 평타 데미지로 준다.

        if(_effectorEntity != null)
            _effectorEntity.OnHitDamage(_retDamage);
        // 데미지를 준다.

        if(_effectorEntity.Info.IsDead() == true)
        {
            _effectorEntity.Controller?._onCB_DiedProcess?.Invoke();

            _shooterEntity.Controller.SetChaseEntity(null);
            EntityManager.GetInstance().RemoveEntity(_effectorID);
            GameObject.Destroy(_effectorEntity.gameObject);
        }
        // 죽었는지 체크한다. 죽었으면 없앤다.
    }
}

