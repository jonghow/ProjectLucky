using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GlobalGameDataSpace;
using System.Reflection;
using System.Text;
using UnityEditor.Hardware;
using System.Linq;

namespace EntityBehaviorTree
{
    public interface BTActionStrategy
    {
        public BTNodeState Run();
        public void Reset();
    }
    public class EnemyFindStategy : BTActionStrategy
    {
        private long _ml_ownerUID;
        private EntityDivision _me_CachedOwnerDivision;
        private Entity _m_CachedOwner;
        private EntityContoller _m_CachedOwnerController;

        public EnemyFindStategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _m_CachedOwner);
            _m_CachedOwnerController = _m_CachedOwner.Controller;
            _me_CachedOwnerDivision = _m_CachedOwner._me_Division;
        }

        public void ProcFindPlayerDivisionEnemy(out Entity _enemy)
        {
            _enemy = null;

            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);


            if (_chaseEntity != null && CheckRange())
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

                NavigationElement _abobeNavElement = null; 

                if(_me_CachedOwnerDivision == EntityDivision.Player)
                {
                    _abobeNavElement = MapManager.GetInstance().GetMyNavigationByPos3D(_m_CachedOwnerController.Pos3D);
                }
                else if (_me_CachedOwnerDivision == EntityDivision.Rival)
                {
                    _abobeNavElement = RivalMapManager.GetInstance().GetMyNavigationByPos3D(_m_CachedOwnerController.Pos3D);
                }

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

        public bool CheckRange()
        {
            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);

            Vector3 chasedEntityPos = _chaseEntity.Controller.Pos3D;
            Vector3 ownerEntityPos = _m_CachedOwnerController.Pos3D;

            float _attackRange = _m_CachedOwner.Info.AttackRange;

            if (MathUtility.CheckOverV3MagnitudeDistance(chasedEntityPos, ownerEntityPos, _attackRange))
                return false;

            return true;
        }

        public void Reset()
        {
        }

        public BTNodeState Run()
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
    }

    public class EnemyExistCheckStategy : BTActionStrategy
    {
        private long _ml_ownerUID;
        private EntityDivision _me_CachedOwnerDivision;
        private Entity _m_CachedOwner;
        private EntityContoller _m_CachedOwnerController;

        public EnemyExistCheckStategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _m_CachedOwner);
            _m_CachedOwnerController = _m_CachedOwner.Controller;
            _me_CachedOwnerDivision = _m_CachedOwner._me_Division;
        }

        public void ProcFindPlayerDivisionEnemy(out Entity _enemy)
        {
            _enemy = null;

            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);

            if (_chaseEntity != null && CheckRange())
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

        public bool CheckRange()
        {
            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);

            Vector3 chasedEntityPos = _chaseEntity.Controller.Pos3D;
            Vector3 ownerEntityPos = _m_CachedOwnerController.Pos3D;

            float _attackRange = _m_CachedOwner.Info.AttackRange;

            if (MathUtility.CheckOverV3MagnitudeDistance(chasedEntityPos, ownerEntityPos, _attackRange))
                return false;

            return true;
        }

        public void Reset()
        {
        }

        public BTNodeState Run()
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
    }

    public class ChaseStrategy : BTActionStrategy
    {
        private long _ml_ownerUID;

        private Entity _m_CachedOwner;
        private EntityContoller _m_CachedOwnerController;
        private EntityMoveAgent _m_Agent;
        private EntityPathFinder _m_PathFinder;

        private Entity _m_ChasedEntity;

        private float waittime = 0f;

        private StringBuilder _sb_AnimationClipName;

        public ChaseStrategy(long _ownerUID)
        {
            _sb_AnimationClipName = new StringBuilder();
            _ml_ownerUID = _ownerUID;
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwner = _entity;
            _m_CachedOwnerController = _entity.Controller;

            EntityMoveAgent _agent;
            _entity.Controller.GetMoveAgent(out _agent);
            _m_Agent = _agent;

            EntityPathFinder _pathfinder;
            _agent.GetPathFinder(out _pathfinder);
            _m_PathFinder = _pathfinder;

            UpdateChaseEntity();
        }

        public BTNodeState Run()
        {
            //UnityLogger.GetInstance().Log($"ChaseStrategy");
            if(_m_Agent.IsProcMoveAgent())
            {
                waittime += Time.deltaTime;

                if(waittime > 0.3f)
                {
                    waittime = 0f;
                    _m_Agent.SetProcMoveAgent(false);
                } // 0.5초 지나면 푼다./

                return BTNodeState.Running;
            }

            UpdateChaseEntity();

            if(_m_ChasedEntity != null)
            {
                UpdateAnimation();

                _m_ChasedEntity.Controller.GetMoveAgent(out EntityMoveAgent _chasedMoveAgent);
                _chasedMoveAgent.GetPathFinder(out EntityPathFinder _chasedPathFinder);

                _m_CachedOwnerController.GetMoveAgent(out EntityMoveAgent _entityMoveAgent);
                _entityMoveAgent.GetPathFinder(out EntityPathFinder _entityPathFinder);

                Vector2Int _myIndex = _entityPathFinder.GetCurrentIndex();

                Vector2Int _chaseIndex = _chasedPathFinder.GetCurrentIndex(); 
                MapManager.GetInstance().GetMoveableCandidate(_m_CachedOwner, _chaseIndex, _myIndex, out var _Lt_MoveCandidate);

                Vector2Int _candidateIndex = _Lt_MoveCandidate.Count != 0 ? _Lt_MoveCandidate[0]._mv2_Index : _chaseIndex;
                _m_Agent.CommandBattleMove(_candidateIndex);

                //UnityLogger.GetInstance().LogWarning($"{_candidateIndex}");

                if(_m_Agent.IsProcMoveAgent())
                    return BTNodeState.Running;
            }

            return BTNodeState.Failure;
        }

        public void UpdateChaseEntity()
        {
            Entity _chaseEntity;
            _m_CachedOwnerController.GetChaseEntity(out _chaseEntity);
            _m_ChasedEntity = _chaseEntity;
        }

        public void UpdateAnimation()
        {
            UpdateAnimationKey();

            EntityDivision _eDivision = _m_CachedOwner._me_Division;
            string _mActXMLName = "Move".ToUpper();

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
                    _sb_AnimationClipName.Append($"Character{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_Move");
                    break;
                case EntityDivision.Enemy:
                    _sb_AnimationClipName.Append($"Monster{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_Move");
                    break;
                case EntityDivision.MealFactory:
                    _sb_AnimationClipName.Append($"MealFactory{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_Move");
                    break;
                case EntityDivision.Neutrality:
                    break;
                case EntityDivision.Deco:
                    break;
                default:
                    break;
            }
        }


        public void Reset() { }
    }

    public class IdleStrategy : BTActionStrategy
    {
        private Entity _m_CachedOwner;
        private EntityContoller _m_CachedOwnerController;
        private EntityMoveAgent _m_CachedMoveAgent;
        public long _ml_ownerUID;
        public int _mi_ownerJobID;
        public StringBuilder _sb_AnimationClipName;

        public IdleStrategy(long _ownerUID)
        {
            _sb_AnimationClipName = new StringBuilder();
            // 평타는 보통 타겟을 정해서 발사한다.
            this._ml_ownerUID = _ownerUID;

            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _m_CachedOwner);
            _m_CachedOwnerController = _m_CachedOwner.Controller;
            _mi_ownerJobID = _m_CachedOwner.JobID;

            _m_CachedOwnerController.GetMoveAgent(out _m_CachedMoveAgent);
        }
        public BTNodeState Run()
        {
            string _mActXMLName = $"IDLE".ToUpper();
            // 클립

            UpdateAnimationKey();
            EntityDivision _eDivision = _m_CachedOwner._me_Division;

            if (_m_CachedOwnerController._m_ActPlayer.IsPlayingEqualAnimation(_sb_AnimationClipName.ToString()) == false)
            {
                _m_CachedOwnerController._m_ActPlayer.PlayAnimationBaseOverride(_eDivision, _mi_ownerJobID, _mActXMLName);
            }
            else
            {
                if (_m_CachedOwnerController._m_ActPlayer.IsEndAnimation(_sb_AnimationClipName.ToString()))
                {
                    _m_CachedOwnerController._m_ActPlayer.PlayAnimationBaseOverride(_eDivision, _mi_ownerJobID, _mActXMLName);
                }

                return BTNodeState.Running;
            }

            return BTNodeState.Failure;
        }

        public void UpdateAnimationKey()
        {
            EntityDivision _eDivision = _m_CachedOwner._me_Division;
            _sb_AnimationClipName.Clear();

            string _mActXMLName = (_m_CachedOwnerController.GetNowFaceDirection() == true ? "Attack_L" : "Attack_R");

            switch (_eDivision)
            {
                case EntityDivision.Player:
                case EntityDivision.Rival:
                    _sb_AnimationClipName.Append($"Character{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_{_mActXMLName}");
                    break;
                case EntityDivision.Enemy:
                    _sb_AnimationClipName.Append($"Monster{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_{_mActXMLName}");
                    break;
                case EntityDivision.MealFactory:
                    _sb_AnimationClipName.Append($"MealFactory{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_{_mActXMLName}");
                    break;
                case EntityDivision.Neutrality:
                    break;
                case EntityDivision.Deco:
                    break;
                default:
                    break;
            }
        }
        public void CheckFaceDirection()
        {
            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);

            if (_chaseEntity == null) return;

            Vector3 _ownerPos = _m_CachedOwnerController.Pos3D;
            Vector3 _chasePos = _chaseEntity.Controller.Pos3D;

            _m_CachedOwnerController.GetMoveAgent(out var _ownerMoveAgent);
            _ownerMoveAgent.ProcFaceDirection(_ownerPos, _chasePos);
        }
        public void Reset()
        {

        }
    }

    public class NormalAtkStrategy : BTActionStrategy
    {
        private Entity _m_CachedOwner;
        private EntityContoller _m_CachedOwnerController;
        private EntityMoveAgent _m_CachedMoveAgent;
        public long _ml_ownerUID;
        public int _mi_ownerJobID;
        public StringBuilder _sb_AnimationClipName;

        public NormalAtkStrategy(long _ownerUID)
        {
            _sb_AnimationClipName = new StringBuilder();
            // 평타는 보통 타겟을 정해서 발사한다.
            this._ml_ownerUID = _ownerUID;

            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _m_CachedOwner);
            _m_CachedOwnerController = _m_CachedOwner.Controller;
            _mi_ownerJobID = _m_CachedOwner.JobID;

            _m_CachedOwnerController.GetMoveAgent(out _m_CachedMoveAgent);
        }
        public BTNodeState Run()
        {
            CheckFaceDirection();
            // 얼굴

            string _mActXMLName = (_m_CachedOwnerController.GetNowFaceDirection() == true ? "Attack_L" : "Attack_R").ToUpper();
            // 클립

            UpdateAnimationKey();
            EntityDivision _eDivision = _m_CachedOwner._me_Division;

            if (_m_CachedOwnerController._m_ActPlayer.IsPlayingEqualAnimation(_sb_AnimationClipName.ToString()) == false)
            {
                _m_CachedOwnerController._m_ActPlayer.PlayAnimationBaseOverride(_eDivision, _mi_ownerJobID, _mActXMLName);
                _m_CachedMoveAgent.SetProcMoveAgent(false);
                _m_CachedMoveAgent.StopBattleMoveImmediate();
            }
            else 
            {
                if(_m_CachedOwnerController._m_ActPlayer.IsEndAnimation(_sb_AnimationClipName.ToString()))
                {
                    _m_CachedOwnerController._m_ActPlayer.PlayAnimationBaseOverride(_eDivision, _mi_ownerJobID, _mActXMLName);
                    _m_CachedMoveAgent.SetProcMoveAgent(false);
                    _m_CachedMoveAgent.StopBattleMoveImmediate();
                }

                return BTNodeState.Running;
            }

            return BTNodeState.Failure;
        }

        public void UpdateAnimationKey()
        {
            EntityDivision _eDivision = _m_CachedOwner._me_Division;
            _sb_AnimationClipName.Clear();

            string _mActXMLName = (_m_CachedOwnerController.GetNowFaceDirection() == true ? "Attack_L" : "Attack_R");

            switch (_eDivision)
            {
                case EntityDivision.Player:
                case EntityDivision.Rival:
                    _sb_AnimationClipName.Append($"Character{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_{_mActXMLName}");
                    break;
                case EntityDivision.Enemy:
                    _sb_AnimationClipName.Append($"Monster{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_{_mActXMLName}");
                    break;
                case EntityDivision.MealFactory:
                    _sb_AnimationClipName.Append($"MealFactory{String.Format("{0:00}", _m_CachedOwner.CharacterID)}_{_mActXMLName}");
                    break;
                case EntityDivision.Neutrality:
                    break;
                case EntityDivision.Deco:
                    break;
                default:
                    break;
            }
        }
        public void CheckFaceDirection()
        {
            _m_CachedOwnerController.GetChaseEntity(out var _chaseEntity);

            if (_chaseEntity == null) return;

            Vector3 _ownerPos = _m_CachedOwnerController.Pos3D;
            Vector3 _chasePos = _chaseEntity.Controller.Pos3D;

            _m_CachedOwnerController.GetMoveAgent(out var _ownerMoveAgent);
            _ownerMoveAgent.ProcFaceDirection(_ownerPos , _chasePos);
        }
        public void Reset()
        {

        }
    }
    public class MonsterProbe : BTActionStrategy
    {
        private long _ml_uniqueID;
        private Entity _m_CachedOwner;
        private EntityContoller _m_CachedOwnerController;
        private EntityInfo _m_EntityInfo;

        private List<Vector3> _mLt_CacheProbe;

        private float _mf_Speed;
        private int _mi_CurrentIndex;
        private StringBuilder _sb;
        public MonsterProbe(long _ownerUID)
        {
            _sb = new StringBuilder();
            _ml_uniqueID = _ownerUID;
            _mi_CurrentIndex = 0;

            EntityManager.GetInstance().GetEntity(_ownerUID, out _m_CachedOwner);
            _m_CachedOwnerController = _m_CachedOwner.Controller;
            _m_EntityInfo = _m_CachedOwner.Info;

            if(_m_CachedOwnerController is EntityMonsterController _monsterController)
                _mLt_CacheProbe = _monsterController._mLt_ProbePosition;

            _mf_Speed = _m_EntityInfo.MoveSpeed;
        }
        public BTNodeState Run()
        {
            UpdatePosition();
            return BTNodeState.Success;
        }

        public void UpdatePosition()
        {
            if (_mi_CurrentIndex >= _mLt_CacheProbe.Count)
                _mi_CurrentIndex = 0;

            Vector3 currentPos = _m_CachedOwnerController.Pos3D;
            Vector3 destinationPos = _mLt_CacheProbe[_mi_CurrentIndex];

            Vector3 dir = (destinationPos - currentPos).normalized;

            Vector3 P0 = _m_CachedOwnerController.Pos3D;
            Vector3 AT = dir * _mf_Speed * Time.deltaTime;
            Vector3 P1 = P0 + AT;
            _m_CachedOwnerController.Pos3D = P1;
            //_owner.LookAt(destinationPos);

            if (MathUtility.CheckOverV2SqrMagnitudeDistance(currentPos, destinationPos, 0.1f) == false)
                ++_mi_CurrentIndex; 
            // 거리만큼 가까워 졌다면.

            UpdateAnimation();
        }

        public void UpdateAnimation()
        {
            if (_m_CachedOwnerController is EntityMonsterController _monsterContoller)
            {
                string _mClipName = "MOVE";

                Entity _retEntity = null;
                EntityManager.GetInstance().GetEntity(_ml_uniqueID, out _retEntity);
                EntityDivision _eDivision = _retEntity._me_Division;

                if (_monsterContoller._m_ActPlayer.IsPlayingEqualAnimation(_mClipName) == false)
                {
                    _monsterContoller._m_ActPlayer.PlayAnimationBaseOverride(_eDivision, _retEntity.JobID, _mClipName);
                }
            }
        }

        public void Reset()
        {
            _mi_CurrentIndex = 0;
        }
    }
    public class EntityBehaviorTreeActionNode : EntityBehaviorTreeNodeBase
    {
        BTActionStrategy _strategy;

        public EntityBehaviorTreeActionNode(BTActionStrategy _strategy)
        {
            this._strategy = _strategy;
        }

        public override BTNodeState Evaluate()
        {
            return _strategy.Run();
        }
    }
}


namespace EntityBehaviorTree
{
    // Rival PlayerAI 전용
    public class RivalRunDiaDraw : BTActionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;
        public RivalRunDiaDraw()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);
        }

        public BTNodeState Run()
        {
            int _drawTarget = m_CacheRivalPlayerAI.GetDrawTarget();

            switch (_drawTarget)
            {
                case 0: DrawUnCommon(); break;
                case 1: DrawHero(); break;
                case 2: DrawMyth(); break;
            }

            UnityLogger.GetInstance().Log($"남은 라이벌 다이아 {RivalPlayerAIManager.GetInstance()._mi_Dia}");

            return BTNodeState.Failure;
        }

        public void Reset()
        {

        }
        public void DrawUnCommon()
        {
            int _rate = 60;
            EntityGrade _me_Grade = EntityGrade.UnCommon;

            int _drawVal = UnityEngine.Random.Range(0, 100);
            int _price = Defines.DrawDiaPriceUncommon;

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

            RivalPlayerAIManager.GetInstance().UseDia(_price);
        }
        public void DrawHero()
        {
            int _rate = 20;
            EntityGrade _me_Grade = EntityGrade.Hero;

            int _drawVal = UnityEngine.Random.Range(0, 100);

            int _price = Defines.DrawDiaPriceHero;

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

            RivalPlayerAIManager.GetInstance().UseDia(_price);


        }
        public void DrawMyth()
        {
            int _rate = 20;
            EntityGrade _me_Grade = EntityGrade.Myth;

            int _drawVal = UnityEngine.Random.Range(0, 100);
            int _price = Defines.DrawDiaPriceMyth;

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

            RivalPlayerAIManager.GetInstance().UseDia(_price);
        }
        public void FindEnableEntityGroups(int _jobID, out EntitiesGroup _ret)
        {
            _ret = null;
            EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival, _jobID, out _ret);
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
            RivalMapManager.GetInstance().GetNavigationElements(out var _dictElements);
            var _Lt_Elements = new List<NavigationElement>(_dictElements.Values.ToList());

            var _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival);

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

            RivalEntitiesGroupFactory _spawner = new RivalEntitiesGroupFactory();
            _ = _spawner.CreateEntity(spawnID, _v3_position, (entitiesGroup) =>
            {
                RivalEntityFactory _entitySpanwer = new RivalEntityFactory();

                _ = _entitySpanwer.CreateEntity(_jobID, _v3_position, (_createEntity) =>
                {
                    entitiesGroup.AddEntity(ref _createEntity);
                    RivalPlayerAIManager.GetInstance().AddSupply(1);
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

            RivalEntityFactory _entitySpanwer = new RivalEntityFactory();
            _ = _entitySpanwer.CreateEntity(spawnID, _v3_position, (_createEntity) =>
            {
                _entitiesGroup.AddEntity(ref _createEntity);
                RivalPlayerAIManager.GetInstance().AddSupply(1);
                _createEntity.Controller._ml_EntityGroupUID = _entitiesGroup.UniqueID;

                _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            });
        }
    }
    public class RivalRunGoldDraw : BTActionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;
        public RivalRunGoldDraw()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);
        }

        public BTNodeState Run()
        {
            RunSpawn();
            return BTNodeState.Failure;
        }

        public void Reset()
        {

        }

        public void RunSpawn()
        {
            int _jobID = DrawCharacterID();
            FindEnableEntityGroups(_jobID, out var _entitiesGroup);

            if (_entitiesGroup == null)
            {
                DrawAnyMapNavigation(out var _Navigation);
                Spawn(_jobID, _Navigation);
            }
            else
            {
                Spawn(_jobID, _entitiesGroup);
            }

            RivalPlayerAIManager.GetInstance().UseGold(Defines.DrawDefaultGoldPrice); // 스폰했으니 차감
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
            EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival, _jobID, out _ret);
        }

        public void DrawAnyMapNavigation(out NavigationElement _retNavigation)
        {
            RivalMapManager.GetInstance().GetNavigationElements(out var _dictElements);
            var _Lt_Elements = new List<NavigationElement>(_dictElements.Values.ToList());

            var _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival);

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

            RivalEntitiesGroupFactory _spawner = new RivalEntitiesGroupFactory();
            _ = _spawner.CreateEntity(spawnID, _v3_position, (entitiesGroup) =>
            {
                RivalEntityFactory _entitySpanwer = new RivalEntityFactory();

                _ = _entitySpanwer.CreateEntity(_jobID, _v3_position, (_createEntity) =>
                {
                    entitiesGroup.AddEntity(ref _createEntity);
                    RivalPlayerAIManager.GetInstance().AddSupply(1);
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

            RivalEntityFactory _entitySpanwer = new RivalEntityFactory();
            _ = _entitySpanwer.CreateEntity(spawnID, _v3_position, (_createEntity) =>
            {
                _entitiesGroup.AddEntity(ref _createEntity);
                RivalPlayerAIManager.GetInstance().AddSupply(1);
                _createEntity.Controller._ml_EntityGroupUID = _entitiesGroup.UniqueID;

                _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            });
        }

    }
    public class RivalRunCombine : BTActionStrategy
    {
        RivalPlayerAI m_CacheRivalPlayerAI;
        public RivalRunCombine()
        {
            RivalPlayerAIManager.GetInstance().GetRavalPlayer(out m_CacheRivalPlayerAI);
        }

        public BTNodeState Run()
        {
            RunCombine();

            return BTNodeState.Failure;
        }

        public void Reset()
        {

        }

        public void RunCombine()
        {
            bool _isRecipeCombine = m_CacheRivalPlayerAI.GetRecipeCombine();

            if (_isRecipeCombine)
            {
                Execute_RecipeCombine();
            }
            else
            {
                // 여기가 단순 MergeCombine
                Execute_MergeCombine();
                
            }




        }
        public void Execute_MergeCombine()
        {
            DeleteMergeCombineMercenary();  // 용병 삭제
            SpawnMergeMercenary();
        }

        public void Execute_RecipeCombine()
        {
            DeleteRecipeCombineMercenary();
            SpawnRecipeMercenary();
        }
        public void DeleteMergeCombineMercenary()
        {
            int _jobID = m_CacheRivalPlayerAI.GetCombineID();
            long _uid = m_CacheRivalPlayerAI.GetCombineUID();

            EntityManager.GetInstance().NewRemoveGroup(EntityDivision.Rival, _jobID, _uid);
        }
        public void DeleteRecipeCombineMercenary()
        {
            List<EntitiesGroup> _Lt_Entities = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival);

            List<EntitiesGroup> _Lt_UseCandidate_CountSindle = new List<EntitiesGroup>();
            List<EntitiesGroup> _Lt_UseCandidate_CountMulti = new List<EntitiesGroup>();

            int _recipeID = m_CacheRivalPlayerAI.GetCombineID();
            GameDataManager.GetInstance().GetRecipeData(_recipeID, out GameDB_MealRecipe _recipe);

            int[] _mArr_Recipe = _recipe.Arr_Recipe;
            int _passCount = 0;

            for (int i = 0; i < _mArr_Recipe.Length; ++i)
            {
                int _characterID = _mArr_Recipe[i];

                for (int j = 0; j < _Lt_Entities.Count; ++j)
                {
                    var _groups = _Lt_Entities[j];

                    if (_groups.ID == _characterID)
                    {
                        if (_groups.Count == 1)
                        {
                            _Lt_UseCandidate_CountSindle.Add(_groups);
                            // 용병 삭제
                        }
                        else
                        {
                            _Lt_UseCandidate_CountMulti.Add(_groups);
                        }

                        ++_passCount;
                        break;
                    }
                }
            }
            // 여긴 용병 후보 대기 구문

            if (_passCount != _mArr_Recipe.Length) return; // 통과 카운터와 재료 갯수가 맞지 않으면 수행하지 않음

            for (int i = 0; i < _Lt_UseCandidate_CountSindle.Count; ++i)
            {
                int _jobID = _Lt_UseCandidate_CountSindle[i].ID;
                long _uid = _Lt_UseCandidate_CountSindle[i].UniqueID;

                EntityManager.GetInstance().NewRemoveGroup(EntityDivision.Rival, _jobID, _uid);
            }
            // 하나 있는 곳 삭제

            for (int i = 0; i < _Lt_UseCandidate_CountMulti.Count; ++i)
            {
                _Lt_UseCandidate_CountMulti[i].RemoveLastEntity();
            }
            // 둘 이상 있는 곳 삭제
        }
        public void SpawnRecipeMercenary()
        {
            int _recipeID = m_CacheRivalPlayerAI.GetCombineID();
            GameDataManager.GetInstance().GetRecipeData(_recipeID, out GameDB_MealRecipe _recipe);

            int _createJobID = _recipe._mi_MealKitID;

            FindEnableEntityGroups(_createJobID, out var _entitiesGroup);

            if (_entitiesGroup == null)
            {
                DrawAnyMapNavigation(out var _Navigation);
                Spawn(_createJobID, _Navigation);
            }
            else
            {
                Spawn(_createJobID, _entitiesGroup);
            }

            UnityLogger.GetInstance().Log($"Execute Recipe Combine!! recipe ID :: {_recipeID} , CreateID :: {_createJobID}");
        }

        public void SpawnMergeMercenary()
        {
            int _jobID = m_CacheRivalPlayerAI.GetCombineID();
            GameDataManager.GetInstance().GetGameDBCharacterInfo(_jobID, out var _ret);

            int _mi_Grade = (int)_ret._me_Grade;
            EntityGrade _me_NextGrade = (EntityGrade)(_mi_Grade + 1);

            int _drawJobID = DrawCharacterID(_me_NextGrade);
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
        public void FindEnableEntityGroups(int _jobID, out EntitiesGroup _ret)
        {
            _ret = null;
            EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival, _jobID, out _ret);
        }
        public void DrawAnyMapNavigation(out NavigationElement _retNavigation)
        {
            RivalMapManager.GetInstance().GetNavigationElements(out var _dictElements);
            var _Lt_Elements = new List<NavigationElement>(_dictElements.Values.ToList());

            var _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Rival);

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

            RivalEntitiesGroupFactory _spawner = new RivalEntitiesGroupFactory();
            _ = _spawner.CreateEntity(spawnID, _v3_position, (entitiesGroup) =>
            {
                RivalEntityFactory _entitySpanwer = new RivalEntityFactory();

                _ = _entitySpanwer.CreateEntity(_jobID, _v3_position, (_createEntity) =>
                {
                    entitiesGroup.AddEntity(ref _createEntity);
                    RivalPlayerAIManager.GetInstance().AddSupply(1);
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

            RivalEntityFactory _entitySpanwer = new RivalEntityFactory();
            _ = _entitySpanwer.CreateEntity(spawnID, _v3_position, (_createEntity) =>
            {
                _entitiesGroup.AddEntity(ref _createEntity);
                RivalPlayerAIManager.GetInstance().AddSupply(1);
                _createEntity.Controller._ml_EntityGroupUID = _entitiesGroup.UniqueID;

                _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
            });
        }
    }
}

