using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SpawnerManager
{
    private static SpawnerManager Instance;
    /// <summary>
    /// NavitionSystem Movable Check를 위한 매니저입니다.
    /// </summary>
    /// <returns>Manager Object</returns>
    /// 

    private Dictionary<int, ISpawnerBase> _mDict_SpawnerInfo;
    private int _mi_SpawnerKey;

    public static SpawnerManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SpawnerManager();
            Instance.OnInitialize();
        }

        return Instance;
    }

    public void OnInitialize()
    {
        _mDict_SpawnerInfo = new Dictionary<int, ISpawnerBase>();
    }

    /// <summary>
    /// Stage ID로 MapNavigation을 불러올 예정입니다.
    /// </summary>
    /// <param name="_stageID"></param>
    public void LoadEditSpanwer(int _stageID)
    {
        ClearSpawner();

        ResourceManager.GetInstance().GetResource(ResourceType.SpawnerData, _stageID, true, (_gObj_SpawnerXMLData) => {
            UnityEngine.Object _xmlObject = _gObj_SpawnerXMLData as UnityEngine.Object;
            var _mapText = _xmlObject as TextAsset;

            if(string.IsNullOrEmpty(_mapText.text))
            {
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadEditSpanwer", $" Loaded Stage Idx : {_stageID} Spawner File Empty");
                return;
            }

            SpawnerWrap _loadedSpawnerData = XMLUtility.Deserialize<SpawnerWrap>(_mapText.text);

            for (int i = 0; i < _loadedSpawnerData.List.Count; ++i)
            {
                var _spawner = _loadedSpawnerData.List[i];
                int _spawnerID = (_spawner as SpawnerUseEdit)._mi_IndexID;
                var _ISpawner = _spawner as ISpawnerBase;

                if (_mDict_SpawnerInfo.ContainsKey(_spawnerID) == true)
                {
                    UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadStage", $"stage ID {_stageID}, 동일한 네비게이션 그리드가 있습니다. 게임 종료하겠습니다. 확인해주세요!");
                    Application.Quit();
                }

                _mDict_SpawnerInfo.Add(_spawnerID, _ISpawner);
            }
        });
    }
    public void ClearSpawner()
    {
        _mDict_SpawnerInfo.Clear();
        GC.Collect();
    }
    public void AddSpawner(int _spawnerKey, ref ISpawnerBase _spawner)
    {
        if(_mDict_SpawnerInfo.ContainsKey(_spawnerKey))
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"AddSpanwer", $"_spawnerKey 가 동일한 원소가 있습니다. 확인해주세요.");
            return;
        }

        _mDict_SpawnerInfo.Add(_spawnerKey, _spawner);
    }
    public void UpdateSpawnerKey()
    {
        _mi_SpawnerKey = 0;
        var _values = GetSpawnerDeepCopy<ISpawnerBase>();

        ClearSpawner();

        foreach(var _value in _values)
        {
            ISpawnerBase _v = _value;
            (_v as SpawnerBase)._mi_IndexID = _mi_SpawnerKey;
            AddSpawner(_mi_SpawnerKey++, ref _v);
        }
    }

    public List<T> GetSpawnerDeepCopy<T>() where T :  ISpawnerBase
    {
        List<T> _ret = new List<T>();

        foreach (var spawner in _mDict_SpawnerInfo.Values)
        {
            if (spawner is T typedSpawner)
            {
                _ret.Add((T)typedSpawner.Clone());
            }
        }

        return _ret;
    }

    public bool IsContainAnyData()
    {
        return !_mDict_SpawnerInfo.IsNullOrEmpty();
    }
    public int GetSpawnerKey()
    {
        UpdateSpawnerKey();
        return _mi_SpawnerKey;
    }
    public void GetSpawnerDatas(out Dictionary<int, ISpawnerBase> _ret) => _ret = _mDict_SpawnerInfo;
    public async UniTask LoadSpawner(int _stageID, int _step, Action _onCBComplete = null)
    {
        bool _isLoaded = false;
        int _calcedStageID = _stageID + _step;

        ResourceManager.GetInstance().GetResource(ResourceType.SpawnerData, _calcedStageID, true, (_gObj_SpawnerXMLData) =>
        {
            UnityEngine.Object _xmlObject = _gObj_SpawnerXMLData as UnityEngine.Object;
            var _mapText = _xmlObject as TextAsset;

            if (string.IsNullOrEmpty(_mapText.text))
            {
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadStage", $" Loaded Stage Idx : {_calcedStageID} Spawner File Empty");
                return;
            }

            var serializer = new XmlSerializer(typeof(SpawnerWrap));
            SpawnerWrap _loadedSpawnerData = XMLUtility.Deserialize<SpawnerWrap>(_mapText.text);

            for (int i = 0; i < _loadedSpawnerData.List.Count; ++i)
            {
                var _element = _loadedSpawnerData.List[i];
                int _index = _element._mi_IndexID;
                var _ISpawner = _element as ISpawnerBase;
                _ISpawner.Initialize();

                SpawnerManager.GetInstance().GetSpawnerDatas(out var spawnerDatas);

                if (spawnerDatas.ContainsKey(_index) == true)
                {
                    UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadStage", $"stage ID {_stageID}, 동일한 스포너 ID가 있습니다. 게임 종료하겠습니다. 확인해주세요!");
                    Application.Quit();
                }
                SpawnerManager.GetInstance().AddSpawner(_index, ref _ISpawner);
            }

            _isLoaded = true;
        });

        await UniTask.WaitUntil(() => _isLoaded == true);
        _onCBComplete?.Invoke();
    }
    public void CommandAllSpawnStart()
    {
        foreach(var _spawnerPair in _mDict_SpawnerInfo)
        {
            SpawnerUseEdit _ingameSpawner = _spawnerPair.Value as SpawnerUseEdit;
            _ingameSpawner.StartSpawn();
        }
    }

    private int _mi_SpawnCompleteCount = 0;

    public void AddSpawnComplete()
    {
        _mi_SpawnCompleteCount += 1;
    }

    private void CheckAllSpawnCount()
    {
        if(_mDict_SpawnerInfo.Count == _mi_SpawnCompleteCount)
        {
            SceneLoadManager.GetInstance().GetStage(out var _stageBase);
            if(_stageBase is BattleStage _battleStage)
            {
                int _stageStep = _battleStage.GetStageValue();
                _stageStep += 1;

                _ = LoadSpawner(1000, _stageStep);
            }
        }
    }



    public void CommandAllSpawnStop()
    {
        foreach (var _spawnerPair in _mDict_SpawnerInfo)
        {
            SpawnerUseEdit _ingameSpawner = _spawnerPair.Value as SpawnerUseEdit;
            _ingameSpawner.StopSpawn();
            _ingameSpawner.ClearSpawnCount();
        }
    }
    public bool GetIsSpawning()
    {
        bool _ret = false;

        foreach (var _spawnerPair in _mDict_SpawnerInfo)
        {
            SpawnerUseEdit _ingameSpawner = _spawnerPair.Value as SpawnerUseEdit;

            if(_ingameSpawner.IsSpawning())
            {
                _ret = true;
                break;
            }
        }
        return _ret;
    }

    private ISpawnerBase _selectedSpawnerInfo; // Editor에서 사용
    public ISpawnerBase SelectedSpawnerInfo
    {
        get { return _selectedSpawnerInfo; }
        set { _selectedSpawnerInfo = value; }
    }
}
