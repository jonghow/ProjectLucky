using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading;

public class ActionInfoAttack : ActionInfoBase
{
    public override void DoExecute(Animator _animator , int _totalFrame)
    {
        base.DoExecute(_animator, _totalFrame);

        if(_hs_ProcessedFrame == null ) 
            _hs_ProcessedFrame = new HashSet<int>();

        _hs_ProcessedFrame.Clear();

        _m_Animator = _animator;
        _ = DoExecuteFuction();
    }

    protected override async UniTask DoExecuteFuction()
    {
        if (_cancellationToken == null) return;

        CancellationToken token = _cancellationToken.Token; //  기존 토큰을 저장하여 사용

        while (true)
        {
            if (token.IsCancellationRequested) //  기존 토큰을 유지하여 체크
                break;

            if (_m_Animator == null)
                break;

            AnimatorStateInfo _stateInfo = _m_Animator.GetCurrentAnimatorStateInfo(2);

            float _normalizeTime = _stateInfo.normalizedTime % 1;

            _mi_CurFrame = Mathf.FloorToInt(_normalizeTime * _mi_TotalFrame) + 1;

            var _actList = _m_ChrActInfo.GetActXMLInfoByFrame(_mi_CurFrame);

            if (_actList.Count > 0 && !_hs_ProcessedFrame.Contains(_mi_CurFrame))
            {
                //UnityLogger.GetInstance().Log($"=== Frame Proc Start ==== ");
                //UnityLogger.GetInstance().Log($" Frame :: {_mi_CurFrame}");

                _hs_ProcessedFrame.Add(_mi_CurFrame);


                foreach (var _actInfo in _actList)
                {
                    switch (_actInfo.ActType)
                    {
                        case "DamageObject":
                            //Debug.Log($"Action DamageObject!! CurFrame :: {_mi_CurFrame}");
                            DamageAction();
                            break;
                        case "EffectObject":
                            //Debug.Log($"Action EffectObject Hit!! CurFrame :: {_mi_CurFrame}");
                            break;
                        case "SoundObject":
                            //Debug.Log($"Action SoundObject Hit!! CurFrame :: {_mi_CurFrame}");
                            break;
                        case "DamageObject_Projectile":
                            DamageAction_PooledObject_Projectile();
                            break;
                        case "DamageObject_Fireball":
                            DamageAction_PooledObject_FireBall();
                            break;
                        default:
                            break;
                    }
                }
            }

            await UniTask.Yield(PlayerLoopTiming.Update, _cancellationToken.Token);
        }
    }

    public void DamageAction()
    {
        Entity _chaseEntity;
        _mCachedOnwerEntityController.GetChaseEntity(out _chaseEntity);

        if (_chaseEntity == null || _chaseEntity.Info.IsDead())
            return;

        if (_chaseEntity != null && !_chaseEntity.Info.IsDead())
            BattleRoutine.OnHitUpdateBattleRoutine(_ml_OwnerUID, _chaseEntity.UID);

        SoundManager.GetInstance().PlaySFX($"SoundSFX_Punch");
    }

    public void DamageAction_PooledObject_Projectile()
    {
        Entity _chaseEntity;
        _mCachedOnwerEntityController.GetChaseEntity(out _chaseEntity);

        if (_chaseEntity == null || _chaseEntity.Info.IsDead())
            return;

        IPoolBase _IPooled;
        PoolingManager.GetInstance().GetPooledObject(PooledObject.WO, PooledObjectInner.WO_Projectile_Arrow, out _IPooled);
        PooledObjectArrow _pooledObject = _IPooled as PooledObjectArrow;

        _pooledObject.SetData(_mCachedOnwerEntityController._ml_EntityUID, _chaseEntity.UID);

        SoundManager.GetInstance().PlaySFX($"SoundSFX_ArrowShot");
    }

    public void DamageAction_PooledObject_FireBall()
    {
        Entity _chaseEntity;
        _mCachedOnwerEntityController.GetChaseEntity(out _chaseEntity);

        if (_chaseEntity == null || _chaseEntity.Info.IsDead())
            return;

        IPoolBase _IPooled;
        PoolingManager.GetInstance().GetPooledObject(PooledObject.WO, PooledObjectInner.WO_Projectile_FireBall, out _IPooled);
        PooledObjectFireball _pooledObject = _IPooled as PooledObjectFireball;

        _pooledObject.SetData(_mCachedOnwerEntityController._ml_EntityUID, _chaseEntity.UID);

        SoundManager.GetInstance().PlaySFX($"SoundSFX_Fireball");
    }
}
