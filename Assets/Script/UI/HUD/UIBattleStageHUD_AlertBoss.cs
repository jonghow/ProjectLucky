using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class UIBattleStageHUD_AlertBoss : MonoBehaviour , IBattleHUDActivation
{
    public enum AlertBossState
    {
        None,
        In,
        Loop,
        Out
    }

    [SerializeField] AlertBossState _me_AlertState;
    [SerializeField] Animator _animator;
    CancellationTokenSource _cancellationToken;

    int _spawnEnitityID = 0;
    public void ProcActivationCardList(bool isActive) { }
    public void OnPlayAlertBoss(int _bossIndex)
    {
        this.gameObject.SetActive(true);
        _spawnEnitityID = _bossIndex;
        _me_AlertState = AlertBossState.None;
    }

    public void OnClick_SpawnBoss()
    {
        _me_AlertState = AlertBossState.Out;
        _animator.Play("Out");

        Vector3 _newMyBossSpawnPos = new Vector3(-3.8f, -3.2f, 0f);
        bool _isMySpwner = true;

        EnemyEntityFactory _factory = new EnemyEntityFactory();

        _ = _factory.CreateEntity(_spawnEnitityID, _newMyBossSpawnPos, _isMySpwner, (_createEntity) =>
        {
            PlayerManager.GetInstance().AddEnemyCount(1);

            _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
        });
    }

    public void Update()
    {
        AnimatorStateInfo _animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        switch (_me_AlertState)
        {
            case AlertBossState.None:
                _me_AlertState = AlertBossState.In;
                _animator.Play("In");
                break;
            case AlertBossState.In:
                if (_animStateInfo.normalizedTime >= 1f)
                {
                    _me_AlertState = AlertBossState.Loop;
                    _animator.Play("Loop");
                }
                break;
            case AlertBossState.Loop:
                if (_animStateInfo.normalizedTime >= 1f)
                {
                    _me_AlertState = AlertBossState.Out;
                    _animator.Play("Out");
                }
                break;
            case AlertBossState.Out:
                {
                    if (_animStateInfo.normalizedTime >= 1f)
                        this.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
}

