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
using System.Text;

public partial class GameDataManager
{
    private Dictionary<int, GameDB_MealKitInfo> _dict_MealKitInfo;
    private void InitializeMealKitPartial()
    {
        _dict_MealKitInfo = new Dictionary<int, GameDB_MealKitInfo>();
    }
    public async UniTask UTask_Load_MealKitInfo()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        for (int i = 0; i < (int)DB_MealKit.Max; ++i)
        {
            _loadingFileName = $"GameDB_MealKit{((DB_MealKit)i).ToString()}";
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
                        GameDB_MealKitInfo _gameDB_MealKitInfo = new GameDB_MealKitInfo(_node);
                        int _mealkitID= _gameDB_MealKitInfo._mi_ID;

                        if (_dict_MealKitInfo.ContainsKey(_mealkitID))
                        {
                            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_MealKitInfo", $"동일한 ID를 가진 _gameDB_MealKitInfo._mi_ID가 존재합니다.");
                            EditorApplication.isPlaying = false;
                        }

                        _dict_MealKitInfo.Add(_mealkitID, _gameDB_MealKitInfo);
                    }
                }
            };
            await UniTask.WaitUntil(() => _loaded == true);
        }
    }

    public GameDB_MealKitInfo GetMealKitInfo(int _mealKitID)
    {
        _dict_MealKitInfo.TryGetValue(_mealKitID, out var _ret);
        return _ret;
    }
}