using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.U2D;

public class RecipeInfo : MonoBehaviour 
{
    [SerializeField] int _mi_RecipeID;
    [SerializeField] int _mi_CharacterID;
    [SerializeField] string _mStr_Recipe;

    [SerializeField] int[] _mArr_Recipe;

    SpriteAtlas _m_CachedAtlas;

    [Header("Top")]
    [SerializeField] TMPro.TextMeshProUGUI _m_TextName;
    [SerializeField] Image _m_Img_TopPortrait;

    [SerializeField] TMPro.TextMeshProUGUI _m_TextProgress;

    [Header("Materials")]
    [SerializeField] RecipeMaterial _m_MaterialsOrigin;

    [SerializeField] List<RecipeMaterial> _mLt_RecipeMaterial;
    [SerializeField] GameObject _RecipeMaterialContentsParent;

    [SerializeField] Image _m_Img_Model;
    [SerializeField] Button _mBtn_Combine;

    public void OnUpdate(int _recipeID)
    {
        _mi_RecipeID = _recipeID;


        GameDataManager.GetInstance().GetRecipeData(_mi_RecipeID, out GameDB_MealRecipe _ret);

        if (_ret == null) return;

        _mStr_Recipe = _ret._mStr_Recipe;
        _mArr_Recipe = _ret.Arr_Recipe;
        _mi_CharacterID = _ret._mi_MealKitID;

        CacheAtlas();

        OnUpdateMaterials();
        OnUpdateProgeress();
        OnUpdateBtnCombine();
    }

    public void CacheAtlas()
    {
        ResourceManager.GetInstance().GetResource(ResourceType.PortraitAtlas, 12, true, (obj) =>
        {
            _m_CachedAtlas = obj as SpriteAtlas;
            OnUpdateModel();
        });
    }

    public void OnUpdateMaterials()
    {
        int Index = 0;

        for (; Index < _mArr_Recipe.Length; ++Index)
        {
            GameObject _obj;
            int _characterID = _mArr_Recipe[Index];

            RecipeMaterial _materials;
            if (Index >= _mLt_RecipeMaterial.Count)
            {
                _obj = GameObject.Instantiate(_m_MaterialsOrigin.gameObject, _RecipeMaterialContentsParent.transform);
                _obj.transform.SetAsLastSibling();
                _materials = _obj.GetComponent<RecipeMaterial>();
                _mLt_RecipeMaterial.Add(_materials);
            }
            else
            {
                _obj = _mLt_RecipeMaterial[Index].gameObject;
                _materials = _obj.GetComponent<RecipeMaterial>();
            }

            _materials.OnInitElement(_characterID);
            _materials.gameObject.SetActive(true);
        }
        for (int i = Index; i < _mLt_RecipeMaterial.Count; ++i)
        {
            var _recipe = _mLt_RecipeMaterial[i];
            _recipe.OnRelease();
            _recipe.gameObject.SetActive(false);
        }
    }

    public void OnUpdateProgeress()
    {
        //_m_TextProgress 

        // 일단 냅두고 조합 부터
    }

    public void OnUpdateModel()
    {
        _m_Img_Model.sprite = _m_CachedAtlas.GetSprite($"CharacterResource_{String.Format("{0:00}", _mi_CharacterID)}");
    }
    public void OnUpdateBtnCombine()
    {
        bool _ret = true;

        List<EntitiesGroup> _Lt_Entities = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);
        int _passCount = 0;

        if(_Lt_Entities.Count <= 0 )
        {
            _ret = false;
        }
        else
        {
            for (int i = 0; i < _mArr_Recipe.Length; ++i)
            {
                int _characterID = _mArr_Recipe[i];

                for (int j = 0; j < _Lt_Entities.Count; ++j)
                {
                    var _groups = _Lt_Entities[j];

                    if (_groups.ID == _characterID)
                    {
                        ++_passCount;
                        break;
                    }
                }
            }

            _ret = _passCount == _mArr_Recipe.Length;
        }

        _mBtn_Combine.interactable = _ret;
    }
    public void OnClick_Combine()
    {
        if (_mBtn_Combine.enabled == false) return;

        List<EntitiesGroup> _Lt_Entities = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);

        List<EntitiesGroup> _Lt_UseCandidate_CountSindle = new List<EntitiesGroup>();
        List<EntitiesGroup> _Lt_UseCandidate_CountMulti = new List<EntitiesGroup>();

        int _passCount = 0;

        for (int i = 0; i < _mArr_Recipe.Length; ++i)
        {
            int _characterID = _mArr_Recipe[i];

            for (int j = 0; j < _Lt_Entities.Count; ++j)
            {
                var _groups = _Lt_Entities[j];

                if (_groups.ID == _characterID)
                {
                    if(_groups.Count == 1)
                    {
                        _Lt_UseCandidate_CountSindle.Add(_groups);
                        // 용병 삭제
                    }
                    else
                    {
                        _Lt_UseCandidate_CountMulti.Add(_groups); 
                    }

                    ++_passCount;
                    break;
                }
            }
        }
        // 여긴 용병 후보 대기 구문

        if (_passCount != _mArr_Recipe.Length) return; // 통과 카운터와 재료 갯수가 맞지 않으면 수행하지 않음

        for(int i = 0; i < _Lt_UseCandidate_CountSindle.Count; ++i)
        {
            int _jobID = _Lt_UseCandidate_CountSindle[i].ID;
            long _uid = _Lt_UseCandidate_CountSindle[i].UniqueID;

            EntityManager.GetInstance().NewRemoveGroup(EntityDivision.Player, _jobID, _uid);
        }

        for (int i = 0; i < _Lt_UseCandidate_CountMulti.Count; ++i)
        {
            _Lt_UseCandidate_CountMulti[i].RemoveLastEntity();
        }

        int _createJobID = _mi_CharacterID;

        FindEnableEntityGroups(_createJobID, out var _entitiesGroup);

        if (_entitiesGroup == null)
        {
            DrawAnyMapNavigation(out var _Navigation);
            Spawn(_createJobID, _Navigation);
        }
        else
        {
            Spawn(_createJobID, _entitiesGroup);
        }

        OnUpdate(_mi_RecipeID);
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

