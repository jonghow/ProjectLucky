using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class UIBattleStageHUD : MonoBehaviour
{
    /// <summary>
    /// Bottom
    /// </summary>
    [SerializeField] UIBattleStageHUD_LuckyDraw _m_MyLuckyDraw;
    [SerializeField] UIBattleStageHUD_Combine _m_Combine;
    [SerializeField] UIBattleStageHUD_AlertBoss _m_AlertBoss;
    [SerializeField] UIBattleStageHUD_Wave _m_AlertWave;
    [SerializeField] UIBattleStageHUD_TopInfo _m_TopInfo;

    public GameObject _mObj_CoinCountPivot;

    [SerializeField] Button _Btn_Spawn;

    [SerializeField] TextMeshProUGUI _mText_Gold;
    [SerializeField] TextMeshProUGUI _mText_Dia;
    [SerializeField] TextMeshProUGUI _mText_Supply;

    [SerializeField] string _mStr_SupplyFormat;

    private void Awake()
    {
        _mStr_SupplyFormat = "{0}/{1}";
    }

    public void Start()
    {
        UpdateSpawnButton(PlayerManager.GetInstance().GetGold());
        UpdateGold(PlayerManager.GetInstance().GetGold());
        UpdateDia(PlayerManager.GetInstance().GetDia());
        UpdateSupply(PlayerManager.GetInstance().GetSupply());
    }

    private void OnEnable()
    {
        PlayerManager.GetInstance()._onCB_ChangeGold -= UpdateGold;
        PlayerManager.GetInstance()._onCB_ChangeGold += UpdateGold;

        PlayerManager.GetInstance()._onCB_ChangeGold -= UpdateSpawnButton;
        PlayerManager.GetInstance()._onCB_ChangeGold += UpdateSpawnButton;

        PlayerManager.GetInstance()._onCB_ChangeDia -= UpdateDia;
        PlayerManager.GetInstance()._onCB_ChangeDia += UpdateDia;

        PlayerManager.GetInstance()._onCB_ChangeSupply -= UpdateSupply;
        PlayerManager.GetInstance()._onCB_ChangeSupply += UpdateSupply;

        PlayerManager.GetInstance()._onCB_AlertBoss -= OnNotify_AlertBoss;
        PlayerManager.GetInstance()._onCB_AlertBoss += OnNotify_AlertBoss;

        PlayerManager.GetInstance()._onCB_AlertBoss -= OnNotify_AlertBoss;
        PlayerManager.GetInstance()._onCB_AlertBoss += OnNotify_AlertBoss;

        SceneLoadManager.GetInstance().GetStage(out var _stage);
        if (_stage is BattleStage _battleStage)
        {
            _battleStage._onCB_ChangeStage -= OnNotify_AlertWave;
            _battleStage._onCB_ChangeStage += OnNotify_AlertWave;
        }

        PlayerManager.GetInstance()._onCB_ChangeEnemyCount -= OnNotify_ChangeEnemyCount;
        PlayerManager.GetInstance()._onCB_ChangeEnemyCount += OnNotify_ChangeEnemyCount;
    }
    private void OnDisable()
    {
        PlayerManager.GetInstance()._onCB_ChangeGold -= UpdateGold;

        PlayerManager.GetInstance()._onCB_ChangeGold -= UpdateSpawnButton;

        PlayerManager.GetInstance()._onCB_ChangeDia -= UpdateDia;

        PlayerManager.GetInstance()._onCB_ChangeSupply -= UpdateSupply;

        PlayerManager.GetInstance()._onCB_AlertBoss -= OnNotify_AlertBoss;

        SceneLoadManager.GetInstance().GetStage(out var _stage);
        if (_stage is BattleStage _battleStage)
        {
            _battleStage._onCB_ChangeStage -= OnNotify_AlertWave;
        }

        PlayerManager.GetInstance()._onCB_ChangeEnemyCount -= OnNotify_ChangeEnemyCount;
    }
    public void On_ClickSpawn()
    {
        if (PlayerManager.GetInstance().IsMaxSupply())
            return;

        if (!PlayerManager.GetInstance().IsEnougnGold(Defines.DrawDefaultGoldPrice))
            return;

        int _jobID = DrawCharacterID();
        FindEnableEntityGroups(_jobID, out var _entitiesGroup);

        if (_entitiesGroup == null)
        {
            DrawAnyMapNavigation(out var _Navigation);

            if(_Navigation == null)
            {
                // 만약에 넣을 자리가 없다면, 있는애들 중 뽑고, 스폰해라
                DrawInGroupEntity(out var _reDrawEntityGroup);

                if (_reDrawEntityGroup == null)
                    return; // 실패처리

                _jobID = _reDrawEntityGroup.ID;
                Spawn(_jobID, _reDrawEntityGroup);
            }
            else
            {
                Spawn(_jobID, _Navigation);
            }
        }
        else
        {
            Spawn(_jobID, _entitiesGroup);
        }

        PlayerManager.GetInstance().UseGold(Defines.DrawDefaultGoldPrice); // 스폰했으니 차감
    }

    public void UpdateSpawnButton(int _value)
    {
        if ((_value >= Defines.DrawDefaultGoldPrice) && (!PlayerManager.GetInstance().IsMaxSupply()))
        {
            _Btn_Spawn.interactable = true;
        }
        else
        {
            _Btn_Spawn.interactable = false;
        }
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

    public void DrawInGroupEntity(out EntitiesGroup _ret)
    {
        // 그룹 있는 애들 중에 하나 뽑자
        _ret = null;
        List<EntitiesGroup> _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);

        List<EntitiesGroup> _Lt_DrawPoolGroup = new List<EntitiesGroup>();

        for(int i = 0; i < _Lt_Groups.Count; ++i)
        {
            if (_Lt_Groups[i].IsEnableAddEntity() == true)
            {
                _Lt_DrawPoolGroup.Add(_Lt_Groups[i]);
            }
        }

        if(_Lt_DrawPoolGroup.Count > 0)
        {
            int suffleCount = 20;

            for (int i = 0; i < suffleCount; ++i)
            {
                int _prevIndex = UnityEngine.Random.Range(0, _Lt_DrawPoolGroup.Count);
                int _nextIndex = UnityEngine.Random.Range(0, _Lt_DrawPoolGroup.Count);

                var _temp = _Lt_DrawPoolGroup[_nextIndex];
                _Lt_DrawPoolGroup[_nextIndex] = _Lt_DrawPoolGroup[_prevIndex];
                _Lt_DrawPoolGroup[_prevIndex] = _temp;
            }
        }

        _ret = _Lt_DrawPoolGroup.Count > 0 ? _Lt_DrawPoolGroup[0] : null;
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
        
        if(_Lt_Elements.Count > 0)
        {
            int suffleCount = 20;

            for (int i = 0; i < suffleCount; ++i)
            {
                int _prevIndex = UnityEngine.Random.Range(0, _Lt_Elements.Count);
                int _nextIndex = UnityEngine.Random.Range(0, _Lt_Elements.Count);

                var _temp = _Lt_Elements[_nextIndex];
                _Lt_Elements[_nextIndex] = _Lt_Elements[_prevIndex];
                _Lt_Elements[_prevIndex] = _temp;
            }
        }
        // 셔플 완료

        _retNavigation = _Lt_Elements.Count > 0? _Lt_Elements[0] : null;
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
        _mText_Supply.text = string.Format(_mStr_SupplyFormat, _supply, Defines.NormalSingleGameSupplyMaxCount);
    }

    #endregion

    #region AlertBoss

    public void OnNotify_AlertBoss(int _bossIndex)
    {
        _m_AlertBoss.OnPlayAlertBoss(_bossIndex);
    }
    public void OnNotify_AlertWave(int _waveIndex)
    {
        _m_AlertWave.OnPlayWave(_waveIndex);
    }

    public void OnNotify_ChangeEnemyCount(int _enemyCount)
    {
        _m_TopInfo.OnUpdateEnemySupplyCount(_enemyCount);
    }

    #endregion

}
