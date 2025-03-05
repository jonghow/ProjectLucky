using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class EntityPathFinder : MonoBehaviour
{
    readonly public int[,] DIR_HV = new int[,] { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 } };  // ↑ ↓ ← →
    readonly public int[,] DIR_DIAGONAL = new int[,] { { -1, 1 }, { -1, -1 }, { 1, 1 }, { 1, -1 } };// ↖ ↙ ↗ ↘

    [SerializeField] Vector2Int _mv2l_ArrivalPoint;

    [SerializeField] List<NavigationElement> _mLt_OpenList = new List<NavigationElement>();
    [SerializeField] List<NavigationElement> _mLt_CloseList = new List<NavigationElement>();

    [SerializeField] Dictionary<Vector2Int, NavigationElement> _mDt_CloseList = new Dictionary<Vector2Int, NavigationElement>();
    [SerializeField] Dictionary<Vector2Int, NavigationElement> _mDt_OpenList = new Dictionary<Vector2Int, NavigationElement>();

    [SerializeField] List<NavigationElement> _mLt_DecideFinalPath = new List<NavigationElement>();

    [SerializeField] NavigationElement _mCurNv;
    [SerializeField] NavigationElement _mTargetNv;

    [SerializeField] Dictionary<NavigationElement, NavigationState> _mDict_NodeState = new Dictionary<NavigationElement, NavigationState>();

    [SerializeField] bool _mb_MoveDiagonal;

    //test
    public Action<List<NavigationElement>> _onCompletePathfindAfterMove; // 이동까지 하는 함수
    public Action<List<NavigationElement>> _onCompletePathfind; // 길만 찾아서 돌려주는 함수

    [SerializeField] long _ml_EntityUID; // owner UID
    public void OnInitialize(long _entityUID)
    {
        _ml_EntityUID = _entityUID;
        _mb_MoveDiagonal = true;
        RegistNvElement();
    }

    public void OnDestroy()
    {
        _onCompletePathfindAfterMove = null; // Release Callback
        _onCompletePathfind = null;  // Release Callback
    }
    /// <summary>
    /// 각 Entity 당 사용하는 Navigation Status를 등록합니다.
    /// A* 를 이용한 길찾기를 정확하게 하기 위해선 필수적입니다.
    /// </summary>
    private void RegistNvElement()
    {
        MapManager.GetInstance().GetNavigationElements(out var Elements);

        foreach (var Element in Elements)
        {
            NavigationElement _nvElement = Element.Value;
            _mDict_NodeState.Add(_nvElement, new NavigationState());
        }
    }
    /// <summary>
    /// 풀링 수집 전, NvElement의 State를 모두 초기화 합니다. 
    /// 재 사용해야 하기 때문에 삭제하지는 않습니다.
    /// </summary>
    public void CleanNvElementState()
    {
        foreach (var pair in _mDict_NodeState)
        {
            pair.Value.OnInitialize();
        }
    }
    public void SetNodeState(NavigationElement node, int gCost, int hCost, NavigationElement parent)
    {
        if (_mDict_NodeState.ContainsKey(node))
        {
            _mDict_NodeState[node]._mi_GCost = gCost;
            _mDict_NodeState[node]._mi_HCost = hCost;
            _mDict_NodeState[node]._m_Parent = parent;
        }
    }
    private int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        if (_mb_MoveDiagonal)
            return 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy); // Diagonal weight = 14, straight = 10
        else
            return 10 * (dx + dy);
    }

    /// <summary>
    /// A* 알고리즘 기반으로 길찾기를 실시합니다. 
    /// 인덱스는 실제 값이 아닌, NavagationElement의 인덱스 입니다.
    /// </summary>
    /// <param name="_startingPoint">시작 인덱스 입니다.</param>
    /// <param name="_arrivalPoint">도착 인덱스 입니다.</param>
    public void ExecutePathFindingAfterMoveByAStar(Vector2Int _startingPoint, Vector2Int _arrivalPoint)
    {
        if (_mv2l_ArrivalPoint != _arrivalPoint)
            _mv2l_ArrivalPoint = _arrivalPoint;

        MapManager.GetInstance().SelectNavi(out var _startNv, _startingPoint);
        MapManager.GetInstance().SelectNavi(out var _targetNv, _mv2l_ArrivalPoint);

        ClearMoveList();

        _mCurNv = _startNv;
        _mTargetNv = _targetNv;

        _mDict_NodeState[_mCurNv]._mi_GCost = 0;
        _mDict_NodeState[_mCurNv]._mi_HCost = GetDistance(_startingPoint, _mv2l_ArrivalPoint);

        _mLt_OpenList.Add(_mCurNv);
        // 0 : OpenLt 넣어준다. 다시 찾을 필요 없도록

        // 내 현재 기준
        while (_mLt_OpenList.Count > 0)
        {
            // 오픈리스트 찾기
            _mCurNv = GetLowestCostNode();

            if (_mCurNv == _mTargetNv)
            {
                _mLt_CloseList.Add(_mCurNv);

                // Root Reverse
                ReverseTracePath();
                _onCompletePathfindAfterMove.Invoke(_mLt_DecideFinalPath);
                return;
            }

            _mLt_OpenList.Remove(_mCurNv);
            _mLt_CloseList.Add(_mCurNv);

            ExploreNeighbors();
        }
    }

    public void ExecutePathfindingDictAfterMoveByAStar(Vector2Int _startingPoint, Vector2Int _arrivalPoint)
    {
        if (_mv2l_ArrivalPoint != _arrivalPoint)
            _mv2l_ArrivalPoint = _arrivalPoint;

        MapManager.GetInstance().SelectNavi(out var _startNv, _startingPoint);
        MapManager.GetInstance().SelectNavi(out var _targetNv, _mv2l_ArrivalPoint);

        ClearMoveList();

        _mCurNv = _startNv;
        _mTargetNv = _targetNv;

        _mDict_NodeState[_mCurNv]._mi_GCost = 0;
        _mDict_NodeState[_mCurNv]._mi_HCost = GetDistance(_startingPoint, _mv2l_ArrivalPoint);

        _mDt_OpenList.Add(_mCurNv._mv2_Index, _mCurNv);
        // 0 : OpenLt 넣어준다. 다시 찾을 필요 없도록

        // 내 현재 기준
        while (_mDt_OpenList.Count > 0)
        {
            // 오픈리스트 찾기
            _mCurNv = GetDtLowestCostNode();

            if (_mCurNv == _mTargetNv)
            {
                _mDt_CloseList.Add(_mCurNv._mv2_Index, _mCurNv);

                ReverseTracePath();
                _onCompletePathfindAfterMove.Invoke(_mLt_DecideFinalPath);
                return;
            }

            _mDt_OpenList.Remove(_mCurNv._mv2_Index);
            _mDt_CloseList.Add(_mCurNv._mv2_Index, _mCurNv);

            ExploreDictNeighbors();
        }
    }

    public void ReverseTracePath()
    {
        NavigationState _curNvState = _mDict_NodeState[_mCurNv];
        NavigationElement _curNvElement = _mCurNv;

        while (_curNvState._m_Parent != null)
        {
            _mLt_DecideFinalPath.Add(_curNvElement);

            _curNvElement = _curNvState._m_Parent;
            _curNvState = _mDict_NodeState[_curNvElement];
        }

        _mLt_DecideFinalPath.Reverse();
    }

    public void ExecutePathfindingByAStar(Vector2Int _startingPoint, Vector2Int _arrivalPoint)
    {
        if (_mv2l_ArrivalPoint != _arrivalPoint)
            _mv2l_ArrivalPoint = _arrivalPoint;

        MapManager.GetInstance().SelectNavi(out var _startNv, _startingPoint);
        MapManager.GetInstance().SelectNavi(out var _targetNv, _mv2l_ArrivalPoint);

        ClearMoveList();

        _mCurNv = _startNv;
        _mTargetNv = _targetNv;

        _mDict_NodeState[_mCurNv]._mi_GCost = 0;
        _mDict_NodeState[_mCurNv]._mi_HCost = GetDistance(_startingPoint, _mv2l_ArrivalPoint);

        _mLt_OpenList.Add(_mCurNv);
        // 0 : OpenLt 넣어준다. 다시 찾을 필요 없도록

        // 내 현재 기준
        while (_mLt_OpenList.Count > 0)
        {
            // 오픈리스트 찾기
            _mCurNv = GetLowestCostNode();

            if (_mCurNv == _mTargetNv)
            {
                _mLt_CloseList.Add(_mCurNv);

                ReverseTracePath();
                _onCompletePathfind.Invoke(_mLt_DecideFinalPath);
                return;
            }

            _mLt_OpenList.Remove(_mCurNv);
            _mLt_CloseList.Add(_mCurNv);

            ExploreNeighbors();
        }
    }

    public void ExecutePathfindingDictByAStar(Vector2Int _startingPoint, Vector2Int _arrivalPoint)
    {
        if (_mv2l_ArrivalPoint != _arrivalPoint)
            _mv2l_ArrivalPoint = _arrivalPoint;

        MapManager.GetInstance().SelectNavi(out var _startNv, _startingPoint);
        MapManager.GetInstance().SelectNavi(out var _targetNv, _mv2l_ArrivalPoint);

        ClearMoveList();

        _mCurNv = _startNv;
        _mTargetNv = _targetNv;

        _mDict_NodeState[_mCurNv]._mi_GCost = 0;
        _mDict_NodeState[_mCurNv]._mi_HCost = GetDistance(_startingPoint, _mv2l_ArrivalPoint);

        _mDt_OpenList.Add(_mCurNv._mv2_Index, _mCurNv);
        // 0 : OpenLt 넣어준다. 다시 찾을 필요 없도록

        // 내 현재 기준
        while (_mDt_OpenList.Count > 0)
        {
            // 오픈리스트 찾기
            _mCurNv = GetDtLowestCostNode();

            if (_mCurNv == _mTargetNv)
            {
                if (_mDt_CloseList.ContainsKey(_mCurNv._mv2_Index))
                    return;

                _mDt_CloseList.Add(_mCurNv._mv2_Index, _mCurNv);

                ReverseTracePath();
                _onCompletePathfind?.Invoke(_mLt_DecideFinalPath);
                return;
            }

            _mDt_OpenList.Remove(_mCurNv._mv2_Index);
            _mDt_CloseList.Add(_mCurNv._mv2_Index, _mCurNv);

            ExploreDictNeighbors();
        }
    }

    private NavigationElement GetLowestCostNode()
    {
        return _mLt_OpenList
            .OrderBy(node => _mDict_NodeState[node].FCost)
            .ThenBy(node => _mDict_NodeState[node]._mi_HCost)
            .First();
    }
    private NavigationElement GetDtLowestCostNode()
    {
        //return _mDt_OpenList
        //    .OrderBy(node => _mDict_NodeState[node.Value].FCost)
        //    .ThenBy(node => _mDict_NodeState[node.Value]._mi_HCost)
        //    .First().Value;

        return _mDt_OpenList.Aggregate((best, next) =>
        _mDict_NodeState[next.Value].FCost < _mDict_NodeState[best.Value].FCost ||
        (_mDict_NodeState[next.Value].FCost == _mDict_NodeState[best.Value].FCost &&
         _mDict_NodeState[next.Value]._mi_HCost < _mDict_NodeState[best.Value]._mi_HCost)
        ? next : best).Value;
    }


    private void ExploreNeighbors()
    {
        int[,] combined = GetCombinedDir();

        for (int i = 0; i < combined.GetLength(0); ++i)
        {
            int _curPosX = _mCurNv._mv2_Index.x;
            int _curPosY = _mCurNv._mv2_Index.y;

            int _calcedPosX = _curPosX + combined[i, 0];
            int _calcedPosY = _curPosY + combined[i, 1];

            Vector2Int _vExplorePos = new Vector2Int(_calcedPosX, _calcedPosY);

            if (!MapManager.GetInstance().IsValidPos(_vExplorePos)) continue;

            MapManager.GetInstance().SelectNavi(out var neighborElement, _vExplorePos);

            if (neighborElement == null)
                continue;

            if (neighborElement._mb_IsEnable == false)
                continue;

            if (_mLt_CloseList.Contains(neighborElement))
                continue;

            if (_mLt_OpenList.Contains(neighborElement))
                continue;

            Vector2Int _vCurPos = new Vector2Int(_curPosX, _curPosY);
            int _newGCost = _mDict_NodeState[_mCurNv]._mi_GCost + GetDistance(_vCurPos, _vExplorePos);

            if (_newGCost < _mDict_NodeState[neighborElement]._mi_GCost)
            {
                SetNodeState(neighborElement, _newGCost, GetDistance(_vExplorePos, _mv2l_ArrivalPoint), _mCurNv);
                //NavigationState neightborState = _mDict_NodeState[neighborElement];
                //neightborState._mi_GCost = _newGCost;
                //neightborState._mi_HCost = GetDistance(_vExplorePos, _mv2l_TargetPos);
                //neightborState._m_Parent = _mCurNv;

                if (!_mLt_OpenList.Contains(neighborElement))
                    _mLt_OpenList.Add(neighborElement);
            }
        }
    }

    private void ExploreDictNeighbors()
    {
        int[,] combined = GetCombinedDir();

        for (int i = 0; i < combined.GetLength(0); ++i)
        {
            int _curPosX = _mCurNv._mv2_Index.x;
            int _curPosY = _mCurNv._mv2_Index.y;

            int _calcedPosX = _curPosX + combined[i, 0];
            int _calcedPosY = _curPosY + combined[i, 1];

            Vector2Int _vExplorePos = new Vector2Int(_calcedPosX, _calcedPosY);

            if (!MapManager.GetInstance().IsValidPos(_vExplorePos)) continue;

            MapManager.GetInstance().SelectNavi(out var neighborElement, _vExplorePos);

            if (neighborElement == null)
                continue;

            if (neighborElement._mb_IsEnable == false)
                continue;

            if (_mDt_OpenList.ContainsKey(neighborElement._mv2_Index))
                continue;

            if (_mDt_CloseList.ContainsKey(neighborElement._mv2_Index))
                continue;

            Vector2Int _vCurPos = new Vector2Int(_curPosX, _curPosY);
            int _newGCost = _mDict_NodeState[_mCurNv]._mi_GCost + GetDistance(_vCurPos, _vExplorePos);

            if (_newGCost < _mDict_NodeState[neighborElement]._mi_GCost)
            {
                SetNodeState(neighborElement, _newGCost, GetDistance(_vExplorePos, _mv2l_ArrivalPoint), _mCurNv);
                //NavigationState neightborState = _mDict_NodeState[neighborElement];
                //neightborState._mi_GCost = _newGCost;
                //neightborState._mi_HCost = GetDistance(_vExplorePos, _mv2l_TargetPos);
                //neightborState._m_Parent = _mCurNv;

                if (!_mDt_OpenList.ContainsKey(neighborElement._mv2_Index))
                    _mDt_OpenList.Add(neighborElement._mv2_Index,neighborElement);
            }
        }
    }

    private int[,] GetCombinedDir()
    {
        int[,] _ret = (_mb_MoveDiagonal == true) ? new int[DIR_DIAGONAL.GetLength(0) + DIR_HV.GetLength(0), 2] : DIR_HV;

        if (_mb_MoveDiagonal)
        {
            // ↖ ↙ ↗ ↘
            for (int i = 0; i < DIR_DIAGONAL.GetLength(0); ++i)
            {
                _ret[i, 0] = DIR_DIAGONAL[i, 0];
                _ret[i, 1] = DIR_DIAGONAL[i, 1];
            }

            // ↑ ↓ ← →
            for (int i = 0; i < DIR_HV.GetLength(0); ++i)
            {
                _ret[DIR_DIAGONAL.GetLength(0) + i, 0] = DIR_HV[i, 0];
                _ret[DIR_DIAGONAL.GetLength(0) + i, 1] = DIR_HV[i, 1];
            }
        }

        return _ret;
    }

    public void ClearMoveList()
    {
        _mLt_OpenList.Clear();
        _mLt_CloseList.Clear();
        _mLt_DecideFinalPath.Clear();

        _mDt_CloseList.Clear();
        _mDt_OpenList.Clear();  
    }
    private void OnDrawGizmos()
    {
        if (_mLt_DecideFinalPath.Count == 0) return;

        for (int i = 0; i < _mLt_DecideFinalPath.Count - 1; ++i)
        {
            Gizmos.DrawLine(_mLt_DecideFinalPath[i]._mv3_Pos, _mLt_DecideFinalPath[i + 1]._mv3_Pos);
        }
    }

    public Vector2Int GetCurrentIndex() => _mCurNv != null ? _mCurNv._mv2_Index : Vector2Int.zero;

    /// <summary>
    /// 드래그 이후에 강제 세팅
    /// </summary>
    /// <param name="_navigationElement"></param>
    public void SetNavigationElement(NavigationElement _navigationElement)
    {
        _mCurNv = _navigationElement;

        //UnityLogger.GetInstance().Log($"Selected Entity UID : {_ml_EntityUID}");
        //UnityLogger.GetInstance().Log($"Setting NavElement Index {GetCurrentIndex()}");
    }
}
