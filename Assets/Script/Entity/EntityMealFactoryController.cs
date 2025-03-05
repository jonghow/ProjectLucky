using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;
using GlobalGameDataSpace;
using System.Threading;

public class EntityMealFactoryController : EntityContoller 
{
    // 월드 모델은 어디서 관리?
    private Avatar _avatar; // 공용화된 모델 베이스
    private EntityBehaviorTreeBase _behaviorTree;

    #region EntityMealFactoryController Data
    
    protected CancellationTokenSource _m_CreateCancellation;
    protected MealFactoryData _m_OriginMealFactoryData; // 공장의 원래 데이터
    protected MealFactoryData _m_OriginAdvancedFactoryData; //  공장의 업그레이드 데이터

    public List<MealFactoryAdvanceCommandBase> _mLt_AdvCommand;
    #endregion

    #region CreateEntityData

    private GameDB_MealKitInfo _m_RegistedMealKitInfo;
    protected List<Entity> _createEntities;
    public int CreateEntityCount => _createEntities.Count;
    #endregion

    public override void SetUp(long _entityUID, int _entityID)
    {
        _ml_EntityUID = _entityUID;
        _mi_EntityTID = _entityID;
        _m_RegistedMealKitInfo = null;

        
        TurnOnAI();

        //TurnOffAI(); // 초기는 끈다.
        TransformSetUp();
        SetUpMealFactoryFunc();

        _m_ActPlayer.SetOwnerUID(_entityUID, _entityID);
    }

    public void SetUpMealFactoryFunc()
    {
        _m_CreateCancellation = new CancellationTokenSource();
        _mLt_AdvCommand = new List<MealFactoryAdvanceCommandBase>();
        _createEntities = new List<Entity>();
    }
    public void AISetUp()
    {
        _behaviorTree = new MealFactoryBehaviorNormalType($"MealFactory", _ml_EntityUID, this);
        //_behaviorTree.Evaluate();
    }
    private void TransformSetUp()
    {
        _mTr_WorldObject = GetComponent<Transform>();
    }

    private void Update()
    {
        // AI 평가
        if(_behaviorTree != null)
            _behaviorTree.Evaluate();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this._mv2_Pos, 0.1f);
    }

    #region UpgradeCommand
    public virtual void UpdateCommand()
    {
        for (int i = 0; i < _mLt_AdvCommand.Count; ++i)
        {
            var _command = _mLt_AdvCommand[i];
            _command.Upgrade(this);
        }
    }
    public void UpdateEntityLimitCount(int _count)
    {
        _m_OriginAdvancedFactoryData.UpdateEntityLimitCount(_count);
    }
    public void UpdateEntityGradeUp(int _grade)
    {
        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"UpdateEntityGradeUp", $"미구현 입니다.");
        return;
    }
    public void UpdateReduceCookTime(float _reducePercent)
    {
        _m_OriginAdvancedFactoryData.UpdateReducePercentCookTime(_reducePercent);
    }
    public void UpdateReduceOverHeatingTime(float _reducePercent)
    {
        _m_OriginAdvancedFactoryData.UpdateReducePercentOverheatingTime(_reducePercent);
    }
    public MealFactoryData GetOriginMealFactoryData() => _m_OriginMealFactoryData;
    public MealFactoryData GetAdvanceMealFactoryData() => _m_OriginAdvancedFactoryData;
    #endregion

    #region MealKitInfo
    public bool IsEqualMealKitInfo(int _mealKitID)
    {
        if (_m_RegistedMealKitInfo == null)
            return false;

        return _m_RegistedMealKitInfo._mi_ID == _mealKitID;
    }
    public void RegistMealKit(int _mealKitID)
    {
        _m_RegistedMealKitInfo = GameDataManager.GetInstance().GetMealKitInfo(_mealKitID);
    }
    public void ClearMealKit()
    {
        _m_RegistedMealKitInfo = null;
    }
    public GameDB_MealKitInfo GetMealKitInfo() => _m_RegistedMealKitInfo;
    public void CreateMealKitMercenaryEntity()
    {
        if(_m_RegistedMealKitInfo == null)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"CreateMealKitMercenaryEntity", $"MealKitInfo is NULL");
            return;
        }

        int _limitMealKitEntityCount = _m_RegistedMealKitInfo._mi_CreateLimit;

        if (CreateEntityCount >= _limitMealKitEntityCount)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"CreateMealKitMercenaryEntity", $"Over Entity Count By Limit Count");
            return;
        }

        Vector3 _mv3_mealFactoryPos = _mv3_Pos;
        _mv3_mealFactoryPos.y -= 0.5f;

        UserEntityFactory _creator = new UserEntityFactory();

        _ = _creator.CreateEntity(_m_RegistedMealKitInfo._mi_CreateEntityID, _mv3_mealFactoryPos, (_createEntity) => 
        {
            _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };

            _createEntities.Add(_createEntity);
        });
    }
    public override void OnDieEvent(Entity _entity)
    {
        ReleaseBuildSpaceNav();
        _createEntities.Remove(_entity);
        _m_ActPlayer.ClearActionInfos();

        if (PlayerManager.GetInstance().GetSelectedEntity() != null)
        {
            if (PlayerManager.GetInstance().GetSelectedEntity().UID == _entity.UID)
            {
                PlayerManager.GetInstance().ClearSelectedEntity();
                // Entity Clear
            }
        }
    }

    /// <summary>
    /// 점유했던 네비게이션 메쉬를 다시 돌려줍니다.
    /// </summary>
    public void ReleaseBuildSpaceNav()
    {
        GetMoveAgent(out var _moveAgent);
        _moveAgent.GetPathFinder(out var _pathfinder);
        _pathfinder.GetCurrentIndex();

        Vector2Int _mv2_Origin = _pathfinder.GetCurrentIndex();

        int buildID = _mi_EntityTID;
        GameDataManager.GetInstance().GetGameDBBuildingInfo(buildID, out var _buildingData);

        GridDirectionGroup[] _buildDirectionGroup = _buildingData._meAr_BuildGrid;
        MapManager.GetInstance().SetEnableNavigationElement(_mv2_Origin, _buildDirectionGroup);
    }

    #endregion
}
