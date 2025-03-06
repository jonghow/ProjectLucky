using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public class EntitiesGroup : MonoBehaviour
{
    int _mi_ID;
    public int ID { get { return _mi_ID; } set { _mi_ID = value; } }
    public int Count { get { return _m_Entities.Count; } }

    // ĳ���� �ǹ� ��ġ
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

        for(int i =0; i < _targetPivot.transform.childCount; ++i)
        {
            var _tr_Child = _targetPivot.transform.GetChild(i);
            _m_Entities[i].transform.SetParent(_tr_Child);
            _m_Entities[i].Controller.Pos3D = _tr_Child.position;
        }
    }
}
