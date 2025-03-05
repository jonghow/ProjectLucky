using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


// 좌표 기반 이동으로 변경
public class PooledObjectFireball : PooledBase , IPoolBase
{
    [SerializeField] long _ml_shooterUID;
    [SerializeField] long _ml_effectorUID;

    private Entity _m_CachedShooterEntity;
    private Entity _m_CachedEffectorEntity;

    Vector3 _mv3_TargetPosition;
    Vector3 _mv3_TargetDirection;
    [SerializeField] float _mf_MoveSpeed;
    public void Regist()
    {
        this.gameObject.SetActive(true);

        _me_PooledType = PooledObject.WO;
        _me_PooledInnerType = PooledObjectInner.WO_Projectile_FireBall;
    }

    public void Release()
    {
        var _IPooledBase = this as IPoolBase;
        PoolingManager.GetInstance().CollectObject(_me_PooledType, _me_PooledInnerType, ref _IPooledBase);
    }

    public void SetData(long _shooterUID, long _effectUID)
    {
        _mv3_TargetDirection = Vector3.zero;

        _ml_shooterUID = _shooterUID;
        _ml_effectorUID = _effectUID;

        EntityManager.GetInstance().GetEntity(_ml_shooterUID, out _m_CachedShooterEntity);
        EntityManager.GetInstance().GetEntity(_ml_effectorUID, out _m_CachedEffectorEntity);

        this.transform.position = _m_CachedShooterEntity.Controller.Pos3D;
        _mv3_TargetPosition = _m_CachedEffectorEntity.Controller.Pos3D;
        _mf_MoveSpeed = 3.8f;

        Regist();
        //transform.LookAt(_m_CachedEffectorEntity.transform.position);
    }
    public void Update()
    {
        if(IsEffectorObjectDead())
        {
            Release();
        }
        else if (IsCollision())
        {
            BattleRoutine.OnHitUpdateBattleRoutine(_ml_shooterUID, _ml_effectorUID);
            Release();
        }
        else if(IsReachedTargetPos())
        {
            Release();
        }
        else
        {
            OnUpdatePos();
            OnUpdateRot();
        }
    }

    public bool IsEffectorObjectDead()
    {
        EntityManager.GetInstance().GetEntity(_ml_effectorUID, out _m_CachedEffectorEntity);
        if (_m_CachedEffectorEntity == null || _m_CachedEffectorEntity.Info.IsDead())
            return true;

        return false;
    }
    private void OnUpdatePos()
    {
        Vector3 _P0 = this.transform.position;

        _mv3_TargetDirection = (_mv3_TargetPosition - this.transform.position).normalized;

        Vector3 _AT = Time.deltaTime * _mv3_TargetDirection * _mf_MoveSpeed;

        Vector3 _P1 = _P0 + _AT;
        this.transform.position = _P1;
    }

    private void OnUpdateRot()
    {
        float angle = Mathf.Atan2(_mv3_TargetDirection.y, _mv3_TargetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private bool IsCollision()
    {
        bool _ret = false;

        float _Distance = Vector3.SqrMagnitude(this.transform.position - _m_CachedEffectorEntity.transform.position);

        if (_Distance <= 0.35f)
            _ret = true;

        return _ret;
    }

    private bool IsReachedTargetPos()
    {
        bool _ret = false;

        float _Distance = Vector3.SqrMagnitude(this.transform.position - _mv3_TargetPosition);

        if (_Distance <= 0.15f)
            _ret = true;

        return _ret;
    }
    public void ClearData()
    {
        _ml_shooterUID = 0;
        _ml_effectorUID = 0;

        _m_CachedShooterEntity = null;
        _m_CachedEffectorEntity = null;

        _mv3_TargetDirection = Vector3.zero;
    }
}
