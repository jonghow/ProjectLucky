using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PooledObject
{
    /*
     * 카테고리 별로 나눈다.
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
     * Object Type Inner : 세부 카테고리를 의미한다. 
     */

    WO_Projectile_Arrow,
    WO_Projectile_FireBall,
    WO_OrderDisplay_Move,
    WO_OrderDisplay_Attack,

    WO_DamageTag,
    WO_CoinCountTag,
    WO_WorldHealBarTag,

    // TODO : 발사체는 추후 2D Sprite를 갈아끼울텐데, 이거는 새로만드는게 빠른지 갈아끼우는게 빠른지 체크해봐야한다.

    Effect_FootStep,
    Effect_SkillFx,
    Effect_SkillParticle
}