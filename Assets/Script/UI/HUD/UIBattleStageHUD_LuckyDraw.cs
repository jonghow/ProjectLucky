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

public class UIBattleStageHUD_LuckyDraw : MonoBehaviour , IBattleHUDActivation
{
    [SerializeField] TMPro.TextMeshProUGUI _mText_dia;
    [SerializeField] TMPro.TextMeshProUGUI _mText_Supply;

    public void ProcActivationCardList(bool isActive)
    {
        UpdateText();
        this.gameObject.SetActive(isActive);
    }

    public void UpdateText()
    {
        //_mText_dia.text
        //_mText_Supply.text
    }
    public void OnClick_Close()
    {
        this.gameObject.SetActive(false);
    }
    public void OnClick_DrawUnCommon()
    {
        int _rate = 60;
        EntityGrade _me_Grade = EntityGrade.UnCommon;

        int _drawVal = UnityEngine.Random.Range(0, 100);

        if(_rate >= _drawVal)
        {
            // success

            int _drawJobID = DrawCharacterID(_me_Grade);
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
        else
        {
            // fail
            UnityLogger.GetInstance().Log($"실패"); 
        }
    }
    public void OnClick_DrawHero()
    {
        int _rate = 20;
        EntityGrade _me_Grade = EntityGrade.Hero;

        int _drawVal = UnityEngine.Random.Range(0, 100);

        if (_rate >= _drawVal)
        {
            // success

            int _drawJobID = DrawCharacterID(_me_Grade);
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
        else
        {
            // fail
            UnityLogger.GetInstance().Log($"실패");
        }

    }
    public void OnClick_DrawMyth()
    {
        int _rate = 20;
        EntityGrade _me_Grade = EntityGrade.Myth;

        int _drawVal = UnityEngine.Random.Range(0, 100);

        if (_rate >= _drawVal)
        {
            // success
            int _drawJobID = DrawCharacterID(_me_Grade);
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
        else
        {
            // fail
            UnityLogger.GetInstance().Log($"실패");
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

            _ = _entitySpanwer.CreateEntity(_jobID, _v3_position, (entity) =>
            {
                entitiesGroup.AddEntity(ref entity);

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

