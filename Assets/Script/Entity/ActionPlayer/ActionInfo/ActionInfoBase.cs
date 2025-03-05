using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ActionInfoBase 
{
    protected long _ml_OwnerUID;

    public string _mStr_AnimName;
    public int _mi_CurFrame;
    public int _mi_TotalFrame;

    public Animator _m_Animator;
    public CancellationTokenSource _cancellationToken;
    protected ChrActXMLInfo _m_ChrActInfo;

    protected Entity _mCachedOwnerEntity;
    protected EntityContoller _mCachedOnwerEntityController;

    public HashSet<int> _hs_ProcessedFrame;
    public void SetActInfo(ChrActXMLInfo _chrActInfo, long _ownerUID)
    {
        _m_ChrActInfo = _chrActInfo;
        _ml_OwnerUID = _ownerUID;

        ClearToken();
        _cancellationToken = new CancellationTokenSource();

        SetCacheOwnerEntity();
    }

    public void SetCacheOwnerEntity()
    {
        Entity _entity;
        EntityManager.GetInstance().GetEntity(_ml_OwnerUID, out _entity);
        _mCachedOwnerEntity = _entity;

        _mCachedOnwerEntityController = _entity.Controller;
    }
    public void StopExecute() { ClearToken(); }
    public virtual void DoExecute(Animator _animator, int _totalFrame) 
    {
        _mi_TotalFrame = _totalFrame;
    }
    public void ClearToken()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
        }

        _cancellationToken = new CancellationTokenSource();
    }

    public void DestoryExecute()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
        }

        DestroyActor();
    }

    public void DestroyActor()
    {

        GameObject.DestroyImmediate(_m_Animator);
    }

    protected virtual async UniTask DoExecuteFuction() { await UniTask.Yield(_cancellationToken.Token); }
}
