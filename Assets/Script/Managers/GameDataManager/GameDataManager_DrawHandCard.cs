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
    Dictionary<int, GameDB_DrawHandCardInfo> _dict_DrawHandCardInfo;

    private void InitializeDrawHandCardPartial()
    {
        _dict_DrawHandCardInfo = new Dictionary<int, GameDB_DrawHandCardInfo>();
    }
    public async UniTask UTask_Load_GameDBDarwHandCardDatas()
    {
        await UTask_Load_GameDBDrawHandCardInfo();
        _isLoaded = true;
    }
    private async UniTask UTask_Load_GameDBDrawHandCardInfo()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        _loadingFileName = $"GameDB_DrawHandCardInfo";

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
                    GameDB_DrawHandCardInfo _gameDB_DrawHandCardInfo = new GameDB_DrawHandCardInfo(_node);
                    int _cardID = _gameDB_DrawHandCardInfo._mi_ID;

                    if (_dict_DrawHandCardInfo.ContainsKey(_cardID))
                    {
                        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_GameDBDrawHandCardInfo", $"동일한 ID를 가진 _cardID 존재합니다.");
                        EditorApplication.isPlaying = false;
                    }

                    _dict_DrawHandCardInfo.Add(_cardID, _gameDB_DrawHandCardInfo);
                }
            }
        };

        await UniTask.WaitUntil(() => _loaded == true);
    }
    public void GetGameDBDrawHandCard(int _cardID, out GameDB_DrawHandCardInfo _ret)
    => _dict_DrawHandCardInfo.TryGetValue(_cardID, out _ret);

    public void GetGameDBDrawHandCardDatas(out Dictionary<int, GameDB_DrawHandCardInfo> _ret)
        => _ret = _dict_DrawHandCardInfo;
}
