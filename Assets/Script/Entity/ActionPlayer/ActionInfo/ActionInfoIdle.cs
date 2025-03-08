using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

public class ActionInfoIdle : ActionInfoBase
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
            if (token.IsCancellationRequested) //  기존 토큰을 유지하여 체크
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
                //,UnityLogger.GetInstance().Log($"=== Frame Proc Start ==== ");
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
                            UnityLogger.GetInstance().Log($"현재는 Act XML 꾸겨넣었지만, 추후에 SKILLTABLE로 옮겨서 작성합시다.");
                            break;
                        default:
                            break;
                    }
                }

                //UnityLogger.GetInstance().Log($" $"=== Frame Proc End ====");
            }

            await UniTask.Yield(PlayerLoopTiming.Update, _cancellationToken.Token);
        }
    }


}
