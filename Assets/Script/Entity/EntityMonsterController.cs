using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;

public class EntityMonsterController : EntityContoller
{
    // 월드 모델은 어디서 관리?
    private Avatar _avatar; // 공용화된 모델 베이스
    private EntityBehaviorTreeBase _behaviorTree;

    // AnimatoController

    // Controller BuffSystem
    private AbnormalSystem _abnormalSystem;

    public List<Vector3> _mLt_ProbePosition; // 몬스터가 정찰할 포지션 대략 4개면 된다. 얘는 A* 로 움직일것도아니고 그냥 무빙시킬거

    public bool _m_IsKillMine = true; // 내가 잡았는지 라이벌이 잡았는지

    public IPoolBase _m_worldHaelBarTag; // HealBatTag

    public override void SetUp(long _entityUID, int _entityID)
    {
        _ml_EntityUID = _entityUID;
        _mi_EntityTID = _entityID;
        //TurnOffAI(); // 초기는 끈다.
        TurnOnAI(); // 몬스터는 스포너에서만 생길거라,ON 하고 태어난다.
        TransformSetUp();

        _m_ActPlayer.SetOwnerUID(_entityUID, _entityID);
        AbnormalSystemSetUp();
    }

    public void SetPlayerZoneProbeList()
    {
        if (_mLt_ProbePosition == null)
        {
            _mLt_ProbePosition = new List<Vector3>();
            _mLt_ProbePosition.Add(new Vector3(-3.8f, -3.2f, 0f));
            _mLt_ProbePosition.Add(new Vector3(-3.8f, 0.6f, 0f));
            _mLt_ProbePosition.Add(new Vector3(3.65f, 0.6f, 0f));
            _mLt_ProbePosition.Add(new Vector3(3.65f, -3.2f, 0f));
        }
    }

    public void SetRivalZoneProbeList()
    {
        if (_mLt_ProbePosition == null)
        {
            _mLt_ProbePosition = new List<Vector3>();
            _mLt_ProbePosition.Add(new Vector3(-3.8f, 5.6f, 0f));
            _mLt_ProbePosition.Add(new Vector3(-3.8f, 0.6f, 0f));
            _mLt_ProbePosition.Add(new Vector3(3.65f, 0.6f, 0f));
            _mLt_ProbePosition.Add(new Vector3(3.65f, 5.6f, 0f));
        }
    }

    public void AISetUp()
    {
        _behaviorTree = new MonsterBehaviorMeleeNormalType($"Monster", 1, this);
        //_behaviorTree.Evaluate();
    }
    private void TransformSetUp()
    {
        _mTr_WorldObject = GetComponent<Transform>();
    }

    private void AbnormalSystemSetUp()
    {
        _abnormalSystem = new AbnormalSystem();
    }

    private void Update()
    {
        // AI 평가
        if (_behaviorTree != null)
            _behaviorTree.Evaluate();

        // Abnormal
        //float smoothDeltaTime = Time.smoothDeltaTime;
        //_abnormalSystem.OnUpdateAbnormals(smoothDeltaTime);
    }

    public int GetFressness()
    {
        int _freshness = 0;

        GameDataManager.GetInstance().GetGameDBCharacterInfo(this._mi_EntityTID, out var _ret);
        _freshness = _ret == null ? 0 : _ret._mi_Freshness;
        return _freshness;
    }

    public int GetDia()
    {
        int _dia = 0;

        GameDataManager.GetInstance().GetGameDBCharacterInfo(this._mi_EntityTID, out var _ret);
        _dia = _ret == null ? 0 : _ret._mi_Dia;
        return _dia;
    }

    public void SetKillDivision(bool _iskillPlayerDivision)
    {
        // True : Player
        // False : Rival
        _m_IsKillMine = _iskillPlayerDivision;
    }

    public override void OnDieEvent(Entity _entity)
    {
        // 붙은 헬스바 해제
        ReleaseWorldHealBarObject();

        // 적군 카운터 해제
        PlayerManager.GetInstance().AddEnemyCount(-1);

        int _freshness = GetFressness();
        int _dia = GetDia();

        if (_m_IsKillMine == true)
        {
            PlayerManager.GetInstance().AddGold(_freshness);
            PlayerManager.GetInstance().AddDia(_dia);

            //내가 죽였으면 내 HUD에 붙인다.
            PoolingManager.GetInstance().GetPooledObject(PooledObject.WO,PooledObjectInner.WO_CoinCountTag, out var _ret);
            var _pooledObject = _ret as PooledObjectCoinCountTag;
            _pooledObject.SetData(_freshness);
        }
        else
        {
            RivalPlayerAIManager.GetInstance().AddGold(_freshness);
            RivalPlayerAIManager.GetInstance().AddDia(_dia);
        }

        _m_ActPlayer.ClearActionInfos();
    }

    public void ReleaseWorldHealBarObject()
    {
        PooledObjectWorldHealBarTag _healthBarTag = _m_worldHaelBarTag as PooledObjectWorldHealBarTag;
        _healthBarTag.Release();
    }

    public void OnDrawGizmos()
    {
        //Color color = Color.blue;
        //Gizmos.color = color;
        //Gizmos.DrawWireSphere(this.transform.position, _attackRange);

        //color = Color.magenta;
        //Gizmos.color = color;
        //Gizmos.DrawWireSphere(this.transform.position, _detectRange);
    }
}
