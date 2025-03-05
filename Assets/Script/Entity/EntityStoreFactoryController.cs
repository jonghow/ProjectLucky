using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;
using GlobalGameDataSpace;
using System.Threading;
using System;

public class EntityStoreFactoryController : EntityContoller
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

    private List<int> _m_Lt_CreateID;
    protected List<Entity> _createEntities;

    public int _mi_LimitCount;
    public int _mi_CreateCount;
    public int LtCreateEntityCount => _createEntities.Count;
    #endregion

    public override void SetUp(long _entityUID, int _entityID)
    {
        _ml_EntityUID = _entityUID;
        _mi_EntityTID = _entityID;

        _m_Lt_CreateID = new List<int>();
        _m_Lt_CreateID.Add(1);
        _m_Lt_CreateID.Add(2);
        _m_Lt_CreateID.Add(3);
        _m_Lt_CreateID.Add(8);
        _m_Lt_CreateID.Add(9);
        //_m_Lt_CreateID.Add(4);

        _mi_LimitCount = 4;
        _mi_CreateCount = 0;

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
        _behaviorTree = new MealFactoryBehaviorStoreType($"StoreFactory", _ml_EntityUID, this);
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

        // Test Key 

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            UnityLogger.GetInstance().Log($"Key Number 1!!");
            Execute_CombineByRecipe();
        }
    }

    public void Execute_CombineByRecipe()
    {
        List<Tuple<long, Entity>> _entities;
        EntityManager.GetInstance().GetEntityList(new EntityDivision[1] { EntityDivision.Player}, out _entities);

        if (_entities == null || _entities.Count == 0)
            return;

        float _magDistance = 1.5f; // 최소 거리
        List<Entity> _entityList = new List<Entity>();
        Vector3 _vOriginPos = this.transform.position;

        if (_entities != null)
        {
            foreach (var _entityPair in _entities)
            {
                Entity _curEntity = _entityPair.Item2;
                EntityContoller _controller = _curEntity.Controller;

                if (Vector3.SqrMagnitude(_vOriginPos - _controller.Pos3D) <= _magDistance)
                {
                    _entityList.Add(_curEntity);
                }
            }
        }

        _entityList.Sort(SortedByID);

        if(GameDataManager.GetInstance().TryGetMealRecipe(_entityList, out var _recipe))
        {
            UnityLogger.GetInstance().LogWarning($"Recipe ID ::{_recipe._mi_ID}");
            UnityLogger.GetInstance().LogWarning($"Create Entity ID ::{_recipe._mi_MealKitID}");
        }
    }

    public int SortedByID(Entity _item1, Entity _item2)
    {
        int _item1ID = _item1.JobID;
        int _item2ID = _item2.JobID;

        return _item1ID.CompareTo(_item2ID);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this._mv2_Pos, 0.1f);
    }

    public void CreateMealKitMercenaryEntity()
    {
        int _limitMealKitEntityCount = _mi_LimitCount;

        if (_mi_CreateCount >= _limitMealKitEntityCount)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"CreateMealKitMercenaryEntity", $"Over Entity Count By Limit Count");
            return;
        }

        Vector3 _mv3_mealFactoryPos = _mv3_Pos;
        _mv3_mealFactoryPos.y -= 0.5f;

        UserEntityFactory _creator = new UserEntityFactory();

        int _randomCount = 10; // 셔플

        List<int> _Lt_RandomBallBox = new List<int>();
        _Lt_RandomBallBox.AddRange(_m_Lt_CreateID);

        for(int i = 0; i < _randomCount; ++i)
        {
            int prevIndex = UnityEngine.Random.Range(0, _Lt_RandomBallBox.Count); // Int 형은 마지막 수는 안뽑음
            int nextIndex = UnityEngine.Random.Range(0, _Lt_RandomBallBox.Count); // Int 형은 마지막 수는 안뽑음

            int temp = _Lt_RandomBallBox[prevIndex];
            _Lt_RandomBallBox[prevIndex] = _Lt_RandomBallBox[nextIndex];
            _Lt_RandomBallBox[nextIndex] = temp;
        }

        int _randomIndex = _Lt_RandomBallBox[0];

        _mi_CreateCount += 1;

        _ = _creator.CreateEntity(_randomIndex, _mv3_mealFactoryPos, (_createEntity) =>
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

        if(PlayerManager.GetInstance().GetSelectedEntity() != null)
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
}
