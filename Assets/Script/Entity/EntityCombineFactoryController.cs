using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;
using GlobalGameDataSpace;
using System.Threading;
using System;

public class EntityCombineFactoryController : EntityContoller
{
    // ���� ���� ��� ����?
    private Avatar _avatar; // ����ȭ�� �� ���̽�
    private EntityBehaviorTreeBase _behaviorTree;

    #region EntityMealFactoryController Data
    
    protected CancellationTokenSource _m_CreateCancellation;
    protected MealFactoryData _m_OriginMealFactoryData; // ������ ���� ������
    protected MealFactoryData _m_OriginAdvancedFactoryData; //  ������ ���׷��̵� ������

    public List<MealFactoryAdvanceCommandBase> _mLt_AdvCommand;
    #endregion

    public override void SetUp(long _entityUID, int _entityID)
    {
        _ml_EntityUID = _entityUID;
        _mi_EntityTID = _entityID;
        
        TurnOnAI();

        //TurnOffAI(); // �ʱ�� ����.
        TransformSetUp();
        SetUpMealFactoryFunc();

        _m_ActPlayer.SetOwnerUID(_entityUID, _entityID);
    }

    public void SetUpMealFactoryFunc()
    {
        _m_CreateCancellation = new CancellationTokenSource();
        _mLt_AdvCommand = new List<MealFactoryAdvanceCommandBase>();
    }
    public void AISetUp()
    {
        //_behaviorTree = new MealFactoryBehaviorNormalType($"MealFactory", _ml_EntityUID, this);
        //_behaviorTree.Evaluate();
    }
    private void TransformSetUp()
    {
        _mTr_WorldObject = GetComponent<Transform>();
    }

    private void Update()
    {
        // AI ��
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

        float _magDistance = 1.5f; // �ּ� �Ÿ�
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
            ReleaseMaterials(_entityList);

            if(_recipe._mi_TechTree < 2)
            {
                // World �� ����
                CreateCombineCharacter(_recipe._mi_MealKitID);
            }
            else
            {
                // ��ŰƮ ī��� ����
                DrawMealKitCard(_recipe._mi_MealKitID);
            }

            //UnityLogger.GetInstance().LogWarning($"Recipe ID ::{_recipe._mi_ID}");
            //UnityLogger.GetInstance().LogWarning($"Create Entity ID ::{_recipe._mi_MealKitID}");
        }
    }

    public int SortedByID(Entity _item1, Entity _item2)
    {
        int _item1ID = _item1.JobID;
        int _item2ID = _item2.JobID;

        return _item1ID.CompareTo(_item2ID);
    }

    public void ReleaseMaterials(List<Entity> _entityList)
    {
        for (int i = 0; i < _entityList.Count; ++i)
        {
            _entityList[i].Controller?._onCB_DiedProcess?.Invoke();

            EntityManager.GetInstance().RemoveEntity(_entityList[i].UID);
            GameObject.Destroy(_entityList[i].gameObject);
        }
    }

    public void CreateCombineCharacter(int _characterID)
    {
        var _creator = new UserEntityFactory();

        GetMoveAgent(out var _moveAgent);
        _moveAgent.GetPathFinder(out var _pathfinder);

        MapManager.GetInstance().GetSpawnableCandidate(_pathfinder.GetCurrentIndex(), _pathfinder.GetCurrentIndex(), out var _Lt_candidate);
        Vector3 _spawnPos = _Lt_candidate.Count > 0 ? _Lt_candidate[0]._mv3_Pos : this.Pos3D;

        _ = _creator.CreateEntity(_characterID, _spawnPos, (_createEntity) =>
        {
            _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
        });
    }

    public void DrawMealKitCard(int _cardID)
    {
        HandCardManager.GetInstance().CommandGetCardByID( _cardID);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this._mv2_Pos, 0.1f);
    }

    public override void OnDieEvent(Entity _entity)
    {
        ReleaseBuildSpaceNav();
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
    /// �����ߴ� �׺���̼� �޽��� �ٽ� �����ݴϴ�.
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
