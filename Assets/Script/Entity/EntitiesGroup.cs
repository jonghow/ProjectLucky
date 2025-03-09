using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public class EntitiesGroup : MonoBehaviour
{
    int _mi_ID;
    long _ml_uniqueID;
    Vector2Int _mv2_NavigationPos;
    public Vector2Int NvPos { get { return _mv2_NavigationPos; } set { _mv2_NavigationPos = value; }}
    public long UniqueID { get { return _ml_uniqueID; } set { _ml_uniqueID = value; } }
    public int ID { get { return _mi_ID; } set { _mi_ID = value; } }
    public int Count { get { return _m_Entities.Count; } }

    Transform _m_Tr_WorldObject;
    public Vector3 Pos3D
    {
        get 
        {
            if(_m_Tr_WorldObject == null)
                _m_Tr_WorldObject = GetComponent<Transform>();

            return _m_Tr_WorldObject == null ? Vector3.zero :  _m_Tr_WorldObject.position;
        }

        set
        {
            if (_m_Tr_WorldObject == null)
                _m_Tr_WorldObject = GetComponent<Transform>();

            _m_Tr_WorldObject.position = value;
        }
    }

    // 캐릭터 피벗 위치
    [SerializeField] GameObject[] _m_Pivots;
    [SerializeField] GameObject _m_TempPivots;

    [SerializeField] List<Entity> _m_Entities;

    public float GetEntityAttackRange()
    {
        return _m_Entities.Count <= 0 ? 1f : _m_Entities[0].Info.AttackRange;
    }

    public EntityGrade GetEntityGrade()
    {
        var _jobID = _m_Entities.Count <= 0? 1 : _m_Entities[0].CharacterID;

        GameDataManager.GetInstance().GetGameDBCharacterInfo(_jobID, out var Info);

        return Info._me_Grade;
    }

    public EntityDivision GetEntityDivision()
    {
        var _division = _m_Entities[0] == null ? EntityDivision.Player : _m_Entities[0]._me_Division;
        return _division;
    }

    public void AddEntity(ref Entity _entity)
    {
        if (_m_Entities.Count > 3) return;
        _m_Entities.Add(_entity);

        OffPivot();
        RelocationPivot();
    }
    public bool IsEnableAddEntity()
    {
        bool _isEnable = true;

        if(Count >= 1) // 하나가 이미 있다.
        {
            var grade = GetEntityGrade();
            if (grade >= EntityGrade.Myth)
            {
                _isEnable = false;
            }
            else
            {
                if (Count < 3)
                    _isEnable = true;
                else
                    _isEnable = false;
            }
        }

        return _isEnable;
    }
    public void Initialize()
    {
        _m_Entities = new List<Entity>();
    }
    public void OffPivot()
    {
        for(int i = 0; i < _m_Pivots.Length; ++i)
        {
            _m_Pivots[i].SetActive(false);
        }

        for(int i = 0; i < _m_Entities.Count; ++i)
        {
            _m_Entities[i].transform.SetParent(_m_TempPivots.transform);
        }
    }
    public void RelocationPivot()
    {
        int _mi_PivotTargetIndex = _m_Entities.Count-1;

        GameObject _targetPivot = _m_Pivots[_mi_PivotTargetIndex];
        _targetPivot.SetActive(true);

        for (int i =0; i < _targetPivot.transform.childCount; ++i)
        {
            var _tr_Child = _targetPivot.transform.GetChild(i);
            _m_Entities[i].transform.SetParent(_tr_Child);
            _m_Entities[i].Controller.Pos3D = _tr_Child.position;
        }
    }
    public void MoveAllEntities(Vector2Int _mMoveIndex)
    {

    }
    public void RemoveEntities()
    {
        for (int i = 0; i < _m_Entities.Count; ++i)
        {
            _m_Entities[i].Controller?._onCB_DiedProcess?.Invoke();
            _m_Entities[i].Controller?.SetChaseEntity(null);
            GameObject.Destroy(_m_Entities[i].gameObject);
        }
    }
    public void RemoveLastEntity()
    {
        List<Entity> _mLt_RemoveEntity = new List<Entity>();

        for (int i = _m_Entities.Count-1; i >= 0; --i)
        {
            if (_m_Entities[i] == null) continue;
            else
            {
                _m_Entities[i].Controller?._onCB_DiedProcess?.Invoke();
                _m_Entities[i].Controller?.SetChaseEntity(null);
                _mLt_RemoveEntity.Add(_m_Entities[i]);
                GameObject.Destroy(_m_Entities[i].gameObject);
                break;
            }
        }

        for(int i = 0; i < _mLt_RemoveEntity.Count; ++i)
        {
            _m_Entities.Remove(_mLt_RemoveEntity[i]);
        }
    }
}

