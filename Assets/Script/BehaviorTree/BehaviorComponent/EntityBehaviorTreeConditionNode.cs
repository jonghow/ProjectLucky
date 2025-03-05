using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GlobalGameDataSpace;
using System.Text;
using UnityEditor;

namespace EntityBehaviorTree
{
    public interface BTConditionStrategy
    {
        public BTNodeState Check();
        public void Reset();
    }
    public class ConditionUserInputAIStopStategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private Entity _m_CachedOwner;
        private EntityContoller _m_CachedOwnerController;
        private StringBuilder _sb_AnimationClipName;
        public ConditionUserInputAIStopStategy(long _ownerUID)
        {
            _sb_AnimationClipName = new StringBuilder();
            this._ml_ownerUID = _ownerUID;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _m_CachedOwner);
            _m_CachedOwnerController = _m_CachedOwner.Controller;
        }

        public BTNodeState Check()
        {
            if(_m_CachedOwnerController.IsTurnOnAI() != true)
                UpdateAnimation();

            return _m_CachedOwnerController.IsTurnOnAI() == true ? BTNodeState.Failure: BTNodeState.Success;
        }

        public void Reset()
        {
        }
        public void UpdateAnimation()
        {
            UpdateAnimationKey();

            EntityDivision _eDivision = _m_CachedOwner._me_Division;
            string _mActXMLName = "Idle".ToUpper();

            if (_m_CachedOwnerController._m_ActPlayer.IsPlayingEqualAnimation(_sb_AnimationClipName.ToString()) == false)
                _m_CachedOwnerController._m_ActPlayer.PlayAnimationBaseOverride(_eDivision, _m_CachedOwner.JobID, _mActXMLName);
        }

        public void UpdateAnimationKey()
        {
            EntityDivision _eDivision = _m_CachedOwner._me_Division;
            _sb_AnimationClipName.Clear();

            switch (_eDivision)
            {
                case EntityDivision.Player:
                    _sb_AnimationClipName.Append($"Character{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_Idle");
                    break;
                case EntityDivision.Enemy:
                    _sb_AnimationClipName.Append($"Monster{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_Idle");
                    break;
                case EntityDivision.MealFactory:
                    _sb_AnimationClipName.Append($"MealFactory{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_Idle");
                    break;
                case EntityDivision.Neutrality:
                    break;
                case EntityDivision.Deco:
                    break;
                default:
                    break;
            }
        }

    }
    public class ConditionEnemyFindStategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityDivision _me_CachedOwnerDivision;
        private EntityContoller _m_CachedOwnerController;

        public ConditionEnemyFindStategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller;
            _me_CachedOwnerDivision = _entity._me_Division;
        }

        public BTNodeState Check()
        {
            Entity _enemy = null;

            switch (_me_CachedOwnerDivision)
            {
                case EntityDivision.Player:
                    ProcFindPlayerDivisionEnemy(out _enemy);
                    _m_CachedOwnerController.SetChaseEntity(_enemy);
                    break;
                case EntityDivision.Enemy:
                    ProcFindRivalDivisionEnemy(out _enemy);
                    _m_CachedOwnerController.SetChaseEntity(_enemy);
                    break;
                case EntityDivision.Neutrality:
                    break;
                default:
                    break;
            }

            return _enemy != null ? BTNodeState.Failure : BTNodeState.Success;
        }

        public void ProcFindPlayerDivisionEnemy(out Entity _enemy)
        {
            _enemy = null;

            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);

            if (_chaseEntity != null)
            {
                _enemy = _chaseEntity;
            }
            else
            {
                List<Tuple<long, Entity>> _entities;
                List<Entity> _sortedEntities = new List<Entity>();

                EntityManager.GetInstance().GetEntityList(EntityDivision.Enemy, out _entities);

                for (int i = 0; i < _entities.Count; ++i)
                    _sortedEntities.Add(_entities[i].Item2);

                _sortedEntities.Sort(Sorted);

                if (_sortedEntities.Count == 0)
                    return;

                var _abobeNavElement = MapManager.GetInstance().GetMyNavigationByPos3D(_m_CachedOwnerController.Pos3D);

                _m_CachedOwnerController.GetMoveAgent(out EntityMoveAgent _moveAgent);
                _moveAgent.SetStartPoint(_abobeNavElement._mv2_Index);

                _enemy = _sortedEntities[0];
            }
        }

        public void ProcFindRivalDivisionEnemy(out Entity _enemy)
        {
            _enemy = null;

            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);

            if (_chaseEntity != null)
            {
                _enemy = _chaseEntity;
            }
            else
            {
                List<Tuple<long, Entity>> _entities;
                List<Entity> _sortedEntities = new List<Entity>();

                EntityManager.GetInstance().GetEntityList(new EntityDivision[2] { EntityDivision.Player, EntityDivision.MealFactory }, out _entities);

                for (int i = 0; i < _entities.Count; ++i)
                    _sortedEntities.Add(_entities[i].Item2);

                _sortedEntities.Sort(Sorted);

                if (_sortedEntities.Count == 0)
                    return;

                var _abobeNavElement = MapManager.GetInstance().GetMyNavigationByPos3D(_m_CachedOwnerController.Pos3D);

                EntityMoveAgent _moveAgent;
                _m_CachedOwnerController.GetMoveAgent(out _moveAgent);
                _moveAgent.SetStartPoint(_abobeNavElement._mv2_Index);

                _enemy = _sortedEntities[0];
            }
        }
        public int Sorted(Entity _a, Entity _b)
        {
            // Division 우선 정렬 (Player가 가장 높은 우선순위)
            int divisionCompare = _a._me_Division.CompareTo(_b._me_Division);
            if (divisionCompare != 0)
            {
                return divisionCompare;
            }

            // 같은 Division 내에서는 가까운 순으로 정렬
            float _ownerToA = Vector3.SqrMagnitude(_m_CachedOwnerController.Pos3D - _a.Controller.Pos3D);
            float _ownerToB = Vector3.SqrMagnitude(_m_CachedOwnerController.Pos3D - _b.Controller.Pos3D);

            return _ownerToA.CompareTo(_ownerToB); // 가까운 순 정렬
        }

        public void Reset()
        {
        }
    }

    public class ConditionDetectRangeStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityContoller _m_CachedOwnerController;
        private EntityDivision _me_CachedOwnerDivision;
        private float _mf_DetectRange;

        public ConditionDetectRangeStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            EntityManager.GetInstance().GetEntity(_ownerUID, out Entity _entity);
            _m_CachedOwnerController = _entity.Controller;
            _me_CachedOwnerDivision = _entity._me_Division;
            _mf_DetectRange = _entity.Info.DetectRange;
        }
        public BTNodeState Check()
        {
            bool _ret = false;
            switch (_me_CachedOwnerDivision)
            {
                case EntityDivision.Player:
                    DetectPlayerDivision(out _ret);
                    break;
                case EntityDivision.Enemy:
                    DetectEnemyDivision(out _ret);
                    break;
                case EntityDivision.MealFactory:
                    DetectMealFactoryDivision(out _ret);
                    break;
                case EntityDivision.Neutrality:
                    break;
                case EntityDivision.Deco:
                    break;
                default:
                    break;
            }

            return !_ret ? BTNodeState.Failure : BTNodeState.Success;
        }

        public void Reset()
        {
        }

        public void DetectPlayerDivision(out bool _ret)
        {
            _ret = false;

            EntityManager.GetInstance().GetEntityList(new EntityDivision[1] { EntityDivision.Enemy }, out var _listEntities);

            for(int i = 0; i < _listEntities.Count; ++i)
            {
                Vector3 _ownerPos = _m_CachedOwnerController.Pos3D;
                Vector3 _entityPos = _listEntities[i].Item2.Controller.Pos3D;

                if(!MathUtility.CheckOverV3MagnitudeDistance(_ownerPos, _entityPos, _mf_DetectRange))
                {
                    _ret = true;
                    break;
                }
            }
        }

        public void DetectEnemyDivision(out bool _ret)
        {
            _ret = false;

            EntityManager.GetInstance().GetEntityList(new EntityDivision[2] { EntityDivision.Player,EntityDivision.MealFactory }, out var _listEntities);

            for (int i = 0; i < _listEntities.Count; ++i)
            {
                Vector3 _ownerPos = _m_CachedOwnerController.Pos3D;
                Vector3 _entityPos = _listEntities[i].Item2.Controller.Pos3D;

                if (!MathUtility.CheckOverV3MagnitudeDistance(_ownerPos, _entityPos, _mf_DetectRange))
                {
                    _ret = true;
                    break;
                }
            }
        }
        public void DetectMealFactoryDivision(out bool _ret)
        {
            _ret = false;
            EntityManager.GetInstance().GetEntityList(new EntityDivision[1] { EntityDivision.Enemy }, out var _listEntities);

            for (int i = 0; i < _listEntities.Count; ++i)
            {
                Vector3 _ownerPos = _m_CachedOwnerController.Pos3D;
                Vector3 _entityPos = _listEntities[i].Item2.Controller.Pos3D;

                if (!MathUtility.CheckOverV3MagnitudeDistance(_ownerPos, _entityPos, _mf_DetectRange))
                {
                    _ret = true;
                    break;
                }
            }
        }
    }




    public class ConditionRangeStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityContoller _m_CachedOwnerController;
        private float _mf_AttackRange;

        public ConditionRangeStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller;
            _mf_AttackRange = _entity.Info.AttackRange;
        }
        public BTNodeState Check()
        {
            Entity _chaseEntity;
            _m_CachedOwnerController.GetChaseEntity(out _chaseEntity);

            if (_chaseEntity == null)
                return BTNodeState.Failure;

            return MathUtility.CheckOverV2SqrMagnitudeDistance(_m_CachedOwnerController.Pos3D, _chaseEntity.Controller.Pos3D, _mf_AttackRange) ? BTNodeState.Failure : BTNodeState.Success;
        }

        public void Reset()
        {
        }
    }
    public class ConditionRangeInverseStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityContoller _m_CachedOwnerController;
        private float _mf_AttackRange;

        public ConditionRangeInverseStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller;
            _mf_AttackRange = _entity.Info.AttackRange;
        }
        public BTNodeState Check()
        {
            Entity _chaseEntity;
            _m_CachedOwnerController.GetChaseEntity(out _chaseEntity);

            return !MathUtility.CheckOverV2SqrMagnitudeDistance(_m_CachedOwnerController.Pos3D, _chaseEntity.Controller.Pos3D, _mf_AttackRange) ? BTNodeState.Failure : BTNodeState.Success;
        }

        public void Reset()
        {
        }
    }
    public class ConditionPreDelayStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        // 기다리는 시간
        float _waitDelay;
        float _accumulationDelay;
        public ConditionPreDelayStrategy(float _delay)
        {
            this._waitDelay = _delay;
            this._accumulationDelay = 0f;
        }
        public BTNodeState Check()
        {
            //UnityLogger.GetInstance().Log($"ConditionPreDelayStrategy");
            BTNodeState eState = BTNodeState.Success;
            if (CheckDelayTime())
            {
                Reset();
                eState = BTNodeState.Success;
            }
            else
            {
                AddTime();
                eState = BTNodeState.Failure;
            }

            return eState;
        }

        public bool CheckDelayTime()
        {
            if (_waitDelay >= _accumulationDelay)
                return false;

            return true;
        }

        private void AddTime()
        {
            _accumulationDelay += Time.deltaTime;
        }

        public void Reset()
        {
            this._accumulationDelay = 0f;
        }
    }
    public class ConditionEqualNavIndex : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private EntityContoller _m_CachedOwnerController;
        private EntityContoller _m_CachedChasedController;

        Vector2Int _mCacheChasedEntityCurIndex_PrevFrame;
        Vector2Int _mCacheChasedEntityCurIndex_NextFrame;
        
        public ConditionEqualNavIndex(long _ownerUID)
        {
            _mCacheChasedEntityCurIndex_PrevFrame = new Vector2Int(int.MaxValue, int.MaxValue);
            _mCacheChasedEntityCurIndex_NextFrame = new Vector2Int(int.MaxValue, int.MaxValue);

            EntityManager.GetInstance().GetEntity(_ownerUID, out var _m_CachedOwner);
            _m_CachedOwnerController = _m_CachedOwner.Controller;

            UpdateCachedChasedInfos();
        }
        public BTNodeState Check()
        {
            UpdateCachedChasedInfos();

            bool _IsSame = true;
            if(_mCacheChasedEntityCurIndex_PrevFrame != _mCacheChasedEntityCurIndex_NextFrame)
            {
                _IsSame = false;
                _mCacheChasedEntityCurIndex_PrevFrame = _mCacheChasedEntityCurIndex_NextFrame;
            }

            return _IsSame ? BTNodeState.Failure : BTNodeState.Success;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void UpdateCachedChasedInfos()
        {
            _m_CachedOwnerController.GetChaseEntity(out var _chasedEntity);
            if (_chasedEntity == null) return;

            _m_CachedChasedController = _chasedEntity.Controller;
            _m_CachedOwnerController.GetMoveAgent(out EntityMoveAgent _entityMoveAgent);
            _entityMoveAgent.GetPathFinder(out EntityPathFinder _entityPathFinder);

            _mCacheChasedEntityCurIndex_NextFrame = _entityPathFinder.GetCurrentIndex();
        }
    }

    public class ConditionCheckEntityDeadStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityContoller _m_CachedOwnerController;
        private EntityInfo _m_CachedOwnerInfo;

        public ConditionCheckEntityDeadStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller;
            _m_CachedOwnerInfo = _entity.Info;
        }
        public BTNodeState Check()
        {
            return _m_CachedOwnerInfo.HP > 0 ? BTNodeState.Failure : BTNodeState.Success;
        }
        public void Reset()
        {
        }
    }
    public class EntityBehaviorTreeConditionNode : EntityBehaviorTreeNodeBase
    {
        BTConditionStrategy _strategy;
        public EntityBehaviorTreeConditionNode(BTConditionStrategy _strategy)
        {
            this._strategy = _strategy;
        }
        public override BTNodeState Evaluate()
        {
            return _strategy.Check();
        }

        public void Reset()
        {
        }
    }


    #region MealFactoryStrategy 

    /// <summary>
    /// MealFactory전용 Strategy AI 입니다. 
    /// 개체 생성량이 한계량 보다 많은지 체크합니다.
    /// </summary>
    public class ConditionCheckChildEntityLimitStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityMealFactoryController _m_CachedOwnerController;
        private GameDB_MealKitInfo _m_CachedMealKitInfo;

        public ConditionCheckChildEntityLimitStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            UpdateEssentialData();
        }
        public BTNodeState Check()
        {
            UpdateEssentialData();

            int _limitCount = _m_CachedMealKitInfo._mi_CreateCount;
            int _createCount = _m_CachedOwnerController.CreateEntityCount;

            return _createCount < _limitCount ? BTNodeState.Failure : BTNodeState.Success;
        }
        public void Reset()
        {
        }

        public void UpdateEssentialData()
        {
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller as EntityMealFactoryController;
            _m_CachedMealKitInfo = _m_CachedOwnerController.GetMealKitInfo();
        }
    }

    /// <summary>
    /// StoreFactory 전용 Strategy AI 입니다. 
    /// 개체 생성을 다했는지 체크합니다.
    /// </summary>
    public class ConditionCheckChildEntityStoreFactoryStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityStoreFactoryController _m_CachedOwnerController;

        public ConditionCheckChildEntityStoreFactoryStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            UpdateEssentialData();
        }

        public BTNodeState Check()
        {
            UpdateEssentialData();

            int _limitCount = _m_CachedOwnerController._mi_LimitCount;
            int _createCount = _m_CachedOwnerController._mi_CreateCount;

            return _createCount < _limitCount ? BTNodeState.Success : BTNodeState.Failure;
        }
        public void Reset()
        {
        }

        public void UpdateEssentialData()
        {
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out Entity _entity);
            _m_CachedOwnerController = _entity.Controller as EntityStoreFactoryController;
        }
    }


    /// <summary>
    /// MealFactory 전용Strategy AI 입니다. 
    /// OverHeating Time인지 체크합니다.
    /// </summary>
    public class ConditionCheckOverHeatingStrategy: EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityMealFactoryController _m_CachedOwnerController;
        private GameDB_MealKitInfo _m_CachedMealKitInfo;
        private float _mf_CachedOverHeatingTime;
        private float _mf_AccumulationDelay;

        public ConditionCheckOverHeatingStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            _mf_AccumulationDelay = 0f;
            UpdateEssentialData();
        }
        public BTNodeState Check()
        {
            UpdateOverWholeData();

            BTNodeState eState = BTNodeState.Success;

            if(_m_CachedMealKitInfo == null)
            {
                Reset();
                eState = BTNodeState.Success;
            }
            else if (CheckDelayTime())
            {
                Reset();
                eState = BTNodeState.Success;
            }
            else
            {
                AddTime();
                eState = BTNodeState.Failure;
            }

            return eState;
        }

        public void UpdateEssentialData()
        {
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller as EntityMealFactoryController;
            _m_CachedMealKitInfo = _m_CachedOwnerController.GetMealKitInfo();
        }
        public void UpdateOverWholeData()
        {
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller as EntityMealFactoryController;
            _m_CachedMealKitInfo = _m_CachedOwnerController.GetMealKitInfo();
            _mf_CachedOverHeatingTime = _m_CachedMealKitInfo._mf_HeatingTime;
        }
        public void Reset()
        {
            _mf_AccumulationDelay = 0f;
        }
        public void AddTime()
        {
            _mf_AccumulationDelay += Time.deltaTime;
        }
        public bool CheckDelayTime()
        {
            if (_mf_CachedOverHeatingTime >= _mf_AccumulationDelay)
                return false;

            return true;
        }
    }
    /// <summary>
    /// MealFactory 전용 Strategy AI 입니다. 
    /// 현재 요리를 만드는 중입니다.
    /// </summary>
    /// </summary>
    public class ConditionProcessCookingTimeStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityMealFactoryController _m_CachedOwnerController;
        private GameDB_MealKitInfo _m_CachedMealKitInfo;
        private float _mf_CachedCookingTime;
        private float _mf_AccumulationDelay;

        public ConditionProcessCookingTimeStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            _mf_AccumulationDelay = 0f;
            UpdateEssentialData();
        }
        public BTNodeState Check()
        {
            UpdatewholeData();

            BTNodeState eState = BTNodeState.Success;

            if (_m_CachedMealKitInfo == null)
            {
                Reset();
                eState = BTNodeState.Success;
            }
            else if (CheckDelayTime())
            {
                Reset();
                eState = BTNodeState.Success;
            }
            else
            {
                AddTime();
                eState = BTNodeState.Failure;
            }

            return eState;
        }
        public void UpdateEssentialData()
        {
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller as EntityMealFactoryController;
            _m_CachedMealKitInfo = _m_CachedOwnerController.GetMealKitInfo();
        }

        public void UpdatewholeData()
        {
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller as EntityMealFactoryController;
            _m_CachedMealKitInfo = _m_CachedOwnerController.GetMealKitInfo();
            _mf_CachedCookingTime = _m_CachedMealKitInfo._mf_CookTime;
        }
        public void Reset()
        {
            _mf_AccumulationDelay = 0f;
        }
        public void AddTime()
        {
            _mf_AccumulationDelay += Time.deltaTime;
        }
        public bool CheckDelayTime()
        {
            if (_mf_CachedCookingTime >= _mf_AccumulationDelay)
                return false;

            return true;
        }
    }
    /// <summary>
    /// MealFactory 전용 Strategy AI 입니다. 현재 솥에 밀키트가 부어져 있는지 체크합니다.
    /// </summary>
    public class ConditionIsContainMealkitStrategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        private long _ml_ownerUID;
        private EntityMealFactoryController _m_CachedOwnerController;
        private GameDB_MealKitInfo _m_CachedMealKitInfo;

        public ConditionIsContainMealkitStrategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            UpdateData();
        }
        public BTNodeState Check()
        {
            UpdateData();
            return _m_CachedMealKitInfo != null ? BTNodeState.Failure : BTNodeState.Success;
        }
        public void UpdateData()
        {
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller as EntityMealFactoryController;
            _m_CachedMealKitInfo = _m_CachedOwnerController.GetMealKitInfo();
        }
        public void Reset()
        {
        }
    }

    #endregion




}


