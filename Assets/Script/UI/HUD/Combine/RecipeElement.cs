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

public class RecipeElement : MonoBehaviour 
{
    [SerializeField] int _mi_RecipeID;
    [SerializeField] int _mi_CharacterID;
    [SerializeField] Image _m_Img_Portrait;

    Action<int> _mCB_ClickCallback;

    public void OnRelease()
    {
        _mi_RecipeID = 0;
        _mCB_ClickCallback = null;
    }

    public void OnInitElement(int _recipeID, Action<int> _onCB_Click)
    {
        _mi_RecipeID = _recipeID;

        GameDataManager.GetInstance().GetRecipeData(_recipeID, out var _ret);
        _mi_CharacterID = _ret == null ? 0 : _ret._mi_MealKitID;

        _mCB_ClickCallback += _onCB_Click;

        OnUpdate();
    }

    public void OnClickElement()
    {
        _mCB_ClickCallback?.Invoke(_mi_RecipeID);
    }

    public void OnUpdate()
    {
        ResourceManager.GetInstance().GetResource(ResourceType.PortraitAtlas, 12, true, (obj) =>
        {
            SpriteAtlas _atlas = obj as SpriteAtlas;
            _m_Img_Portrait.sprite = _atlas.GetSprite($"CharacterResource_{String.Format("{0:00}", _mi_CharacterID)}");
        });
    }
}

