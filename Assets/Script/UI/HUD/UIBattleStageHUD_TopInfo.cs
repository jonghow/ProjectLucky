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

public class UIBattleStageHUD_TopInfo : MonoBehaviour, IBattleHUDActivation
{
    [SerializeField] Animator _anim_Count;
    [SerializeField] int _mi_EnemyCount;

    [SerializeField] TextMeshProUGUI _mText_EnemyCount;

    public void ProcActivationCardList(bool isActive) { }

    public void OnUpdateEnemySupplyCount(int _enemyCount)
    {
        _mi_EnemyCount= _enemyCount;
        _mText_EnemyCount.text = $"{_mi_EnemyCount}/{Defines.NormalSingleGameEnemyAllowCount}";

        _anim_Count.Play($"Update",0,0f);
    }
}

