using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MapManager
{
    private static MapManager Instance;
    /// <summary>
    /// NavitionSystem Movable Check를 위한 매니저입니다.
    /// </summary>
    /// <returns>Manager Object</returns>
    public static MapManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new MapManager();
            Instance.OnInitialize();
        }

        return Instance;
    }

    public void OnInitialize()
    {
        if(_mDict_NvElement == null)
            _mDict_NvElement = new Dictionary<Vector2Int, NavigationElement>();

        if (_mDict_DisableNvElement == null)
            _mDict_DisableNvElement = new Dictionary<Vector2Int, long>();
    }

    /// <summary>
    /// Stage ID로 MapNavigation을 불러올 예정입니다.
    /// </summary>
    /// <param name="_stageID"></param>
    public async UniTask LoadStage(int _stageID)
    {
        bool _isLoaded = false;

        ClearMapNavigation();

        if (_mDict_DisableNvElement == null) _mDict_DisableNvElement = new Dictionary<Vector2Int, long>();

        ResourceManager.GetInstance().GetResource(ResourceType.MapNaviData, _stageID, true, (_gObj_NaviXMLData) => {
            UnityEngine.Object _xmlObject = _gObj_NaviXMLData as UnityEngine.Object;
            var _mapText = _xmlObject as TextAsset;

            if (string.IsNullOrEmpty(_mapText.text))
            {
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadStage", $" Loaded Stage Idx : {_stageID} Navi File Empty");
                return;
            }

            NavigationElementListWrap _loadedNaviData = XMLUtility.Deserialize<NavigationElementListWrap>(_mapText.text);

            for (int i = 0; i < _loadedNaviData.List.Count; ++i)
            {
                var _element = _loadedNaviData.List[i];
                Vector2Int _index = _element._mv2_Index;

                if (_mDict_NvElement.ContainsKey(_index) == true)
                {
                    UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadStage", $"stage ID {_stageID}, 동일한 네비게이션 그리드가 있습니다. 게임 종료하겠습니다. 확인해주세요!");
                    Application.Quit();
                }

                _mDict_NvElement.Add(_index, _element);
            }

            _isLoaded = true;
        });

        await UniTask.WaitUntil(() => _isLoaded == true);
    }

    public bool IsMoveAbleIndex(Vector2Int checkPos){
        return !_mDict_DisableNvElement.ContainsKey(checkPos);
    }

    public void SelectNavi(out NavigationElement retNv, Vector2Int pos){
        int posX = pos.x;
        int posY = pos.y;

        retNv = null;

        if (!IsMoveAbleIndex(pos)) return;

        //PreoccupyNavi(pos, 0); // 임시로 사용중인 uid는 삭제
        //PreoccupyNavi(new Vector2Int(0,1), 0);
        //PreoccupyNavi(new Vector2Int(0,2), 0);
        //PreoccupyNavi(new Vector2Int(0,3), 0);

        NavigationElement _selectedNavi = null;
        _mDict_NvElement.TryGetValue(pos, out _selectedNavi);

        retNv = _selectedNavi;
    }

    public void PreoccupyNavi(Vector2Int pos , long _uid){
        if (!_mDict_DisableNvElement.ContainsKey(pos))
            _mDict_DisableNvElement.Add(pos, _uid);
    }

    public void ReleaseNavi(Vector2Int releasePos){
        _mDict_DisableNvElement.Remove(releasePos);
    }

    public void GetNavigationElements(out Dictionary<Vector2Int, NavigationElement> _dicNavigationElements){
        _dicNavigationElements = _mDict_NvElement;
    }
    /// <summary>
    /// 인덱스 영역 검사로 유효한지 판단
    /// </summary>
    /// <param name="index">검사할 인덱스</param>
    /// <returns>결과 : S : True , F : False </returns>
    public bool IsValidPos(Vector2Int index){
        int indexX = index.x;
        int indexY = index.y;

        if(!_mDict_NvElement.ContainsKey(index))
            return false;

        return true;
    }
    public bool IsEnablePos(Vector2Int index)
    {
        int indexX = index.x;
        int indexY = index.y;

        if (!_mDict_NvElement.TryGetValue(index, out var _element))
            return false;

        if (!_element._mb_IsEnable)
            return false;

        return true;
    }
    public void QuickBatchNavigation(Vector2Int _v2_batchIndex)
    {
        //OnAllocNavMemory(_v2_batchIndex);

        float defScaleX = GlobalGameDataSpace.Defines.DefaultScaleX;
        float defScaleY = GlobalGameDataSpace.Defines.DefaultScaleY;

        float _correctionValueY = ((_v2_batchIndex.y - 1) / 2);
        float _correctionValueX = ((_v2_batchIndex.x - 1) / 2);

        for (int i = 0; i < _v2_batchIndex.y; ++i) // Y 
        {
            for (int j = 0; j < _v2_batchIndex.x; ++j) // X
            {
                float yPos = (i - _correctionValueY) * defScaleY;
                float xPos = (j - _correctionValueX) * defScaleX;

                Vector3 _setPos = new Vector3(xPos, yPos, 0f);
                Vector2Int _setIndex = new Vector2Int(i, j);
                _mDict_NvElement.Add(_setIndex, new NavigationElement(_setIndex, _setPos));
                //_mArr_NvElement[i, j] = (new NavigationElement(new Vector2Int(i, j), _setPos));
            }
        }
    }
    public void ClearMapNavigation()
    {
        _mDict_NvElement.Clear();
        GC.Collect();
    }
    public void GetMoveableCandidate(Entity _starter, Vector2Int _point, Vector2Int _removeIndex, out List<NavigationElement> _list) // AttackRange
    {
        _list = new List<NavigationElement>();

        NavigationElement _standardElement = null;
        float _attackRange = _starter.Info.AttackRange;

        if(_mDict_NvElement.TryGetValue(_point, out _standardElement))
        {
            // 이제 후보를 가져온다.
            Dictionary<Vector2Int, NavigationElement> _mFindNv = new Dictionary<Vector2Int, NavigationElement>(); // 복사

            foreach(var _element in _mDict_NvElement)
            {
                if (_element.Value._mb_IsEnable == false) continue;

                if (!MathUtility.CheckOverV2SqrMagnitudeDistance(_standardElement._mv3_Pos, _element.Value._mv3_Pos, _attackRange))
                {
                    // 범위 안
                    _mFindNv.Add(_element.Key, _element.Value);
                }
            }

            if(_mFindNv.ContainsKey(_removeIndex))
                _mFindNv.Remove(_removeIndex);
            // 포인터의 위치는 배제한다. 이미 자리에 있는곳은 이동하지 않음

            foreach(var _element in _mFindNv)
                _list.Add(_element.Value);

            _list.Sort((item1, item2) =>
            {
                float _distEntityToItem1 = Vector3.SqrMagnitude(_starter.Controller.Pos3D - item1._mv3_Pos);
                float _distEntityToItem2 = Vector3.SqrMagnitude(_starter.Controller.Pos3D - item2._mv3_Pos);

                return _distEntityToItem1.CompareTo(_distEntityToItem2);
            });
        }
    }
    public NavigationElement GetMyNavigationByPos3D(Vector3 _v3_Pos)
    {
        float _restrictSize = 1.5f;
        NavigationElement _result = null;

        foreach (var elementPair in _mDict_NvElement)
        {
            NavigationElement _element = elementPair.Value;

            if (MathUtility.CheckOverV2SqrMagnitudeDistance(_v3_Pos, _element._mv3_Pos, _restrictSize))
                continue;

            Vector2 _point = _v3_Pos;
            Vector2 _center = _element._mv3_Pos;
            List<Vector2> _Lt_Vertice;
            MathUtility.GetNavigationVertice(_center, out _Lt_Vertice);

            if (!MathUtility.CheckInVertice(_point, _Lt_Vertice))
                continue;

            _result = _element;
        }

        return _result;
    }

    public void GetSpawnableCandidate(Vector2Int _point, Vector2Int _removeIndex, out List<NavigationElement> _list)
    {
        _list = new List<NavigationElement>();
        float range = 1.3f;
        NavigationElement _standardElement = null;
        if (_mDict_NvElement.TryGetValue(_point, out _standardElement))
        {
            // 이제 후보를 가져온다.
            Dictionary<Vector2Int, NavigationElement> _mFindNv = new Dictionary<Vector2Int, NavigationElement>(); // 복사

            foreach (var _element in _mDict_NvElement)
            {
                if (_element.Value._mb_IsEnable == false) continue;

                if (!MathUtility.CheckOverV2SqrMagnitudeDistance(_standardElement._mv3_Pos, _element.Value._mv3_Pos, range))
                {
                    // 범위 안
                    _mFindNv.Add(_element.Key, _element.Value);
                }
            }

            if (_mFindNv.ContainsKey(_removeIndex))
                _mFindNv.Remove(_removeIndex);
            // 포인터의 위치는 배제한다. 이미 자리에 있는곳은 이동하지 않음

            foreach (var _element in _mFindNv)
                _list.Add(_element.Value);

            _list.Sort((item1, item2) =>
            {
                float _distEntityToItem1 = Vector3.SqrMagnitude(_standardElement._mv3_Pos - item1._mv3_Pos);
                float _distEntityToItem2 = Vector3.SqrMagnitude(_standardElement._mv3_Pos - item2._mv3_Pos);

                return _distEntityToItem1.CompareTo(_distEntityToItem2);
            });
        }
    }

    public void SetEnableNavigationElement(Vector2Int _stardardPos, GridDirectionGroup[] _buildDirectionGroup)
    {
        for (int i = 0; i < _buildDirectionGroup.Length; ++i)
        {
            Vector2Int _dirConvert = BuildGridHelper.ConvertDirToNavIndex(_buildDirectionGroup[i]);
            Vector2Int _calcedIndex = _stardardPos + _dirConvert;

            if(_mDict_NvElement.TryGetValue(_calcedIndex, out var element))
                element._mb_IsEnable = true;
        }
    }
    public void SetDisableNavigationElement(Vector2Int _stardardPos, GridDirectionGroup[] _buildDirectionGroup)
    {
        for (int i = 0; i < _buildDirectionGroup.Length; ++i)
        {
            Vector2Int _dirConvert = BuildGridHelper.ConvertDirToNavIndex(_buildDirectionGroup[i]);
            Vector2Int _calcedIndex = _stardardPos + _dirConvert;

            if (_mDict_NvElement.TryGetValue(_calcedIndex, out var element))
                element._mb_IsEnable = false;
        }
    }

    private Dictionary<Vector2Int, NavigationElement> _mDict_NvElement;
    private Dictionary<Vector2Int, long> _mDict_DisableNvElement;

    private NavigationElement _selectedElement; // Editor에서 사용
    public NavigationElement SelectedElement
    {
        get { return _selectedElement; }
        set { _selectedElement = value; }
    }
}
