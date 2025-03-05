using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.UI;
using TMPro;

public class UICommonHPBar : StageBase
{
    private float _mf_maxHp;
    private float _mf_CurHp;

    private float _mf_HPRate =>  (_mf_CurHp/_mf_maxHp);
    private float _mf_HPRatePercent =>  (_mf_CurHp/_mf_maxHp) * 100f;

    [SerializeField] Image _m_Img_HpBar;
    [SerializeField] TextMeshProUGUI _m_Img_HpValue;
    public Gradient healthGradient; // 체력에 따른 그라데이션


    public void SetHP(float _hp , float _maxhp)
    {
        _mf_CurHp = _hp;
        _mf_maxHp = _maxhp;

        _m_Img_HpValue.text = $"{(int)_mf_CurHp} / {(int)_mf_maxHp} ({_mf_HPRatePercent}%)";
        OnUpdateHpBar();
    }

    private void OnUpdateHpBar()
    {
        _m_Img_HpBar.fillAmount = _mf_HPRate;
        _m_Img_HpBar.color = healthGradient.Evaluate(_mf_HPRate);
    }
}
