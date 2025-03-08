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
using System;
using UnityEngine.U2D;
using EntityBehaviorTree;

public class RivalPlayerAI : MonoBehaviour 
{
    bool _mb_IsAIStop;
    int _mi_DrawTarget; // 0 : UnCommon, 1 : Hero, 2: Myth

    EntityBehaviorTreeBase _behaviorTree;

    int _mi_CombineID;
    long _ml_CombineUID;

    public void Start()
    {
        //TurnOnAI();
        _mi_CombineID = 0;
        _ml_CombineUID = 0;

        TurnOffAI();
        SetDrawTarget(0);
        AISetUp();
    }
    public void TurnOnAI()
    {
        _mb_IsAIStop = true;
    }
    public void TurnOffAI()
    {
        _mb_IsAIStop = false;
    }
    public bool IsTurnOnAI() => _mb_IsAIStop;
    public void SetDrawTarget(int _drawTargetIndex)
    {
        _mi_DrawTarget = _drawTargetIndex;
    }
    public void SetCombineID(int _combineID)
    {
        _mi_CombineID = _combineID;
    }
    public void SetCombineUID(long _combineUID)
    {
        _ml_CombineUID = _combineUID;
    }

    public int GetCombineID() => _mi_CombineID;
    public long GetCombineUID() => _ml_CombineUID;
    public int GetDrawTarget() => _mi_DrawTarget;
    public void AISetUp()
    {
        _behaviorTree = new RivalPlayerAIType($"RivalPlayerAIType", 0, null);
    }

    public void Update()
    {
        if (_behaviorTree != null)
            _behaviorTree.Evaluate();
    }


}

