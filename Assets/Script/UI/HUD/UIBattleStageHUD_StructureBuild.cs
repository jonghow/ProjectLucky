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

public class UIBattleStageHUD_StructureBuild : MonoBehaviour, IBattleHUDActivation
{
    public void ProcActivationCardList(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }
}

