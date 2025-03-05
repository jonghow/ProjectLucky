using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

[System.Serializable]
public class EntityStatus 
{
    public EntityStatus(int _objectID, EntityDivision _eDivision)
    {
        switch (_eDivision)
        {
            case EntityDivision.Player:
            case EntityDivision.Enemy:
                SetMercenaryStatus(_objectID);
                break;
            case EntityDivision.MealFactory:
                SetStructureStatus(_objectID);
                break;
            default:
                break;
        }
    }

    private void SetMercenaryStatus(int _objectID)
    {
        GameDataManager.GetInstance().GetGameDBCharacterInfo(_objectID, out GameDB_CharacterInfo _ret);
        GameDataManager.GetInstance().GetGameDBCharacterStat(_ret._mi_CharacterStatSet, out GameDB_CharacterStat _retStat);

        _strength = _retStat._mi_StatStr;
        _dexterity = _retStat._mi_StatDex;
        _wisdom = _retStat._mi_StatWis;
        _guts = _retStat._mi_StatGuts;
        _mental = _retStat._mi_StatMen;

        // 2차 스탯
        _physicalAtk = 10f;
        _magicalAtk = 10f;

        _hitRate = 10f;
        _evasionRate = 10f;

        _physicalCriticalAtkRate = 10f;
        _magicalCriticalAtkRate = 10f;

        _physicalCriticalAtkDamageInc = 10f;
        _magicalCriticalAtkDamageInc = 10f;

        _buffEfficiency = 10f;

        _physicalDef = 10f;
        _magicalDef = 10f;

        _physicalCriticalAtkBlockRate = 10f;
        _magicalCriticalAtkBlockRate = 10f;

        _physicalCriticalAtkDamageDec = 10f;
        _magicalCriticalAtkDamageDec = 10f;
    }

    private void SetStructureStatus(int _objectID)
    {
        GameDataManager.GetInstance().GetGameDBBuildingInfo(_objectID, out GameDB_BuildingInfo _ret);
        GameDataManager.GetInstance().GetGameDBBuildingStat(_ret._mi_StatSet, out GameDB_BuildingStat _retStat);

        _strength = _retStat._mi_StatStr;
        _dexterity = _retStat._mi_StatDex;
        _wisdom = _retStat._mi_StatWis;
        _guts = _retStat._mi_StatGuts;
        _mental = _retStat._mi_StatMen;

        // 2차 스탯
        _physicalAtk = 10f;
        _magicalAtk = 10f;

        _hitRate = 10f;
        _evasionRate = 10f;

        _physicalCriticalAtkRate = 10f;
        _magicalCriticalAtkRate = 10f;

        _physicalCriticalAtkDamageInc = 10f;
        _magicalCriticalAtkDamageInc = 10f;

        _buffEfficiency = 10f;

        _physicalDef = 10f;
        _magicalDef = 10f;

        _physicalCriticalAtkBlockRate = 10f;
        _magicalCriticalAtkBlockRate = 10f;

        _physicalCriticalAtkDamageDec = 10f;
        _magicalCriticalAtkDamageDec = 10f;
    }

    public void CalculateStatus()
    {
        Debug.Log($"Proc __ CalculateStatus");
    }

    /*
     * 1차 스탯
     * 힘
     * 민첩
     * 지능
     * 근성
     * 정신
     */

    private int _strength;
    private int _dexterity;
    private int _wisdom;
    private int _guts;
    private int _mental;
    public int STR { set { _strength = value; } get { return _strength; } }
    public int DEX { set { _dexterity = value; } get { return _dexterity; } }
    public int WIS { set { _wisdom = value; } get { return _wisdom; } }
    public int GUT { set { _guts = value; } get { return _guts; } }
    public int MET { set { _mental = value; } get { return _mental; } }

    /* 
     * 
     * 2차 스탯 
     * 물리 공격
     * 물리 방어력
     * 물리 치명타 확률 
     * 물리 치명타 회피율
     * 물리 치명타 데미지 증가율 
     * 물리 치명타 데미지 감소율
     * 
     * 마법 공격
     * 마법 방어력
     * 마법 치명타 확률
     * 마법 치명타 회피율 
     * 마법 치명타 데미지 증가율
     * 마법 치명타 데미지 감소율
     * 
     * 적중률
     * 회피율
     * 
     * 버프 효율 ( 버프량에 대한 효율을 올리거나, 지속 시간 등을 올릴 수 있도록 한다.)
     * 버프 저항 ( 버프에 걸릴 확률을 감소시킨다 )
     * 
     */
    private double _physicalAtk;    
    private double _magicalAtk;

    private double _hitRate;
    private double _evasionRate;

    private double _physicalCriticalAtkRate;
    private double _magicalCriticalAtkRate;

    private double _physicalCriticalAtkDamageInc;
    private double _magicalCriticalAtkDamageInc;

    private double _buffEfficiency;

    private double _physicalDef;
    private double _magicalDef;

    private double _physicalCriticalAtkBlockRate;
    private double _magicalCriticalAtkBlockRate;

    private double _physicalCriticalAtkDamageDec;
    private double _magicalCriticalAtkDamageDec;
}
