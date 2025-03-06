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

public class UIBattleStageHUD_LuckyDraw : MonoBehaviour , IBattleHUDActivation
{
    [SerializeField] TMPro.TextMeshProUGUI _mText_dia;
    [SerializeField] TMPro.TextMeshProUGUI _mText_Supply;

    public void ProcActivationCardList(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    public void OnClick_DrawUnCommon()
    {
        int _rate = 60;

        int _drawVal = UnityEngine.Random.Range(0, 100);

        if(_rate >= _drawVal)
        {
            // success
        }
        else
        {
            // fail
        }
    }
    public void OnClick_DrawHero()
    {
        int _rate = 20;

        int _drawVal = UnityEngine.Random.Range(0, 100);

        if (_rate >= _drawVal)
        {
            // success
        }
        else
        {
            // fail
        }

    }
    public void OnClick_DrawMyth()
    {
        int _rate = 20;

        int _drawVal = UnityEngine.Random.Range(0, 100);

        if (_rate >= _drawVal)
        {
            // success
        }
        else
        {
            // fail
        }
    }



}

