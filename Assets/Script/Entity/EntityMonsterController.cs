using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;

public class EntityMonsterController : EntityContoller
{
    // ���� ���� ��� ����?
    private Avatar _avatar; // ����ȭ�� �� ���̽�
    private EntityBehaviorTreeBase _behaviorTree;

    // AnimatoController

    // Controller BuffSystem
    private AbnormalSystem _abnormalSystem;

    public List<Vector3> _mLt_ProbePosition; // ���Ͱ� ������ ������ �뷫 4���� �ȴ�. ��� A* �� �����ϰ͵��ƴϰ� �׳� ������ų��

    public bool _m_IsKillMine = true; // ���� ��Ҵ��� ���̹��� ��Ҵ���

    public IPoolBase _m_worldHaelBarTag; // HealBatTag

    public override void SetUp(long _entityUID, int _entityID)
    {
        _ml_EntityUID = _entityUID;
        _mi_EntityTID = _entityID;
        //TurnOffAI(); // �ʱ�� ����.
        TurnOnAI(); // ���ʹ� �����ʿ����� ����Ŷ�,ON �ϰ� �¾��.
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
        // AI ��
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
        // ���� �ｺ�� ����
        ReleaseWorldHealBarObject();

        // ���� ī���� ����
        PlayerManager.GetInstance().AddEnemyCount(-1);

        int _freshness = GetFressness();
        int _dia = GetDia();

        if (_m_IsKillMine == true)
        {
            PlayerManager.GetInstance().AddGold(_freshness);
            PlayerManager.GetInstance().AddDia(_dia);

            //���� �׿����� �� HUD�� ���δ�.
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
