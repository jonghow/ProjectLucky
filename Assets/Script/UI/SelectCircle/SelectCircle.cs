using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using System.Linq;
using System.ComponentModel;
using Unity.Mathematics;

public class SelectCircle : MonoBehaviour
{
    [SerializeField] RectTransform _mRectTr_SelectedCircle;
    [SerializeField] Transform _m_Pivot;
    [SerializeField] Image _mImg_Range;

    [SerializeField] EntitiesGroup _m_CachedOwnerEntity;

    [SerializeField] GameObject _m_BtnCombine;

    [SerializeField] float _manipluas;

    float _m_defaultWidth = 100f;

    public Vector3 GetPivotPos3D()
    {
        return _m_Pivot.transform.position;
    }

    public void Awake()
    {
        _manipluas = 90f;
        _mRectTr_SelectedCircle = GetComponent<RectTransform>();
        _m_Pivot.gameObject.SetActive(false); 
    }

    public Transform GetTransform() => _mRectTr_SelectedCircle;
    public void SetOwnerEntity(EntitiesGroup _entity)
    {
        _m_CachedOwnerEntity = _entity;

        if (_entity != null)
        {
            UpdateRangeScale();
            _m_Pivot.gameObject.SetActive(true);
            _m_BtnCombine.gameObject.SetActive(_entity.GetEntityGrade() < EntityGrade.Myth); // 히어로부터 버튼 뺀다. 신화는 무조건 조합UI
        }
        else
        {
            ResetScale();
            _m_Pivot.gameObject.SetActive(false);
        }

        OnUpdateItem();
    }

    public void UpdateRangeScale()
    {
        var rectTr = _mImg_Range.GetComponent<RectTransform>();

        float _fRange = _m_CachedOwnerEntity.GetEntityAttackRange();

        float _newScale = (_fRange * 2)*_m_defaultWidth ; // 내 기준 중점 좌표에서부터 시작이라 상하 2배
        rectTr.sizeDelta = new Vector2(_newScale, _newScale);
    }

    public void ResetScale()
    {
        var rectTr = _mImg_Range.GetComponent<RectTransform>();
        rectTr.sizeDelta = new Vector2(_m_defaultWidth, _m_defaultWidth);
    }

    public void OnUpdateItem()
    {
    }

    private void Update()
    {
        if(_m_CachedOwnerEntity != null)
        {
            Vector3 _ownerPos = _m_CachedOwnerEntity.Pos3D;
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(_ownerPos);

            this._mRectTr_SelectedCircle.position = _screenPos;
        }
        else
        {
            this._mRectTr_SelectedCircle.localPosition = new Vector3(99999f, 99999f, 99999f);
        }
    }

    public void OnClickCombine()
    {
        if (_m_CachedOwnerEntity == null) return;
        if (_m_CachedOwnerEntity.IsEnableAddEntity() == true) return;

        GameDataManager.GetInstance().GetGameDBCharacterInfo(_m_CachedOwnerEntity.ID, out var _ret);

        // 데이터 가져오고 내가 가진 용병을 지운다

        int _jobID = _m_CachedOwnerEntity.ID;
        long _uid = _m_CachedOwnerEntity.UniqueID;

        EntityManager.GetInstance().NewRemoveGroup(EntityDivision.Player, _jobID, _uid);

        _m_CachedOwnerEntity = null;
        PlayerManager.GetInstance().SetSelectedEntity(null);
        SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());

        // 용병 삭제

        int _mi_Grade = (int)_ret._me_Grade;

        if (_mi_Grade >= 4) return;

         EntityGrade _me_NextGrade = (EntityGrade)(_mi_Grade + 1);

        int _drawJobID = DrawCharacterID(_me_NextGrade);
        FindEnableEntityGroups(_drawJobID, out var _entitiesGroup);

        if (_entitiesGroup == null)
        {
            DrawAnyMapNavigation(out var _Navigation);
            Spawn(_drawJobID, _Navigation);
        }
        else
        {
            Spawn(_drawJobID, _entitiesGroup);
        }
    }

    public void FindEnableEntityGroups(int _jobID, out EntitiesGroup _ret)
    {
        _ret = null;
        EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player, _jobID, out _ret);
    }


    public int DrawCharacterID(EntityGrade _drawGrade)
    {
        List<GameDB_CharacterInfo> _Lt_Infos;
        GameDataManager.GetInstance().GetGameDBCharacterInfoByGrade(new EntityGrade[1] { _drawGrade }, out _Lt_Infos);

        int suffleCount = 20;

        for (int i = 0; i < suffleCount; ++i)
        {
            int _prevIndex = UnityEngine.Random.Range(0, _Lt_Infos.Count);
            int _nextIndex = UnityEngine.Random.Range(0, _Lt_Infos.Count);

            var _temp = _Lt_Infos[_nextIndex];
            _Lt_Infos[_nextIndex] = _Lt_Infos[_prevIndex];
            _Lt_Infos[_prevIndex] = _temp;
        }

        return _Lt_Infos[0]._mi_CharacterID;
    }



    public void OnClickTrash()
    {
        UnityLogger.GetInstance().Log($"OnClickTrash");

        int _jobID = _m_CachedOwnerEntity.ID;
        long _uid = _m_CachedOwnerEntity.UniqueID;

        EntityManager.GetInstance().NewRemoveGroup(EntityDivision.Player,_jobID, _uid);

        _m_CachedOwnerEntity = null;
        PlayerManager.GetInstance().SetSelectedEntity(null);
        SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
    }

    public void DrawAnyMapNavigation(out NavigationElement _retNavigation)
    {
        MapManager.GetInstance().GetNavigationElements(out var _dictElements);
        var _Lt_Elements = new List<NavigationElement>(_dictElements.Values.ToList());

        var _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);

        for (int i = 0; i < _Lt_Groups.Count; ++i)
        {
            Vector2Int _v2_Index = _Lt_Groups[i].NvPos;

            var _mLt_ElementToRemove = new List<NavigationElement>();

            foreach (var pair in _Lt_Elements)
            {
                if (pair._mv2_Index == _v2_Index)
                {
                    _mLt_ElementToRemove.Add(pair);
                }
            }

            for (int j = 0; j < _mLt_ElementToRemove.Count; ++j)
            {
                _Lt_Elements.Remove(_mLt_ElementToRemove[j]);
            }
        }
        // 필터링

        int suffleCount = 20;

        for (int i = 0; i < suffleCount; ++i)
        {
            int _prevIndex = UnityEngine.Random.Range(0, _Lt_Elements.Count);
            int _nextIndex = UnityEngine.Random.Range(0, _Lt_Elements.Count);

            var _temp = _Lt_Elements[_nextIndex];
            _Lt_Elements[_nextIndex] = _Lt_Elements[_prevIndex];
            _Lt_Elements[_prevIndex] = _temp;
        }
        // 셔플 완료
        _retNavigation = _Lt_Elements[0];
    }
    public void Spawn(int _jobID, NavigationElement _selectedNavigation)
    {
        if (_selectedNavigation == null)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"SpawnEntity", $"_selectedNavigation is NULL");
            return;
        }

        Vector2Int _n2_NavIdx = _selectedNavigation._mv2_Index;
        Vector3 _v3_position = _selectedNavigation._mv3_Pos;

        int spawnID = _jobID;

        UserEntitiesGroupFactory _spawner = new UserEntitiesGroupFactory();
        _ = _spawner.CreateEntity(spawnID, _v3_position, (entitiesGroup) =>
        {
            UserEntityFactory _entitySpanwer = new UserEntityFactory();

            _ = _entitySpanwer.CreateEntity(_jobID, _v3_position, (_createEntity) =>
            {
                entitiesGroup.AddEntity(ref _createEntity);
                PlayerManager.GetInstance().AddSupply(1);
                _createEntity.Controller._ml_EntityGroupUID = entitiesGroup.UniqueID;

                _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            });
        });
    }
    public void Spawn(int _jobID, EntitiesGroup _entitiesGroup)
    {
        Vector2Int _n2_NavIdx = _entitiesGroup.NvPos;
        Vector3 _v3_position = _entitiesGroup.Pos3D;

        int spawnID = _jobID;

        UserEntityFactory _entitySpanwer = new UserEntityFactory();
        _ = _entitySpanwer.CreateEntity(spawnID, _v3_position, (_createEntity) =>
        {
            _entitiesGroup.AddEntity(ref _createEntity);
            PlayerManager.GetInstance().AddSupply(1);
            _createEntity.Controller._ml_EntityGroupUID = _entitiesGroup.UniqueID;

            _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
        });
    }
}