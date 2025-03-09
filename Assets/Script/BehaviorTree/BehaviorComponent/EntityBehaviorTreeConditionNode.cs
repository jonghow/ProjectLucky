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
                case EntityDivision.Rival:
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
                case EntityDivision.Rival:
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
                case EntityDivision.Rival:
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

            Vector3 _chassPos3D = _chaseEntity == null ? new Vector3(99999f, 99999f, 99999f) : _chaseEntity.Controller.Pos3D;
            return !MathUtility.CheckOverV2SqrMagnitudeDistance(_m_CachedOwnerController.Pos3D, _chassPos3D, _mf_AttackRange) ? BTNodeState.Failure : BTNodeState.Success;
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
}


namespace EntityBehaviorTree
{
    // RivalPlayer Type AI
    public class ConditionRivalPlayerInputAIStopStategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;

        public ConditionRivalPlayerInputAIStopStategy()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);


        }

        public BTNodeState Check()
        {
            return m_CacheRivalPlayerAI.IsTurnOnAI() == true ? BTNodeState.Failure : BTNodeState.Success;
        }

        public void Reset()
        {
        }
    }

    public class ConditionIsOverSupplyStategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;

        public ConditionIsOverSupplyStategy()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);
        }

        public BTNodeState Check()
        {
            return RivalPlayerAIManager.GetInstance().IsMaxSupply() == true ? BTNodeState.Success : BTNodeState.Failure;
            // Max 일때 진행하면 안되니까 Sequence에서는 Success로 멈춘다.
        }

        public void Reset()
        {
        }
    }
    public class ConditionConsumableDiaDrawPriceStategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;

        public ConditionConsumableDiaDrawPriceStategy()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);
        }

        public BTNodeState Check()
        {
            int _drawTarget = UnityEngine.Random.Range(0, 3);
            int _price = 0;

            switch (_drawTarget)
            {
                case 0:
                    _price = Defines.DrawDiaPriceUncommon;
                    break;
                case 1:
                    _price = Defines.DrawDiaPriceHero;
                    break;
                case 2:
                    _price = Defines.DrawDiaPriceMyth;
                    break;
            }

            bool _ret = RivalPlayerAIManager.GetInstance().EnableUseDia(_price);

            if (_ret == true)
                m_CacheRivalPlayerAI.SetDrawTarget(_drawTarget);

            return _ret ? BTNodeState.Success : BTNodeState.Failure;
        }

        public void Reset()
        {
        }
    }
    public class ConditionConsumableGoldDrawPriceStategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;

        public ConditionConsumableGoldDrawPriceStategy()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);
        }

        public BTNodeState Check()
        {
            int _drawTarget = UnityEngine.Random.Range(0, 3);
            int _price = Defines.DrawDefaultGoldPrice;

            return RivalPlayerAIManager.GetInstance().EnableUseGold(_price) ? BTNodeState.Success : BTNodeState.Failure;
        }

        public void Reset()
        {
        }
    }
    public class ConditionEnableCombineStategy : EntityBehaviorTreeNodeBase, BTConditionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;
        public ConditionEnableCombineStategy()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);
        }

        public BTNodeState Check()
        {
            bool _ret = false;

            if(IsEnableCombineLowerHero())
            {
                _ret = true;
            }
            else if (IsEnableCombineUpperHero())
            {
                _ret = true;
            }
            else
            {
                m_CacheRivalPlayerAI.SetCombineID(0);
                m_CacheRivalPlayerAI.SetCombineUID(0);
                m_CacheRivalPlayerAI.SetRecipeCombine(false);
            }

            // Lower에서 Enable Combine 개체를 찾지 못하면, Upper 도 탐색 해본다.
            // 만약 Lower Upper 다 없다면, 이건 이제 가능한 것이 없음

            return _ret ? BTNodeState.Success : BTNodeState.Failure;
        }

        public bool IsEnableCombineLowerHero()
        {
            bool _ret = false;

            List<EntitiesGroup> _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival);

            for(int i = 0; i < _Lt_Groups.Count; ++i)
            {
                EntitiesGroup _groups = _Lt_Groups[i];

                if(_groups.GetEntityGrade() < EntityGrade.Hero)
                {
                    if (_groups.Count == 3)
                    {
                        int _mi_ID = _groups.ID;
                        long _ml_UID = _groups.UniqueID;

                        m_CacheRivalPlayerAI.SetCombineID(_mi_ID);
                        m_CacheRivalPlayerAI.SetCombineUID(_ml_UID);
                        m_CacheRivalPlayerAI.SetRecipeCombine(false);

                        _ret = true;
                        break;
                    }
                }
            }

            return _ret;
        }

        public bool IsEnableCombineUpperHero()
        {
            bool _ret = false;

            GameDataManager.GetInstance().GetRecipeDatasToList(out List<GameDB_MealRecipe> _Lt_recipe);

            for(int i = 0; i < _Lt_recipe.Count; ++i)
            {
                GameDB_MealRecipe _recipe = _Lt_recipe[i];

                if(IsCollectRecipe(_recipe) == true)
                {
                    m_CacheRivalPlayerAI.SetCombineID(_recipe._mi_ID);
                    m_CacheRivalPlayerAI.SetCombineUID(0);
                    m_CacheRivalPlayerAI.SetRecipeCombine(true);
                    _ret = true;
                    break;
                }
            }

            return _ret;
        }

        public bool IsCollectRecipe(GameDB_MealRecipe _recipe)
        {
            bool _ret = false;

            List<EntitiesGroup> _Lt_Entities = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival);

            int[] _mArr_Recipe = _recipe.Arr_Recipe;

            List<EntitiesGroup> _Lt_UseCandidate_CountSindle = new List<EntitiesGroup>();
            List<EntitiesGroup> _Lt_UseCandidate_CountMulti = new List<EntitiesGroup>();

            int _passCount = 0;

            for (int i = 0; i < _mArr_Recipe.Length; ++i)
            {
                int _characterID = _mArr_Recipe[i];

                if(_Lt_Entities.Exists(rhs => rhs.ID == _characterID))
                {
                    var _entityGroup = _Lt_Entities.Find(rhs => rhs.ID == _characterID);
                    if(_entityGroup.Count ==1)
                    {
                        _Lt_UseCandidate_CountSindle.Add(_entityGroup);
                        // 용병 삭제
                    }
                    else
                    {
                        _Lt_UseCandidate_CountMulti.Add(_entityGroup);
                    }
                    ++_passCount;
                }
            }

            if (_passCount == _mArr_Recipe.Length)  // 통과 카운터와 재료 갯수가 맞지 않으면 수행하지 않음
                _ret = true;

            return _ret;
        }





        public void Reset()
        {
        }
    }




}


