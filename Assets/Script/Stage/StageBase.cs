using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class StageBase : MonoBehaviour
{
    protected bool _isLoadingComplete = false;
    protected CancellationTokenSource _onLoadedCancellationToken;

    public virtual void InitStage()
    {
        _isLoadingComplete = false; 
        _onLoadedCancellationToken = new CancellationTokenSource();
    }

    protected List<InputCommandBase> _mLt_useCommand;
    public bool IsUseInputCommand() => _mLt_useCommand != null;

    private void OnEnable()
    {
        InputManager.GetInstance()._mCB_OnChangeInputSystem -= RefreshInputSystem;
        InputManager.GetInstance()._mCB_OnChangeInputSystem += RefreshInputSystem;
    }

    private void OnDisable()
    {
        InputManager.GetInstance()._mCB_OnChangeInputSystem -= RefreshInputSystem;
    }

    public void RefreshInputSystem()
    {
        _mLt_useCommand = InputManager.GetInstance().GetNowUseInputCommandList();
    }
}
