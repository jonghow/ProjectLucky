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
        Neutrality, // �߸��ε�.. ����? �ϴ� Player - Rival ����
        Deco, // �ʻ� ������Ʈ�ε�, Deco ����? Ȥ�� �𸣴� ������ �س���.
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
