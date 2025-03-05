using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GlobalGameDataSpace;
using System.Reflection;
using System.Text;
using UnityEditor.Hardware;

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
        private EntityContoller _m_CachedOwnerController;

        public EnemyFindStategy(long _ownerUID)
        {
            this._ml_ownerUID = _ownerUID;
            Entity _entity;
            EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _entity);
            _m_CachedOwnerController = _entity.Controller;
            _me_CachedOwnerDivision = _entity._me_Division;
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

        public BTNodeState Run()
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
        private Transform _owner;
        private List<Vector3> _probeList;
        private int _currentIndex;
        private float _speed;
        private StringBuilder _sb;
        public MonsterProbe(Transform _owner)
        {
            _sb = new StringBuilder();
            _currentIndex = 0;
            _speed = 5f;

            this._owner = _owner;

            if (_probeList == null)
                _probeList = new List<Vector3>();

            _probeList.Clear();

            for (int i = 0; i < 4; ++i)
            {
                int rangeX = UnityEngine.Random.Range(1, 15);
                int rangeZ = UnityEngine.Random.Range(1, 15);

                Vector3 pos = new Vector3(_owner.transform.position.x - rangeX, _owner.transform.position.y, _owner.transform.position.z - rangeZ);
                _probeList.Add(pos);
            }
        }
        public BTNodeState Run()
        {
            UpdatePosition();
            return BTNodeState.Success;
        }

        public void UpdatePosition()
        {
            if (_currentIndex >= _probeList.Count)
                _currentIndex = 0;

            Vector3 currentPos = _owner.position;
            Vector3 destinationPos = _probeList[_currentIndex];

            Vector3 dir = (destinationPos - currentPos).normalized;

            Vector3 P0 = _owner.position;
            Vector3 AT = dir * _speed * Time.deltaTime;
            Vector3 P1 = P0 + AT;
            _owner.position = P1;
            _owner.LookAt(destinationPos);

            if (Vector3.Distance(currentPos, destinationPos) <= 0.1f)
                ++_currentIndex;

            UpdateAnimation();
        }

        public void UpdateAnimation()
        {
            EntityContoller _controllerBase = _owner.GetComponent<EntityContoller>();

            if (_controllerBase is EntityMonsterController _monsterContoller)
            {
                string _mClipName = "MOVE";

                Entity _retEntity = null;
                EntityManager.GetInstance().GetEntity(_monsterContoller._ml_EntityUID, out _retEntity);
                EntityDivision _eDivision = _retEntity._me_Division;

                if (_monsterContoller._m_ActPlayer.IsPlayingEqualAnimation(_mClipName) == false)
                {
                    _monsterContoller._m_ActPlayer.PlayAnimationBaseOverride(_eDivision, _retEntity.JobID, _mClipName);
                }
            }
        }

        public void Reset()
        {
            _currentIndex = 0;
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

