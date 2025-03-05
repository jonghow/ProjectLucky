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

public class UIBattleStageHUD_MyHandInfo : MonoBehaviour , IBattleHUDActivation
{
    [SerializeField] List<UIHandCard> _mLt_HandCards;

    public void ProcActivationCardList(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    public void UpdateHandCards()
    {
        var _mHandItems = InventoryManager.GetInstance().GetHandCardItems();
        CardOff();

        int _mIdx = 0;
        foreach(var _pair in _mHandItems)
        {
            _mLt_HandCards[_mIdx].SetCard(_pair.Value.GetCardID(), _pair.Value.GetCardUID());
            _mLt_HandCards[_mIdx].gameObject.SetActive(true);

            ++_mIdx;
        }
    }
    public void CardOff()
    {
        for(int i = 0; i < _mLt_HandCards.Count; ++i)
            _mLt_HandCards[i].gameObject.SetActive(false);
    }
}

