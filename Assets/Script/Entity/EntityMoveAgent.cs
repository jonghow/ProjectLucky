using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;

public class EntityMoveAgent : MonoBehaviour
{
    [SerializeField] EntityPathFinder _mEntityPathFinder;

    [SerializeField] bool _mbProcPathfind;
    private float _mf_MoveSpeed;
    public float MoveSpeed
    {
        get
        {
            if (_mf_MoveSpeed <= 0f)
            {
                Entity _entity;
                EntityManager.GetInstance().GetEntity(_ml_EntityUID, out _entity);
                _mf_MoveSpeed = _entity.Info.MoveSpeed;
            }
            return _mf_MoveSpeed;
        }
    }

    private CancellationTokenSource _cancellationToken;
    [SerializeField] Vector2Int _mv2I_StartingPoint;

    [SerializeField] long _ml_EntityUID;
    [SerializeField] EntityContoller _mCache_Controller;

    List<NavigationElement> _mLt_CloseList; // ������ ���
    private bool _isProcMove; // �̵����ΰ�?

    public void OnInitialize(long _entityUID, EntityContoller _contoller)
    {
        _ml_EntityUID = _entityUID;
        _mCache_Controller = _contoller;
        _isProcMove = false;
        ClearUnitaskToken();

        _mEntityPathFinder.OnInitialize(_entityUID);
        _mEntityPathFinder._onCompletePathfindAfterMove += OnMoveByAStar;

        _mEntityPathFinder._onCompletePathfind += SetCloseList;
    }
    public void CommandMove(Vector2Int _arrivalPoint)
    {
        _mEntityPathFinder.CleanNvElementState();
        _mEntityPathFinder.ExecutePathfindingDictAfterMoveByAStar(_mv2I_StartingPoint, _arrivalPoint);
    }
    public void SetCloseList(List<NavigationElement> _Lt_CloseList)
    {
        _mLt_CloseList = _Lt_CloseList;
    }
    public void OnMoveByAStar(List<NavigationElement> _Lt_CloseList)
    {
        Debug.Log($"OnMoveByAStar!!");

        if(_Lt_CloseList.Count > 0)
        {
            Debug.Log($"Finish PathFinding!!");
            ClearUnitaskToken();
            _ = UTask_MovePath(_Lt_CloseList);
        }
    }
    private void ClearUnitaskToken()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
        }

        _cancellationToken = new CancellationTokenSource();
    }


    async UniTaskVoid UTask_MovePath(List<NavigationElement> _Lt_CloseList)
    {
        int _index = 0;

        if (_cancellationToken == null) return;

        CancellationToken token = _cancellationToken.Token; //  ���� ��ū�� �����Ͽ� ���

        while (_index < _Lt_CloseList.Count)
        {
            if (token.IsCancellationRequested)
                break;

            if (_mCache_Controller == null)
                break;

            Vector3 _v3_CloseElementPos = _Lt_CloseList[_index]._mv3_Pos;
            _mv2I_StartingPoint = _Lt_CloseList[_index]._mv2_Index; // ���� �� ��ġ

            Vector3 P0 = _mCache_Controller.Pos3D;
            Vector3 Dir = Vector3.Normalize(_v3_CloseElementPos - _mCache_Controller.Pos3D);
            Vector3 AT = Dir * MoveSpeed * Time.deltaTime;

            Vector3 P1 = P0 + AT;
            _mCache_Controller.Pos3D = P1;

            if (_index < _Lt_CloseList.Count-1)
                ProcFaceDirection(_Lt_CloseList[_index]._mv3_Pos, _Lt_CloseList[_index+1]._mv3_Pos);

            if (Vector3.SqrMagnitude(_v3_CloseElementPos - P0) < 0.001f)
            {
                ++_index;
            }

            await UniTask.Yield(_cancellationToken.Token);
        }

        _mEntityPathFinder.ClearMoveList();
    }


    #region Relative Battle Moving
    /// <summary>
    /// ���� ���� �̵� �Լ� �Դϴ�. 
    /// �� �Լ��� ���� ���ÿ����� ���Ǹ�, �ѹ��� �� ���� �ʽ��ϴ�.
    /// </summary>
    /// <param name="_arrivalPoint">���� ���� �Դϴ�.</param>
    public void CommandBattleMove(Vector2Int _arrivalPoint)
    {
        _isProcMove = false; 
        _mEntityPathFinder.CleanNvElementState();
        //_mEntityPathFinder.ExecutePathfindingByAStar(_mv2I_StartingPoint, _arrivalPoint);

        _mEntityPathFinder.ExecutePathfindingDictByAStar(_mv2I_StartingPoint, _arrivalPoint);

        if (_mLt_CloseList.Count > 0)
        {
            _isProcMove = true;
            _cancellationToken = new CancellationTokenSource();

            _ = UTask_BattleMove();
        }
        // ������� �н����ε�
    }
    async UniTaskVoid UTask_BattleMove()
    {
        int _index = 0;

        if (_cancellationToken == null) return;

        CancellationToken token = _cancellationToken.Token; //  ���� ��ū�� �����Ͽ� ���

        while (_index < _mLt_CloseList.Count)
        {
            if (token.IsCancellationRequested)
                break;

            _isProcMove = true;

            Vector3 _v3_CloseElementPos = _mLt_CloseList[_index]._mv3_Pos;
            _mv2I_StartingPoint = _mLt_CloseList[_index]._mv2_Index; // ���� �� ��ġ

            Vector3 P0 = _mCache_Controller.Pos3D;
            Vector3 Dir = Vector3.Normalize(_v3_CloseElementPos - _mCache_Controller.Pos3D);
            Vector3 AT = Dir * MoveSpeed * Time.deltaTime;

            Vector3 P1 = P0 + AT;
            _mCache_Controller.Pos3D = P1;

            if (Vector3.SqrMagnitude(_v3_CloseElementPos - P0) < 0.001f)
            {
                _mCache_Controller.Pos3D = _v3_CloseElementPos;
                ++_index;

                if (_index < _mLt_CloseList.Count)
                    ProcFaceDirection(_mLt_CloseList[_index - 1]._mv3_Pos, _mLt_CloseList[_index]._mv3_Pos);

                if (_index > 1)
                {
                    StopBattleMoveImmediate();
                    break;
                }
            }

            await UniTask.Yield(_cancellationToken.Token);
        }

        if(_index == _mLt_CloseList.Count -1)
            _mEntityPathFinder.ClearMoveList();
    }
    /// <summary>
    /// �̵� ���� �Լ� ��� �����Դϴ�.
    /// </summary>
    public void StopBattleMoveImmediate()
    {
        if(_cancellationToken != null)
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _isProcMove = false;
        }

        _cancellationToken = new CancellationTokenSource();
    }

    #endregion

    public void SetStartPoint(Vector2Int _startPoint) { _mv2I_StartingPoint = _startPoint; }
    public void GetPathFinder(out EntityPathFinder _outPathfinder)
    {
        _outPathfinder = _mEntityPathFinder;
    }
    public void SetPathFinder(EntityPathFinder _pathFinder) { _mEntityPathFinder = _pathFinder; }
    public void ProcFaceDirection(Vector2 _curPos, Vector2 _nextPos)
    {
        Vector3 _dir = (_nextPos - _curPos).normalized;
        Vector3 _up = _mCache_Controller.UpVector;

        Vector3 _cross = Vector3.Cross(_up, _dir);

        if(_cross.z < -0.05f)
        {
            if(_mCache_Controller.GetNowFaceDirection())
                _mCache_Controller.OnFlipFaceDirection();
        }
        else if (_cross.z > 0.05f)
        {
            if (!_mCache_Controller.GetNowFaceDirection())
                _mCache_Controller.OnFlipFaceDirection();
        }
        else
        {
            // ���� ������ �κ��� ���� ���̶�� ����Ǿ� ������ �κ� �������.
        }
    }
    public bool IsProcMoveAgent() => _isProcMove == true ? true : false;

    public void SetProcMoveAgent(bool _procMove)
    {
        _isProcMove = _procMove;
    }
}
