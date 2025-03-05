using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public class Entity : MonoBehaviour
{
    EntityInfo _m_Info;
    public EntityInfo Info => _m_Info;

    EntityContoller _m_Controller;
    public EntityContoller Controller => _m_Controller;

    [SerializeField] long _ml_uid;
    public long UID => _ml_uid; // UID

    [SerializeField] CharacterJob _eJobType;
    public CharacterJob JobType => _eJobType;
    public int JobID => (int)_eJobType; // JID

    [SerializeField] int _mi_entityID;
    public int CharacterID => _mi_entityID; // EID

    [SerializeField] EntityDivision Division;

    public EntityDivision _me_Division;

    public void InitEntityData(long _uniqueID, int _characterID,EntityDivision _eDivision,  EntityContoller _controller)
    {
        // NOTICE : 따로 테이블이 없는 상황이어서, JID == CID 로 한다. 
        // 추후엔 캐릭터 ID를 만들면, 그에 엮인 JOB 아이디를 가져와서 세팅할 수 있도록 한다.

        _ml_uid = _uniqueID;
        _mi_entityID = _characterID;
        _eJobType = (CharacterJob)_characterID;
        _me_Division = _eDivision;
        _m_Info = new EntityInfo(_characterID, _eDivision);
        _m_Controller = _controller;
    }

    public void InitEntityFactoryData(long _uniqueID, int _factoryID, EntityDivision _eDivision, EntityContoller _controller)
    {

    }

    private void InitializeBaseStat()
    {

    }

    public void OnHitDamage(int _damage)
    {
        int _curHp = Info.HP;

        int _calcHp = _curHp - _damage;

        Info.SetHp(_calcHp);

        OnHitDamageTag(_damage);
        //UnityLogger.GetInstance().Log($"[OnHitDamage] effectID {UID} , HP : {Info.HP}");

        Controller._onCB_HitProcess?.Invoke();
    }

    public void OnHitDamageTag(int _damage)
    {
        PoolingManager.GetInstance().GetPooledObject(PooledObject.WO, PooledObjectInner.WO_DamageTag, out var _ret);
        if(_ret is PooledObjectDamageTag _damageTag)
        {
            _damageTag.SetData(this.transform.position, _damage);
        }
    }
}
