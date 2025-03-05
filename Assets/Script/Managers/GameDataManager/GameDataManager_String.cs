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
    private Dictionary<int, GameDB_StringCommon> _dict_StringCommon;

    private void InitializeStringPartial()
    {
        _dict_StringCommon = new Dictionary<int, GameDB_StringCommon>();
    }

    #region StringCommon
    public async UniTask UTask_Load_StringCommon()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        for (int i = 0; i < (int)DB_Enum_String.Max; ++i)
        {
            _loadingFileName = $"GameDB_String{((DB_Enum_String)i).ToString()}";
            _loaded = false;

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

                    foreach(XmlNode _node in _nodes)
                    {
                        GameDB_StringCommon _gameDB_StringCommon = new GameDB_StringCommon(_node);
                        int _characterID = _gameDB_StringCommon._mi_ID;

                        if (_dict_StringCommon.ContainsKey(_characterID))
                        {
                            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_StringCommon", $"동일한 ID를 가진 StringCommonID가 존재합니다.");
                            EditorApplication.isPlaying = false;
                        }

                        _dict_StringCommon.Add(_characterID, _gameDB_StringCommon);
                    }
                }
            };

            await UniTask.WaitUntil(() => _loaded == true);
        }
    }
        #endregion

    public string GetCommonString(int _stringID)
    {
        _dict_StringCommon.TryGetValue(_stringID, out var _ret);
        return _ret._mStr_Common;
    }



}