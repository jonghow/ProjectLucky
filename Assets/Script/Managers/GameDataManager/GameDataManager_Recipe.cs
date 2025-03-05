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
    private Dictionary<string, GameDB_MealRecipe> _dict_MealRecipe;

    private void InitializeMealRecipePartial()
    {
        _dict_MealRecipe = new Dictionary<string, GameDB_MealRecipe>();
    }
    public async UniTask UTask_Load_MealRecipe()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        for (int i = 0; i < (int)DB_MealRecipe.Max; ++i)
        {
            _loadingFileName = $"GameDB_MealRecipe{((DB_MealRecipe)i).ToString()}";
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
                        GameDB_MealRecipe _gameDB_MealRecipeInfo = new GameDB_MealRecipe(_node);
                        string _recipeCombine = _gameDB_MealRecipeInfo._mStr_Recipe;

                        if (_dict_MealRecipe.ContainsKey(_recipeCombine))
                        {
                            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_StringCommon", $"동일한 ID를 가진 StringCommonID가 존재합니다.");
                            EditorApplication.isPlaying = false;
                        }

                        _dict_MealRecipe.Add(_recipeCombine, _gameDB_MealRecipeInfo);
                    }
                }
            };
            await UniTask.WaitUntil(() => _loaded == true);
        }
    }

    public bool IsContainsMealRecipe(List<Entity> _entities)
    {
        if (_entities == null || _entities.Count == 0)
            return false;

        StringBuilder _sb =new StringBuilder();

        for(int i = 0; i < _entities.Count; i++)
        {
            _sb.Append(_entities[i].CharacterID);
            _sb.Append('|');
        }

        _sb.Remove(_sb.Length - 1, 1); // 1|2|3| 이라고 할때, _sb[4] => 3부터 1개 삭제 하면 3뒤에 | 만 삭제

        if (_dict_MealRecipe.ContainsKey(_sb.ToString()))
            return true;

        return false;
    }

    public GameDB_MealRecipe GetContainsMealRecipe(List<Entity> _entities)
    {
        if (_entities == null || _entities.Count == 0)
            return null;

        StringBuilder _sb = new StringBuilder();
        GameDB_MealRecipe _recipeData = null;

        for (int i = 0; i < _entities.Count; i++)
        {
            _sb.Append(_entities[i].CharacterID);
            _sb.Append('|');
        }

        _sb.Remove(_sb.Length - 1, 1); // 1|2|3| 이라고 할때, _sb[4] => 3부터 1개 삭제 하면 3뒤에 | 만 삭제
        _dict_MealRecipe.TryGetValue(_sb.ToString(), out _recipeData);
        return _recipeData;
    }

    public bool TryGetMealRecipe(List<Entity> _entities, out GameDB_MealRecipe _ret)
    {
        bool _result = false;
        _ret = null;

        if (_entities == null || _entities.Count == 0)
            return _result;

        StringBuilder _sb = new StringBuilder();

        for (int i = 0; i < _entities.Count; i++)
        {
            _sb.Append(_entities[i].CharacterID);
            _sb.Append('|');
        }

        _sb.Remove(_sb.Length - 1, 1); // 1|2|3| 이라고 할때, _sb[4] => 3부터 1개 삭제 하면 3뒤에 | 만 삭제

        if (_dict_MealRecipe.TryGetValue(_sb.ToString(), out _ret))
            _result = true;

        return _result;
    }
}