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

public class UIBattleStageHUD_Combine : MonoBehaviour , IBattleHUDActivation
{
    [SerializeField] RecipeElement _m_CombineOrigin;

    [SerializeField] List<RecipeElement> _mLt_Recipe;
    [SerializeField] GameObject _CombineContentsParent;

    [SerializeField] RecipeInfo _m_RecipeInfo;

    public int _mi_SelectID;

    public void OnSetFirstElement()
    {
        if (_mLt_Recipe.Count > 0)
            _mLt_Recipe[0].OnClickElement();
    }

    public void ProcActivationCardList(bool isActive)
    {
        OnInitRecipeElement();
        UpdateRecipes();
        OnSetFirstElement();
        this.gameObject.SetActive(isActive);
    }

    public void UpdateRecipes()
    {
        UpdateInfoElement();
    }

    public void OnInitRecipeElement()
    {
        GameDataManager.GetInstance().GetRecipeDatasToList(out List<GameDB_MealRecipe> _ret);

        int Index = 0; 

        for(; Index < _ret.Count; ++Index)
        {
            GameObject _obj;
            int _recipeID = _ret[Index]._mi_ID;
            RecipeElement _recipe;
            if (Index >= _mLt_Recipe.Count)
            {
                _obj = GameObject.Instantiate(_m_CombineOrigin.gameObject, _CombineContentsParent.transform);
                _obj.transform.SetAsLastSibling();
                _recipe = _obj.GetComponent<RecipeElement>();
                _mLt_Recipe.Add(_recipe);
            }
            else
            {
                _obj = _mLt_Recipe[Index].gameObject;
                _recipe = _obj.GetComponent<RecipeElement>();
            }

            _recipe.OnInitElement(_recipeID, (_m_id) => {
                _mi_SelectID = _m_id;
                UpdateRecipes(); 
            });
            _recipe.gameObject.SetActive(true);
        }
        for(int i = Index; i < _mLt_Recipe.Count; ++i)
        {
            var _recipe = _mLt_Recipe[i];
            _recipe.OnRelease();
            _recipe.gameObject.SetActive(false);
        }
    }

    public void UpdateInfoElement()
    {
        _m_RecipeInfo.OnUpdate(_mi_SelectID);
    }
    public void OnClick_Close()
    {
        this.gameObject.SetActive(false);
    }
    public void OnClick_Combine()
    {
       
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
        _ = _entitySpanwer.CreateEntity(spawnID, _v3_position, (entity) =>
        {
            _entitiesGroup.AddEntity(ref entity);
        });
    }

}

