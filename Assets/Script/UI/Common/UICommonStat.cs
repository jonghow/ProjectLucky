using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.UI;
using TMPro;


public class UICommonStat : StageBase
{
    [SerializeField] TextMeshProUGUI _text_Value;

    public void SetStat(StatType type, float value)
    {
        _text_Value.text = value.ToString();
    }
}
