using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using UnityEditor;

public class EntityManager
{
    public static EntityManager Instance;
    public static EntityManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new EntityManager();
        }

        return Instance;
    }

    public Dictionary<EntityDivision, Dictionary<long, Entity>> _mDict_Entities = new Dictionary<EntityDivision, Dictionary<long, Entity>>();
    public void AddEntity(EntityDivision _category, long _uid, ref Entity _entity)
    {
        if (_mDict_Entities.ContainsKey(_category) == false)
            _mDict_Entities.Add(_category, new Dictionary<long, Entity>());

        _mDict_Entities[_category].Add(_uid, _entity);
    }
    public void GetEntity(EntityDivision _category, long _uid, out Entity _entity)
    {
        Dictionary<long, Entity> _uidPair = null;
        _entity = null;

        if (_mDict_Entities.TryGetValue(_category, out _uidPair))
        {
            if (_uidPair.TryGetValue(_uid, out _entity))
            {
            }
        }
    }
    public void GetEntityList(EntityDivision _category, out List<Tuple<long, Entity>> _listEntities)
    {
        if (_mDict_Entities.ContainsKey(_category) == false)
            _mDict_Entities.Add(_category, new Dictionary<long, Entity>());

        _listEntities = new List<Tuple<long, Entity>>();

        foreach (var pair in _mDict_Entities[_category])
        {
            _listEntities.Add(new Tuple<long, Entity>(pair.Key, pair.Value));
        }
    }
    public void GetEntity(long _uid, out Entity _entity)
    {
        _entity = null;
        foreach(var _dict_Category_EntitiesPair in _mDict_Entities)
        {
            var _dict_Category_Entities = _dict_Category_EntitiesPair.Value;
            foreach (var _dic_Inner_Entities in _dict_Category_Entities)
            {
                var checkEntity = _dic_Inner_Entities.Value;
                if (checkEntity.UID == _uid)
                {
                    _entity = checkEntity;
                    return;
                }
            }
        }
    }
    public void GetEntityList(EntityDivision[] _categories, out List<Tuple<long, Entity>> _listEntities)
    {
        for (int i = 0; i < _categories.Length; ++i)
        {
            if (_mDict_Entities.ContainsKey(_categories[i]) == false)
                _mDict_Entities.Add(_categories[i], new Dictionary<long, Entity>());
        }

        _listEntities = new List<Tuple<long, Entity>>();

        for (int i = 0; i < _categories.Length; ++i)
        {
            foreach (var pair in _mDict_Entities[_categories[i]])
            {
                _listEntities.Add(new Tuple<long, Entity>(pair.Key, pair.Value));
            }
        }
    }
    public bool CheckContainEntityKey(long _uid)
    {
        foreach (var _dict_Category_EntitiesPair in _mDict_Entities)
        {
            var _dict_Category_Entities = _dict_Category_EntitiesPair.Value;

            if(_dict_Category_Entities.ContainsKey(_uid))
            {
                return true;
            }
        }
        return false;
    }
    public void RemoveEntity(long _uid)
    {
        foreach (var _dict_Category_EntitiesPair in _mDict_Entities)
        {
            var _dict_Category_Entities = _dict_Category_EntitiesPair.Value;
            foreach (var _dic_Inner_Entities in _dict_Category_Entities)
            {
                var checkEntity = _dic_Inner_Entities.Value;
                if (checkEntity.UID == _uid)
                {
                    _dict_Category_Entities.Remove(_uid);
                    return;
                }
            }
        }
    }
    public void ClearEntity()
    {
        foreach (var _dict_Category_EntitiesPair in _mDict_Entities)
        {
            var _dict_Category_Entities = _dict_Category_EntitiesPair.Value;
            foreach (var _dic_Inner_Entities in _dict_Category_Entities)
            {
                var checkEntity = _dic_Inner_Entities.Value;
                checkEntity.Controller?._onCB_DiedProcess?.Invoke();
                GameObject.Destroy(checkEntity.gameObject);
            }

            _dict_Category_Entities.Clear();
        }
        _mDict_Entities.Clear();
    }

    #region NewManager

    public Dictionary<EntityDivision, List<EntitiesGroup>> _mDict_EntityGroup = new Dictionary<EntityDivision, List<EntitiesGroup>>();

    public void NewAddEntityGroup(EntityDivision _category, int _jobID, ref EntitiesGroup _entitiesGroup)
    {
        if (_mDict_EntityGroup.ContainsKey(_category) == false)
        {
            _mDict_EntityGroup.Add(_category, new List<EntitiesGroup>());
        }

        _mDict_EntityGroup[_category].Add(_entitiesGroup);

        //var _groups = _mDict_EntityGroup[_category].FindAll(rhs => rhs.ID == _jobID);
        //bool _isSetEntity = false;

        //foreach (var group in _groups)
        //{
        //    if (!group.IsEnableAddEntity())
        //        continue;

        //    _isSetEntity = true;
        //    group.AddEntity(ref _entity);
        //}

        //if (!_isSetEntity)
        //{
        //    // 아무곳에도 넣지 못한 경우 그룹을 만든다.
        //    var _newEntitiesGroup = new EntitiesGroup();
        //    _newEntitiesGroup.ID = _jobID;

        //    _mDict_EntityGroup[_category].Add(_newEntitiesGroup);
        //}
    }
    public void NewGetEntityGroups(EntityDivision _category, int _jobID, out EntitiesGroup _ret)
    {
        _ret = null;
        // 내가 들어갈자리를 찾아본다.
        if (_mDict_EntityGroup.ContainsKey(_category) == false)
        {
            _mDict_EntityGroup.Add(_category, new List<EntitiesGroup>());
        }

        var _groups = _mDict_EntityGroup[_category].FindAll(rhs => rhs.ID == _jobID);
        foreach (var group in _groups)
        {
            if (group.IsEnableAddEntity())
            {
                _ret = group;
                break;
                // 타겟으로 하는 곳을 찾았다.
            }
        }
    }
    public Dictionary<EntityDivision, List<EntitiesGroup>> NewGetEntityGroups() => _mDict_EntityGroup;
    public List<EntitiesGroup> NewGetEntityGroups(EntityDivision eDivision)
    {
        if(!_mDict_EntityGroup.ContainsKey(eDivision))
        {
            _mDict_EntityGroup.Add(eDivision, new List<EntitiesGroup>());
        }

        return _mDict_EntityGroup[eDivision];
    }

    public List<EntitiesGroup> NewGetEntityGroups(EntityDivision[] _categories)
    {

        for (int i = 0; i < _categories.Length; ++i)
        {
            if (_mDict_EntityGroup.ContainsKey(_categories[i]) == false)
                _mDict_EntityGroup.Add(_categories[i], new List<EntitiesGroup>());
        }

        List<EntitiesGroup> _Lt_Entities = new List<EntitiesGroup>();

        for (int i = 0; i < _categories.Length; ++i)
        {
            foreach (var pair in _mDict_EntityGroup[_categories[i]])
            {
                _Lt_Entities.Add(pair);
            }
        }

        return _Lt_Entities;
    }

    public void NewRemoveGroup(EntityDivision _category, int _jobID, long _uid)
    {
        // 내가 들어갈자리를 찾아본다.
        if (_mDict_EntityGroup.ContainsKey(_category) == false)
        {
            _mDict_EntityGroup.Add(_category, new List<EntitiesGroup>());
        }

        var _groups = _mDict_EntityGroup[_category].FindAll(rhs => rhs.ID == _jobID);

        foreach (var group in _groups)
        {
            if (group.UniqueID == _uid)
            {
                group.RemoveEntities();
                _mDict_EntityGroup[_category].RemoveAll(rhs => rhs.UniqueID == _uid);
                GameObject.Destroy(group.transform.parent.gameObject);
                break;
                // 타겟으로 하는 곳을 찾았다.
            }
        }
    }

    #endregion
}