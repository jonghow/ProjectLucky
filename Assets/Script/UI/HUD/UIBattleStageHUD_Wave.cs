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

public class UIBattleStageHUD_Wave : MonoBehaviour , IBattleHUDActivation
{
    [SerializeField] Animator _animator;
    CancellationTokenSource _cancellationToken;

    [SerializeField] TextMeshProUGUI _mText_Wave;

    int _waveID = 0;
    public void ProcActivationCardList(bool isActive) { }
    public void OnPlayWave(int _waveIndex)
    {
        this.gameObject.SetActive(true);
        _waveID = _waveIndex;
        _mText_Wave.text = $"Wave {_waveID}";
    }

    public void Update()
    {
        AnimatorStateInfo _animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if(_animStateInfo.normalizedTime >= 1f)
            this.gameObject.SetActive(false);
    }
}

