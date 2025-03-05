using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;

public static class BattleRoutine 
{
    // ���������� ���α׷���
    // ���굥���� ���α׷���

    /* 
     * ������ ������ ����, 
     * 1. ���� ��� ( Ƚ�� ��� ) 
     * 2. ���� ������ ( ���� ������ )
     * 3. ü�� 
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
        // �ӽ÷� ĳ������ �� ���� ��Ÿ �������� �ش�.

        if(_effectorEntity != null)
            _effectorEntity.OnHitDamage(_retDamage);
        // �������� �ش�.

        if(_effectorEntity.Info.IsDead() == true)
        {
            _effectorEntity.Controller?._onCB_DiedProcess?.Invoke();

            _shooterEntity.Controller.SetChaseEntity(null);
            EntityManager.GetInstance().RemoveEntity(_effectorID);
            GameObject.Destroy(_effectorEntity.gameObject);
        }
        // �׾����� üũ�Ѵ�. �׾����� ���ش�.
    }
}

