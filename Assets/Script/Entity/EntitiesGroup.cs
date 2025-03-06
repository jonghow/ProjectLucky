using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public class EntitiesGroup : MonoBehaviour
{
    int _mi_ID;
    long _ml_uniqueID;
    Vector2Int _mv2_NavigationPos;
    public Vector2Int NvPos { get { return _mv2_NavigationPos; } set { _mv2_NavigationPos = value; } }
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

    public void AddEntity(ref Entity _entity)
    {
        if (_m_Entities.Count > 3) return;
        _m_Entities.Add(_entity);

        OffPivot();
        RelocationPivot();
    }
    public bool IsEnableAddEntity()
    {
        return Count < 3;
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
}

