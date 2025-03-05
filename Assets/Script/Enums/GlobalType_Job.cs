using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterJob
{
    Novice,
    SwordMan,
    Archer,
    Magician,
    Cleric,

}

public enum AbnormalType
{
    None,

    // Mez
    Stun,
    Confused,

    // Buff
    BuffAttackUp,

    // Debuff
    DebuffAttackDown,

    End
}