using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using GlobalGameDataSpace;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEditor;

public partial class GameDataManager
{
    Dictionary<int, GameDB_BuildingInfo> _dict_BuildingInfo;
    Dictionary<int, GameDB_BuildingStat> _dict_BuildingStat;

    private void InitializeBuildingPartial()
    {
        _dict_BuildingInfo = new Dictionary<int, GameDB_BuildingInfo>();
        _dict_BuildingStat = new Dictionary<int, GameDB_BuildingStat>();
    }

    public async UniTask UTask_Load_GameDBBuildingDatas()
    {
        await UTask_Load_GameDBBuildingInfo();
        await UTask_Load_GameDBBuildingStat();

        _isLoaded = true;
    }

    private async UniTask UTask_Load_GameDBBuildingInfo()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        _loadingFileName = $"GameDB_BuildingInfo";

        Addressables.LoadAssetAsync<TextAsset>(_loadingFileName).Completed += (op) =>
        {
            _loaded = true;

            if (((AsyncOperationHandle<TextAsset>)op).Status == AsyncOperationStatus.Succeeded)
            {
                // 로딩에 성공
                var _loadedTextAsset = op.Result;
                _xmlDoc.LoadXml(_loadedTextAsset.text);

                XmlNode _root = _xmlDoc.DocumentElement;
                XmlNodeList _nodes = _root.SelectNodes("Item");

                foreach (XmlNode _node in _nodes)
                {
                    GameDB_BuildingInfo _gameDB_Building = new GameDB_BuildingInfo(_node);
                    int _BuildingID = _gameDB_Building._mi_ID;

                    if (_dict_BuildingInfo.ContainsKey(_BuildingID))
                    {
                        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_GameDBBuildingInfo", $"동일한 ID를 가진 BuildingID가 존재합니다.");
                        EditorApplication.isPlaying = false;
                    }

                    _dict_BuildingInfo.Add(_BuildingID, _gameDB_Building);
                }
            }
        };

        await UniTask.WaitUntil(() => _loaded == true);
    }

    private async UniTask UTask_Load_GameDBBuildingStat()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        _loadingFileName = $"GameDB_BuildingStat";

        Addressables.LoadAssetAsync<TextAsset>(_loadingFileName).Completed += (op) =>
        {
            _loaded = true;

            if (((AsyncOperationHandle<TextAsset>)op).Status == AsyncOperationStatus.Succeeded)
            {
                // 로딩에 성공
                var _loadedTextAsset = op.Result;
                _xmlDoc.LoadXml(_loadedTextAsset.text);

                XmlNode _root = _xmlDoc.DocumentElement;
                XmlNodeList _nodes = _root.SelectNodes("Item");

                foreach (XmlNode _node in _nodes)
                {
                    GameDB_BuildingStat _gameDB_BuildingStat = new GameDB_BuildingStat(_node);
                    int _BuildingStatID = _gameDB_BuildingStat._mi_StatID;

                    if (_dict_BuildingStat.ContainsKey(_BuildingStatID))
                    {
                        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_GameDBBuildingStat", $"동일한 ID를 가진 BuildingStatID가 존재합니다.");
                        EditorApplication.isPlaying = false;
                    }

                    _dict_BuildingStat.Add(_BuildingStatID, _gameDB_BuildingStat);
                }
            }
        };

        await UniTask.WaitUntil(() => _loaded == true);
    }

    public void GetGameDBBuildingInfo(int _buildingInfoID, out GameDB_BuildingInfo _ret)
    => _dict_BuildingInfo.TryGetValue(_buildingInfoID, out _ret);

    public void GetGameDBBuildingStat(int _buildingStatID, out GameDB_BuildingStat _ret)
=> _dict_BuildingStat.TryGetValue(_buildingStatID, out _ret);
}
