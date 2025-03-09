using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using System.Linq;
using TMPro;

public class UIBattleStageHUD : MonoBehaviour
{
    /// <summary>
    /// Bottom
    /// </summary>
    [SerializeField] UIBattleStageHUD_LuckyDraw _m_MyLuckyDraw;
    [SerializeField] UIBattleStageHUD_Combine _m_Combine;
    [SerializeField] UIBattleStageHUD_AlertBoss _m_AlertBoss;

    [SerializeField] TextMeshProUGUI _mText_Gold;
    [SerializeField] TextMeshProUGUI _mText_Dia;
    [SerializeField] TextMeshProUGUI _mText_Supply;

    [SerializeField] string _mStr_SupplyFormat;

    private void Awake()
    {
        _mStr_SupplyFormat = $"{0}/{Defines.NormalSingleGameSupplyMaxCount}";
    }

    public void Start()
    {
        UpdateGold(PlayerManager.GetInstance().GetGold());
        UpdateDia(PlayerManager.GetInstance().GetDia());
        UpdateSupply(PlayerManager.GetInstance().GetSupply());
    }

    private void OnEnable()
    {
        PlayerManager.GetInstance()._onCB_ChangeGold -= UpdateGold;
        PlayerManager.GetInstance()._onCB_ChangeGold += UpdateGold;

        PlayerManager.GetInstance()._onCB_ChangeDia -= UpdateDia;
        PlayerManager.GetInstance()._onCB_ChangeDia += UpdateDia;

        PlayerManager.GetInstance()._onCB_ChangeSupply -= UpdateSupply;
        PlayerManager.GetInstance()._onCB_ChangeSupply += UpdateSupply;

        PlayerManager.GetInstance()._onCB_AlertBoss -= OnNotify_AlertBoss;
        PlayerManager.GetInstance()._onCB_AlertBoss += OnNotify_AlertBoss;

    }
    private void OnDisable()
    {
        PlayerManager.GetInstance()._onCB_ChangeGold -= UpdateGold;

        PlayerManager.GetInstance()._onCB_ChangeDia -= UpdateDia;

        PlayerManager.GetInstance()._onCB_ChangeSupply -= UpdateSupply;

        PlayerManager.GetInstance()._onCB_AlertBoss -= OnNotify_AlertBoss;
    }
    public void On_ClickSpawn()
    {
        int _jobID = DrawCharacterID();
        FindEnableEntityGroups(_jobID , out var _entitiesGroup);

        if(_entitiesGroup == null)
        {
            DrawAnyMapNavigation(out var _Navigation);
            Spawn(_jobID, _Navigation);
        }
        else
        {
            Spawn(_jobID, _entitiesGroup);
        }

        PlayerManager.GetInstance().UseGold(Defines.DrawDefaultGoldPrice); // 스폰했으니 차감
    }
    public void OnClick_LuckyDraw()
    {
        _m_MyLuckyDraw.ProcActivationCardList(true);
    }
    public void OnClick_Combine()
    {
        _m_Combine.ProcActivationCardList(true);
    }
    public int DrawCharacterID()
    {
        List<GameDB_CharacterInfo> _Lt_Infos;
        GameDataManager.GetInstance().GetGameDBCharacterInfoByGrade(new EntityGrade[2] { EntityGrade.Common, EntityGrade.UnCommon }, out _Lt_Infos);

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
    public void FindEnableEntityGroups(int _jobID, out EntitiesGroup _ret)
    {
        _ret = null;
        EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player, _jobID, out _ret);
    }
    public void DrawAnyMapNavigation(out NavigationElement _retNavigation)
    {
        MapManager.GetInstance().GetNavigationElements(out var _dictElements);
        var _Lt_Elements = new List<NavigationElement>(_dictElements.Values.ToList());

        var _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);

        for(int i = 0; i < _Lt_Groups.Count; ++i)
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

            for(int j = 0; j < _mLt_ElementToRemove.Count; ++j)
            {
                _Lt_Elements.Remove(_mLt_ElementToRemove[j]);
            }
        }
        // 필터링

        int suffleCount = 20;

        for(int i = 0; i < suffleCount; ++i)
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

    #region GoodsInfo

    public void UpdateGold(int _gold)
    {
        _mText_Gold.text = _gold.ToString();
    }

    public void UpdateDia(int _dia)
    {
        _mText_Dia.text = _dia.ToString();
    }

    public void UpdateSupply(int _supply)
    {
        _mText_Supply.text = string.Format(_mStr_SupplyFormat, _supply);
    }

    #endregion

    #region AlertBoss

    public void OnNotify_AlertBoss(int _bossIndex)
    {
        _m_AlertBoss.OnPlayAlertBoss(_bossIndex);
    }





    #endregion

}
