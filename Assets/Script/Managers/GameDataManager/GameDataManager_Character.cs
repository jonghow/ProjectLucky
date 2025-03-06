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
    Dictionary<int, Dictionary<string, ChrActXMLInfo>> _dict_CharacterActInfo;
    Dictionary<int, Dictionary<string, ChrActXMLInfo>> _dict_EnemyActInfo;
    Dictionary<int, Dictionary<string, ChrActXMLInfo>> _dict_MealFactoryInfo;

    Dictionary<int, GameDB_CharacterInfo> _dict_CharacterInfo;
    Dictionary<int, GameDB_CharacterStat> _dict_CharacterStat;

    #region CharacterAnimXMLData

    private void InitializeCharacterPartial()
    {
        _dict_CharacterActInfo = new Dictionary<int, Dictionary<string, ChrActXMLInfo>>();
        _dict_EnemyActInfo = new Dictionary<int, Dictionary<string, ChrActXMLInfo>>();
        _dict_MealFactoryInfo = new Dictionary<int, Dictionary<string, ChrActXMLInfo>>();
        //Act Data

        _dict_CharacterInfo = new Dictionary<int, GameDB_CharacterInfo>();
        _dict_CharacterStat = new Dictionary<int, GameDB_CharacterStat>();
    }

    public async UniTask UTaskInitEntityXmlLoad()
    {
        await UTaskInitEntityCharacterXmlLoad();
        await UTaskInitEntityEnemyXmlLoad();
        await UTaskInitEntityFactoryXmlLoad();

        _isLoaded = true;
    }

    private async UniTask UTaskInitEntityCharacterXmlLoad()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        string _actionName = string.Empty;

        for (int i = 0; i < (int)CHARACTER_ACT_DATA.MAX; ++i)
        {
            _loadingFileName = $"Act_{((CHARACTER_ACT_DATA)i).ToString().ToLower()}";
            _loaded = false;

            if (!(i == 0 || i == 1 || i == 2 || i == 3 || i == 17 || i == 8 || i == 9 || i == 23)) continue;
            // 사이에 없는 캐릭터 패스

            Addressables.LoadAssetAsync<TextAsset>(_loadingFileName).Completed += (op) =>
            {
                _loaded = true;

                if (((AsyncOperationHandle<TextAsset>)op).Status == AsyncOperationStatus.Succeeded)
                {
                    // 로딩에 성공
                    var _loadedTextAsset = op.Result;
                    _xmlDoc.LoadXml(_loadedTextAsset.text);

                    XmlNode _root = _xmlDoc.DocumentElement;
                    XmlNodeList _nodes = _root.SelectNodes("ActionData");

                    foreach (XmlNode _node in _nodes)
                    {
                        _actionName = _node.Attributes["ActionName"].Value;

                        ChrActXMLInfo _info = new ChrActXMLInfo(_node);

                        if (_dict_CharacterActInfo.ContainsKey(i) == false)
                            _dict_CharacterActInfo.Add(i, new Dictionary<string, ChrActXMLInfo>());

                        if (_dict_CharacterActInfo[i].ContainsKey(_actionName))
                            EditorUtility.DisplayDialog($"Error!!", $"Character 카테고리 관련  {_actionName} 파일에 동일한 Animation Object가 존재합니다. \n 같은 애니메이션은 하나의 Animation Object로 처리해야합니다.", $"확인");

                        _dict_CharacterActInfo[i].Add(_actionName, _info);
                    }
                }
            };

            await UniTask.WaitUntil(() => _loaded == true);
        }
    }

    private async UniTask UTaskInitEntityEnemyXmlLoad()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        string _actionName = string.Empty;

        for (int i = (int)ENEMY_ACT_DATA.None; i < (int)ENEMY_ACT_DATA.MAX; ++i)
        {
            if (i == 1000) continue;

            _loadingFileName = $"Act_Enemy_{((ENEMY_ACT_DATA)i).ToString().ToLower()}";
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
                    XmlNodeList _nodes = _root.SelectNodes("ActionData");

                    foreach (XmlNode _node in _nodes)
                    {
                        _actionName = _node.Attributes["ActionName"].Value;

                        ChrActXMLInfo _info = new ChrActXMLInfo(_node);

                        if (_dict_EnemyActInfo.ContainsKey(i) == false)
                            _dict_EnemyActInfo.Add(i, new Dictionary<string, ChrActXMLInfo>());

                        if (_dict_EnemyActInfo[i].ContainsKey(_actionName))
                            EditorUtility.DisplayDialog($"Error!!", $" Enemy 카테고리 관련 {_actionName} 파일에 동일한 Animation Object가 존재합니다. \n 같은 애니메이션은 하나의 Animation Object로 처리해야합니다.", $"확인");

                        _dict_EnemyActInfo[i].Add(_actionName, _info);
                    }
                }
            };

            await UniTask.WaitUntil(() => _loaded == true);
        }
    }

    private async UniTask UTaskInitEntityFactoryXmlLoad()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        string _actionName = string.Empty;

        for (int i = 0; i < (int)MEALFACTORY_ACT_DATA.MAX; ++i)
        {
            if (i != (int)MEALFACTORY_ACT_DATA.MAX) continue;

            _loadingFileName = $"Act_{((MEALFACTORY_ACT_DATA)i).ToString().ToLower()}";
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
                    XmlNodeList _nodes = _root.SelectNodes("ActionData");

                    foreach (XmlNode _node in _nodes)
                    {
                        _actionName = _node.Attributes["ActionName"].Value;

                        ChrActXMLInfo _info = new ChrActXMLInfo(_node);

                        if (_dict_MealFactoryInfo.ContainsKey(i) == false)
                            _dict_MealFactoryInfo.Add(i, new Dictionary<string, ChrActXMLInfo>());

                        if (_dict_MealFactoryInfo[i].ContainsKey(_actionName))
                            EditorUtility.DisplayDialog($"Error!!", $" Enemy 카테고리 관련 {_actionName} 파일에 동일한 Animation Object가 존재합니다. \n 같은 애니메이션은 하나의 Animation Object로 처리해야합니다.", $"확인");

                        _dict_MealFactoryInfo[i].Add(_actionName, _info);
                    }
                }
            };

            await UniTask.WaitUntil(() => _loaded == true);
        }
    }

    private Dictionary<string, ChrActXMLInfo> GetCharacterActions(int _jobID)
    {
        if (_dict_CharacterActInfo.ContainsKey(_jobID))
            return _dict_CharacterActInfo[_jobID];

        return null;
    }
    public ChrActXMLInfo GetCharacterActionInfo(int _jobID, string _actName)
    {
        if (_dict_CharacterActInfo.ContainsKey(_jobID))
        {
            if (_dict_CharacterActInfo[_jobID].ContainsKey(_actName))
                return _dict_CharacterActInfo[_jobID][_actName];
        }

        return null;
    }
    private bool IsValidCharacterActionInfo(int _jobId, string _actName)
    {
        return GetCharacterActionInfo(_jobId, _actName) != null;
    }

    private Dictionary<string, ChrActXMLInfo> GetEnemyActions(int _jobID)
    {
        if (_dict_EnemyActInfo.ContainsKey(_jobID))
            return _dict_EnemyActInfo[_jobID];

        return null;
    }
    public ChrActXMLInfo GetEnemyActionInfo(int _jobID, string _actName)
    {
        if (_dict_EnemyActInfo.ContainsKey(_jobID))
        {
            if (_dict_EnemyActInfo[_jobID].ContainsKey(_actName))
                return _dict_EnemyActInfo[_jobID][_actName];
        }

        return null;
    }
    private bool IsValidEnemyActionInfo(int _jobId, string _actName)
    {
        return GetEnemyActionInfo(_jobId, _actName) != null;
    }

    private Dictionary<string, ChrActXMLInfo> GetMealFactoryActions(int _jobID)
    {
        if (_dict_MealFactoryInfo.ContainsKey(_jobID))
            return _dict_MealFactoryInfo[_jobID];

        return null;
    }
    public ChrActXMLInfo GetMealFactoryActionInfo(int _jobID, string _actName)
    {
        if (_dict_MealFactoryInfo.ContainsKey(_jobID))
        {
            if (_dict_MealFactoryInfo[_jobID].ContainsKey(_actName))
                return _dict_MealFactoryInfo[_jobID][_actName];
        }

        return null;
    }
    private bool IsValidMealfactoryActionInfo(int _jobId, string _actName)
    {
        return GetMealFactoryActionInfo(_jobId, _actName) != null;
    }

    public bool IsValidActionInfo(EntityDivision _eDivision, int _jobId, string _actName)
    {
        bool _IsValid = false;
        switch (_eDivision)
        {
            case EntityDivision.Player:
                _IsValid = IsValidCharacterActionInfo(_jobId, _actName);
                break;
            case EntityDivision.Enemy:
                _IsValid = IsValidEnemyActionInfo(_jobId, _actName);
                break;
            case EntityDivision.MealFactory:
                _IsValid = IsValidMealfactoryActionInfo(_jobId, _actName);
                break;
            case EntityDivision.Neutrality:
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"IsValidActionInfo", $"작업 안된 타입을 넣으시면 안됩니다. 확인해주세요. 현재 타입은 {_eDivision.ToString()} 입니다.");
                break;
            case EntityDivision.Deco:
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"IsValidActionInfo", $"작업 안된 타입을 넣으시면 안됩니다. 확인해주세요. 현재 타입은 {_eDivision.ToString()} 입니다.");
                break;
            default:
                break;
        }

        return _IsValid;
    }
    public Dictionary<string, ChrActXMLInfo> GetActionInfo(EntityDivision _eDivision, int _jobId)
    {
        Dictionary<string, ChrActXMLInfo> _dict_Ret = new Dictionary<string, ChrActXMLInfo>();

        switch (_eDivision)
        {
            case EntityDivision.Player:
                _dict_Ret = GetCharacterActions(_jobId);
                break;
            case EntityDivision.Enemy:
                _dict_Ret = GetEnemyActions(_jobId);
                break;
            case EntityDivision.MealFactory:
                _dict_Ret = GetMealFactoryActions(_jobId);
                break;
            case EntityDivision.Neutrality:
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"GetActionInfo", $"작업 안된 타입을 넣으시면 안됩니다. 확인해주세요. 현재 타입은 {_eDivision.ToString()} 입니다.");
                break;
            case EntityDivision.Deco:
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"GetActionInfo", $"작업 안된 타입을 넣으시면 안됩니다. 확인해주세요. 현재 타입은 {_eDivision.ToString()} 입니다.");
                break;
            default:
                break;
        }

        return _dict_Ret;
    }

    #endregion

    #region CharacterInfoData
    public async UniTask UTask_Load_GameDBCharacterDatas()
    {
        await UTask_Load_GameDBCharacterInfo();
        await UTask_Load_GameDBCharacterStat();
        _isLoaded = true;
    }
    

    public void GetGameDBCharacterInfo(int _characterID, out GameDB_CharacterInfo _ret) 
        => _dict_CharacterInfo.TryGetValue( _characterID, out _ret );

    public void GetGameDBCharacterInfoByGrade(EntityGrade[] _categories, out List<GameDB_CharacterInfo> _Lt_Infos)
    {
        _Lt_Infos = new List<GameDB_CharacterInfo>();

        for (int i = 0; i < _categories.Length; ++i)
        {
            foreach (var pair in _dict_CharacterInfo)
            {
                if (pair.Value._me_Grade == _categories[i])
                    _Lt_Infos.Add(pair.Value);
            }
        }
    }

    #endregion

    private async UniTask UTask_Load_GameDBCharacterInfo()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        _loadingFileName = $"GameDB_CharacterInfo";

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
                    GameDB_CharacterInfo _gameDB_CharacterInfo = new GameDB_CharacterInfo(_node);
                    int _characterID = _gameDB_CharacterInfo._mi_CharacterID;

                    if (_dict_CharacterInfo.ContainsKey(_characterID))
                    {
                        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_GameDBCharacterInfo", $"동일한 ID를 가진 CharacterID가 존재합니다.");
                        EditorApplication.isPlaying = false;
                    }

                    _dict_CharacterInfo.Add(_characterID, _gameDB_CharacterInfo);
                }
            }
        };

        await UniTask.WaitUntil(() => _loaded == true);
    }

    #region CharacterStatData
    private async UniTask UTask_Load_GameDBCharacterStat()
    {
        string _loadingFileName = string.Empty;
        XmlDocument _xmlDoc = new XmlDocument();
        bool _loaded = false;

        _loadingFileName = $"GameDB_CharacterStat";
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

                foreach (XmlNode _node in _nodes)
                {
                    GameDB_CharacterStat _gameDB_CharacterStat = new GameDB_CharacterStat(_node);
                    int _statID = _gameDB_CharacterStat._mi_StatID;

                    if (_dict_CharacterStat.ContainsKey(_statID))
                    {
                        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UTask_Load_GameDBCharacterStat", $"동일한 ID를 가진 StatID가 존재합니다.");
                        EditorApplication.isPlaying = false;
                    }

                    _dict_CharacterStat.Add(_statID, _gameDB_CharacterStat);
                }
            }

        };
        await UniTask.WaitUntil(() => _loaded == true);
    }

    public void GetGameDBCharacterStat(int _characterID, out GameDB_CharacterStat _ret)
    => _dict_CharacterStat.TryGetValue(_characterID, out _ret);
    #endregion
}
