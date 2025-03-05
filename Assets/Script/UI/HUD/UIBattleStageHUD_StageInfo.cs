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

public class UIBattleStageHUD_StageInfo : MonoBehaviour , IBattleHUDActivation
{
    [SerializeField] TextMeshProUGUI _mText_Stage;

    public void ProcActivationCardList(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    public void Awake()
    {
        _mText_Stage.text = $"스테이지 1-0";
    }

    private void OnEnable()
    {
        OnSetCBChangeStage();
    }
    private void OnDisable()
    {
        OnReleaseCBChangeStage();
    }

    public void OnSetCBChangeStage()
    {
        SceneLoadManager.GetInstance().GetStage(out var _stage);

        if (_stage is BattleStage _battleStage)
        {
            _battleStage._onCB_ChangeStage -= RefreshStageInfo;
            _battleStage._onCB_ChangeStage += RefreshStageInfo;
        }
    }
    public void OnReleaseCBChangeStage()
    {
        SceneLoadManager.GetInstance().GetStage(out var _stage);

        if (_stage is BattleStage _battleStage)
        {
            _battleStage._onCB_ChangeStage -= RefreshStageInfo;
        }
    }

    public void RefreshStageInfo(int _stage)
    {
        _mText_Stage.text = $"스테이지 1-{_stage}";
    }
}

