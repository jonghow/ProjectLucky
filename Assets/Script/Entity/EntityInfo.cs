using GlobalGameDataSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityInfo
{
    public EntityInfo(int _objectID, EntityDivision _eDivision)
    // NOTICE : 나중에는 오브젝트 ID로 스탯을 가져오는 구조로 하는것이 맞음.
    {
        switch (_eDivision)
        {
            case EntityDivision.Player:
            case EntityDivision.Enemy:
            case EntityDivision.Rival:
                SetEntityMercenaryStatInfo(_objectID, _eDivision);
                break;
            case EntityDivision.MealFactory:
                SetEntityStructureStatInfo(_objectID, _eDivision);
                break;
            default:
                break;
        }
    }

    private void SetEntityMercenaryStatInfo(int _objectID, EntityDivision _eDivision)
    {
        GameDataManager.GetInstance().GetGameDBCharacterInfo(_objectID, out GameDB_CharacterInfo _ret);
        GameDataManager.GetInstance().GetGameDBCharacterStat(_ret._mi_CharacterStatSet, out GameDB_CharacterStat _retStat);

        _mi_Maxhp = _retStat._mi_StatMaxHp;
        _mi_hp = _retStat._mi_StatMaxHp;

        _mi_mp = 100;
        _mi_Maxmp = _mi_mp;

        _moveSpeed = _retStat._mi_StatDex * 0.5f;

        _attackRange = _retStat._mf_AttackRange; // 일단 고정,추후 테이블 연동
        _detectRange = _retStat._mf_AttackRange * 2f ; // 일단 고정, 추후 아이템이나 버프로만 증가하도록(일단, 원거리 캐릭 한정)

        _status = new EntityStatus(_objectID, _eDivision);
    }
    private void SetEntityStructureStatInfo(int _objectID, EntityDivision _eDivision)
    {
        GameDataManager.GetInstance().GetGameDBBuildingInfo(_objectID, out GameDB_BuildingInfo _ret);
        GameDataManager.GetInstance().GetGameDBBuildingStat(_ret._mi_StatSet, out GameDB_BuildingStat _retStat);

        _mi_Maxhp = _retStat._mi_StatMaxHp;
        _mi_hp = _retStat._mi_StatMaxHp;

        _mi_mp = 100;
        _mi_Maxmp = _mi_mp;

        _moveSpeed = _retStat._mi_StatDex * 0.5f;

        _attackRange = _retStat._mf_AttackRange; // 일단 고정,추후 테이블 연동
        _detectRange = _retStat._mf_AttackRange * 1.3f; // 일단 고정, 추후 아이템이나 버프로만 증가하도록(일단, 원거리 캐릭 한정)

        _status = new EntityStatus(_objectID, _eDivision);
    }

    public void SetHp(int _hp)
    {
        this._mi_hp = _hp;
    }

    public bool IsDead() => this._mi_hp <= 0;

    private int _mi_hp;
    private int _mi_Maxhp;
    public int HP { set { _mi_hp = value; } get { return _mi_hp; } }
    public int MaxHP { get { return _mi_Maxhp; } }
    public float HPPercent { get { return (_mi_hp / _mi_Maxhp) * 100f; } }
    // Hp

    private int _mi_mp;
    private int _mi_Maxmp;
    public int MP { get { return _mi_mp; } }
    public int MaxMP { get { return _mi_Maxmp; } }
    public float MPPercent { get { return (_mi_mp / _mi_Maxmp) * 100f; } }
    // Mp

    private float _moveSpeed;
    private float _detectRange;
    private float _attackRange;
    public float MoveSpeed => _moveSpeed;
    public float DetectRange => _detectRange;
    public float AttackRange => _attackRange;

    private EntityStatus _status;
    public EntityStatus Status { get { return _status; } }
}
