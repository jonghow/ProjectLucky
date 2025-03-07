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

        CacheAtlas();

        GameDataManager.GetInstance().GetRecipeData(_mi_RecipeID, out GameDB_MealRecipe _ret);

        if (_ret == null) return;

        _mStr_Recipe = _ret._mStr_Recipe;
        _mArr_Recipe = _ret.Arr_Recipe;
        _mi_CharacterID = _ret._mi_MealKitID;

        OnUpdateMaterials();
        OnUpdateModel();
    }

    public void CacheAtlas()
    {
        ResourceManager.GetInstance().GetResource(ResourceType.PortraitAtlas, 12, true, (obj) =>
        {
            _m_CachedAtlas = obj as SpriteAtlas;
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

    public void OnUpdateModel()
    {
        _m_Img_Model.sprite = _m_CachedAtlas.GetSprite($"CharacterResource_{String.Format("{0:00}", _mi_CharacterID)}");
    }
}

