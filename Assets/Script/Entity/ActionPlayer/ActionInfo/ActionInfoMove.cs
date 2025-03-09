using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ActionInfoMove : ActionInfoBase
{
    public override void DoExecute(Animator _animator, int _totalFrame)
    {
        base.DoExecute(_animator, _totalFrame);

        if (_hs_ProcessedFrame == null)
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
            if (token.IsCancellationRequested)
                break;

            if (_m_Animator == null)
                break;

            AnimatorStateInfo _stateInfo = _m_Animator.GetCurrentAnimatorStateInfo(2);

            float _normalizeTime = _stateInfo.normalizedTime % 1;

            if (_normalizeTime > 0.99f)
                break;

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

                            Entity _chaseEntity;
                            _mCachedOnwerEntityController.GetChaseEntity(out _chaseEntity);

                            BattleRoutine.OnHitUpdateBattleRoutine(_ml_OwnerUID, _chaseEntity.UID);
                            break;
                        case "EffectObject":
                            //Debug.Log($"Action EffectObject Hit!! CurFrame :: {_mi_CurFrame}");
                            break;
                        case "SoundObject":
                            //Debug.Log($"Action SoundObject Hit!! CurFrame :: {_mi_CurFrame}");
                            break;
                        case "DamageObject_Projectile":

                            Debug.Log($"DamageObject_Projectile Hit!! CurFrame :: {_mi_CurFrame}");

                            //WO_Arrow

                            Addressables.LoadAssetAsync<GameObject>($"WO_Arrow").Completed += (op) =>
                            {
                                if (((AsyncOperationHandle<GameObject>)op).Status == AsyncOperationStatus.Succeeded)
                                {
                                    var _loadedArrow = op.Result;
                                    var _pooledObject = _loadedArrow.GetComponent<PooledObjectArrow>();

                                    _mCachedOnwerEntityController.GetChaseEntity(out var _chaseObject);

                                    _pooledObject.SetData(_mCachedOnwerEntityController._ml_EntityUID, _chaseObject.UID);
                                }
                            };

                            break;
                        default:
                            break;
                    }
                }

                //Debug.Log($"=== Frame Proc End ==== ");
            }

            await UniTask.Yield(PlayerLoopTiming.Update, _cancellationToken.Token);
        }
    }


}
