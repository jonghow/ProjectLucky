using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;
using System;

public abstract class EntityContoller : MonoBehaviour {
    public ActionPlayer _m_ActPlayer;
    [SerializeField] EntityMoveAgent _m_MoveAgent;

    public Vector2 _mv2_Pos => this.transform.position;
    public Vector3 _mv3_Pos => this.transform.position;

    public long _ml_EntityUID;
    public int _mi_EntityTID;
    public long _ml_EntityGroupUID;

    public abstract void SetUp(long _entityUID, int _entityID);
    public void SetActPlayer(ActionPlayer _actPlayer) { _m_ActPlayer = _actPlayer; }
    public void SetMoveAgent(EntityMoveAgent _moveAgent) { _m_MoveAgent = _moveAgent; }
    public void GetMoveAgent(out EntityMoveAgent _moveAgent) { _moveAgent = _m_MoveAgent; }

    #region AI ����

    [SerializeField] bool _mb_ProcAI; // AI ����ǰ� �ִ��� üũ
    public void TurnOnAI() => _mb_ProcAI = true;
    public void TurnOffAI() => _mb_ProcAI = false;
    public bool IsTurnOnAI() => _mb_ProcAI == true ? true : false;
    #endregion

    #region ���� Ÿ�� ����
    private Entity _m_CachedChaseEntity; // ���� Ÿ��
    public void SetChaseEntity(Entity _entity) { _m_CachedChaseEntity = _entity; }
    public void GetChaseEntity(out Entity _entity) { _entity = _m_CachedChaseEntity; }
    #endregion

    #region 3D WorldObject �̵� ��ǥ ����
    [SerializeField] protected Transform _mTr_WorldObject;
    public Transform WorldObject { get { return _mTr_WorldObject; } }
    public Vector3 Pos3D
    {
        get { return _mTr_WorldObject != null ? _mTr_WorldObject.position : Vector3.zero; }
        set { _mTr_WorldObject.position = value; }
    }
    public Vector2 Pos2D
    {
        get { return _mTr_WorldObject != null ? _mTr_WorldObject.position : Vector2.zero; }
        set { _mTr_WorldObject.position = value; }
    }
    public Vector3 LookVector { get { return _mTr_WorldObject != null ? _mTr_WorldObject.forward : Vector3.forward; } }
    public Vector3 RightVector { get { return _mTr_WorldObject != null ? _mTr_WorldObject.right : Vector3.right; } }
    public Vector3 UpVector { get { return _mTr_WorldObject != null ? _mTr_WorldObject.up : Vector3.up; } }

    #endregion

    #region �� ���� ����

    [SerializeField] protected SpriteRenderer _m_SpriteRenderer;
    public SpriteRenderer SpriteRenderer { get { return _m_SpriteRenderer; } }

    public bool _mb_FaceDirection = false;

    public void OnFlipFaceDirection()
    {
        _mb_FaceDirection = !_mb_FaceDirection;

        if (_m_SpriteRenderer == null)
            SpriteRendererSetUp();

        _m_SpriteRenderer.flipX = _mb_FaceDirection;
    }
    public bool GetNowFaceDirection()
    {
        return _mb_FaceDirection == false; 
        // ������ ����, False ��� ������ �����ִ�.
        // üũ������ ������ False �� False�� ����
    }
    private void SpriteRendererSetUp()
    {
        _m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    #endregion

    #region ��ü ���� �̺�Ʈ �Լ� 

    public Action _onCB_DiedProcess;
    public Action _onCB_HitProcess;
    public Action _onCB_BuffProcess;
    public Action _onCB_AttackProcess;

    public virtual void OnHitEvent(Entity _entity) { }
    public virtual void OnBuffEvent(Entity _entity) { }
    public virtual void OnAttackEvent(Entity _entity) { }
    public virtual void OnDieEvent(Entity _entity) { }

    #endregion
}
