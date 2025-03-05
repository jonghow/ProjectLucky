using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SceneLoadManager 
{
    public static SceneLoadManager Instance;

    public static SceneLoadManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SceneLoadManager();
            Instance.Initialize();
        }
        return Instance;
    }

    CancellationTokenSource _onLoadSceneCancellationToken;
    bool _isLoadingComplete;

    StageBase _m_CurrentStage;

    public void GetStage(out StageBase _stage) => _stage = _m_CurrentStage;

    private void Initialize()
    {
        _isLoadingComplete = false;
        _onLoadSceneCancellationToken = new CancellationTokenSource();
    }

    public async UniTask OnLoadTitleScene()
    {
        await UniTask.Yield(PlayerLoopTiming.Update, _onLoadSceneCancellationToken.Token);
        await Resources.UnloadUnusedAssets();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync($"StartScene");

        while (!asyncLoad.isDone)
            await UniTask.Yield(PlayerLoopTiming.Update, _onLoadSceneCancellationToken.Token);
    }

    public async UniTask OnLoadScene(string sceneName, int _stageID, Action onCallback = null)
    {
        UnityEngine.Debug.Log($"Start Load Scene : {sceneName} ");
        await UniTask.Yield(PlayerLoopTiming.Update, _onLoadSceneCancellationToken.Token);
        await Resources.UnloadUnusedAssets();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync($"{sceneName}");

        Stopwatch st = Stopwatch.StartNew();
        st.Start();

        while (!asyncLoad.isDone)
            await UniTask.Yield(PlayerLoopTiming.Update, _onLoadSceneCancellationToken.Token);

        st.Stop();
        UnityEngine.Debug.Log($"End Load Scene : {sceneName} , Load Time : {st.ElapsedMilliseconds}");

        CreateStageScript(_stageID, out _m_CurrentStage);

        await CreateHUD(_stageID);
        await MapManager.GetInstance().LoadStage(1001);
        await SpawnerManager.GetInstance().LoadSpawner(1000,0);

        _m_CurrentStage.InitStage();
        onCallback?.Invoke();
    }

    public void CreateStageScript(int _stageID, out StageBase _createStage)
    {
        GameObject _newGameObject = new GameObject($"StageObject");
        _createStage = null;

        if (_stageID == 1001)
        {
            _createStage = _newGameObject.AddComponent<BattleStage>();
        }
        else
        {
            UnityLogger.GetInstance().Log($"작업이 필요합니다.");
        }

        _newGameObject.transform.SetAsFirstSibling();
    }

    public async UniTask CreateHUD(int _stageID)
    {
        bool _isLoaded = false;

        if (_stageID == 1001)
        {
            ResourceManager.GetInstance().GetResource(ResourceType.HUD, _stageID, true, (_gObject) => {

                var _findParent = GameObject.Find($"HUD");
                var _gObj = GameObject.Instantiate(_gObject, _findParent.transform);

                //_gObj.transform.SetParent(_findParent.transform);
                _isLoaded = true;
            });
        }

        await UniTask.WaitUntil(() => _isLoaded == true); 
    }
}


