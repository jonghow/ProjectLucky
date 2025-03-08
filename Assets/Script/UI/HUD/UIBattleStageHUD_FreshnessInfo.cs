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

public class UIBattleStageHUD_FreshnessInfo : MonoBehaviour , IBattleHUDActivation
{
    [SerializeField] TextMeshProUGUI _mText_Freshness;

    public void ProcActivationCardList(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    public void Awake()
    {
        _mText_Freshness.text = $"0";
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
        PlayerManager.GetInstance()._onCB_ChangeGold -= RefreshFreshnessInfo;
        PlayerManager.GetInstance()._onCB_ChangeGold += RefreshFreshnessInfo;
    }
    public void OnReleaseCBChangeStage()
    {
        PlayerManager.GetInstance()._onCB_ChangeGold -= RefreshFreshnessInfo;
    }

    public void RefreshFreshnessInfo(int _freshness)
    {
        _mText_Freshness.text = $"{_freshness}";
    }
}

