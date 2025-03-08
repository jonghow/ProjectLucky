using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GlobalGameDataSpace
{
    public enum ChrActType
    {
        None,
        Effect,
        Sound,
        Footstep,
        ShockWave,
        Damage,
        DamageHitEvent,
        SummonObject,
        BuffEvent,
        SubShape,
    }
    public enum ActEffectMoveType
    {
        None,
        FixedPlace,
        FollowCaster,
    }
    public enum EntityDivision
    {
        Player,
        Rival,
        Enemy,
        MealFactory,
        Neutrality, // 중립인데.. 공격? 일단 Player - Rival 구도
        Deco, // 맵상 오브젝트인데, Deco 정도? 혹시 모르니 수집만 해놓자.
    }

    public enum EntityGrade
    {
        None,
        Common,
        UnCommon,
        Hero,
        Myth,
        Max
    }
    public enum HandCardType
    {
        EntityCountUp,
        EntityGradeUp,
        ReduceCookTime,
        RedeceOverHeatingTime,

        BuildStruct,
        CookMealKit,
        SpawnEntity,
    }
    public enum StatType
    {
        Attack,
        Shield,
        AttackRange,
        CriticalRate,
        MoveSpeed
    }

    public enum AnimationCategory
    {
        Character,
        Rival,
        Monster,
        Factory,
    }

    public enum GridDirectionGroup { O, L, T, R, B, LT, RT, RB, LB, Max }

    public enum SoundType { BGM, SFX }
}
