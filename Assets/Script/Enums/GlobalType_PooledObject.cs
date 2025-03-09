using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PooledObject
{
    /*
     * ī�װ� ���� ������.
     * Object
     * 
     * UI
     * 
     * Effect
     * 
     * 
     * 
     */
    WO,
    NameTag,
    Effect,
}

public enum PooledObjectInner
{
    /*
     * Object Type Inner : ���� ī�װ��� �ǹ��Ѵ�. 
     */

    WO_Projectile_Arrow,
    WO_Projectile_FireBall,
    WO_OrderDisplay_Move,
    WO_OrderDisplay_Attack,

    WO_DamageTag,
    WO_CoinCountTag,
    WO_WorldHealBarTag,

    // TODO : �߻�ü�� ���� 2D Sprite�� ���Ƴ����ٵ�, �̰Ŵ� ���θ���°� ������ ���Ƴ���°� ������ üũ�غ����Ѵ�.

    Effect_FootStep,
    Effect_SkillFx,
    Effect_SkillParticle
}