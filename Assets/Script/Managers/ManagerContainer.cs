using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerContainer 
{
    public static ManagerContainer Instance;
    public static ManagerContainer GetInstance()
    {
        if (Instance == null)
        {
            Instance = new ManagerContainer();
            Instance.Initialize();
        }

        return Instance;
    }
    public ManagerContainer()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode == true)
#endif
        {
            _m_Obj = new GameObject("Brocolling.Managers");
            _mTr_Obj = _m_Obj.transform;

            GameObject.DontDestroyOnLoad(_m_Obj);
        }
    }

    public GameObject _m_Obj;
    public Transform _mTr_Obj;

    protected CancellationTokenSource _onLoadedCancellationToken;
    bool _isLoadingComplete;

    private void Initialize()
    {
        _isLoadingComplete = false;
        _onLoadedCancellationToken = new CancellationTokenSource();
        _ = InitTime_LoadManager();
    }

    public async UniTask InitTime_LoadManager()
    {
        bool _isComplete = false;
        GameObject _gObj_Manager;

        while (true)
        {
            _isComplete = ResourceManager.GetInstance() != null ? true : false;

            ResourceManager _resourceMgr = ResourceManager.GetInstance();

            _resourceMgr.transform.SetParent(_mTr_Obj);
            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = AnimationManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"AnimationManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = GameDataManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"GameDataManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = InputManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"InputManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = HandCardManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"HandCardManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = PlayerManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"PlayerManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = MapManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"MapManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = SpawnerManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"SpanwerManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);

            _isComplete = PoolingManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"PoolingManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await UniTask.WaitUntil(() => _isComplete == true, cancellationToken: _onLoadedCancellationToken.Token);
            await PoolingManager.GetInstance().InitCollectObjects(_gObj_Manager);

            _isComplete = SoundManager.GetInstance() != null ? true : false;
            _gObj_Manager = new GameObject($"SoundManager");
            _gObj_Manager.transform.SetParent(_mTr_Obj);
            _gObj_Manager.transform.SetAsLastSibling();

            await SoundManager.GetInstance().InitLoadDatas();
            await SoundManager.GetInstance().InitCollectObjects(_gObj_Manager); 

            if (_isComplete == true)
                break;
        }

        _isLoadingComplete = true;
    }
}