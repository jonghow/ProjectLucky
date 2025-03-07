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

public class RecipeMaterial : MonoBehaviour 
{
    [SerializeField] int _mi_CharacterID;
    [SerializeField] Image _m_Img_Portrait;

    [SerializeField] TextMeshProUGUI _mText_VisualUsed;

    SpriteAtlas _m_CachedAtlas;

    public void OnRelease()
    {
    }

    public void OnInitElement(int _characterID)
    {
        CacheAtlas();
        _mi_CharacterID = _characterID;

        OnUpdate();
    }

    public void OnUpdate()
    {
        _mText_VisualUsed.text = FindCharacter() == true ? "보유중" : "미보유";
    }
    public bool FindCharacter()
    {
        bool _ret = false;

        List<EntitiesGroup> _Lt_Entities = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);

        for(int i = 0; i < _Lt_Entities.Count; ++i)
        {
            var _groups = _Lt_Entities[i];

            if(_groups.ID == _mi_CharacterID)
            {
                _ret = true;
                break;
            }
        }

        return _ret;
    }
    public void CacheAtlas()
    {
        ResourceManager.GetInstance().GetResource(ResourceType.PortraitAtlas, 12, true, (obj) =>
        {
            _m_CachedAtlas = obj as SpriteAtlas;
            _m_Img_Portrait.sprite = _m_CachedAtlas.GetSprite($"CharacterResource_{String.Format("{0:00}", _mi_CharacterID)}");
        });
    }
}

