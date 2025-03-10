using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using GlobalGameDataSpace;

public interface IEntityAbsFactoryBase
{
    public Entity CreateEntity(int _entityTID, Vector3 _position);
}

public abstract class EntityFactoryBase : IEntityAbsFactoryBase
{
    public abstract Entity CreateEntity(int _entityTID, Vector3 _position);
}

public class UserEntitiesGroupFactory : EntityFactoryBase
{
    [Obsolete("Unitask 기반으로 생성하는 것으로 변경")]
    public override Entity CreateEntity(int _entityTID, Vector3 _position) { return null; }
    public async UniTask CreateEntity(int _entityTID, Vector3 _position, Action<EntitiesGroup> _onCB_Create)
    {
        bool _IsCreate = false;
        bool _IsLoaded = false;

        // 관련 데이터 먼저 모두 로딩
        //ResourceManager.GetInstance().GetResource(ResourceType.PlayerAnimationController, _entityTID, true, (obj) =>
        //{
        //    _IsLoaded = true;
        //});

        //await UniTask.WaitUntil(() => _IsLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.EntityGroup, _entityTID, true, (_modelResource) => {

            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            EntitiesGroup _entitiesGroup = _modelObj.GetComponent<EntitiesGroup>();

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            while (true)
            {
                if (EntityManager.GetInstance().CheckContainEntityKey(uUID) == false)
                    break;

                uUID = uUIDGenerator.Generate();
            }

            _entitiesGroup.UniqueID = uUID;
            // UID 세팅

            EntityManager.GetInstance().NewAddEntityGroup(EntityDivision.Player, _entityTID, ref _entitiesGroup);
            // 선 컴포넌트 세팅 및 UID 발급

            var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _entitiesGroup.NvPos = _navigationElement._mv2_Index;
            // 무빙 에이전트 맵 그리드 세팅

            _entitiesGroup.Pos3D = _position;
            _entitiesGroup.ID = _entityTID;
            // 위치 세팅

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            SceneLoadManager.GetInstance().GetStage(out var _stage);

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅

            _IsCreate = true;

            _onCB_Create?.Invoke(_entitiesGroup);
        });

        await UniTask.WaitUntil(() => _IsCreate == true);
    }
}

public class RivalEntitiesGroupFactory : EntityFactoryBase
{
    [Obsolete("Unitask 기반으로 생성하는 것으로 변경")]
    public override Entity CreateEntity(int _entityTID, Vector3 _position) { return null; }
    public async UniTask CreateEntity(int _entityTID, Vector3 _position, Action<EntitiesGroup> _onCB_Create)
    {
        bool _IsCreate = false;
        bool _IsLoaded = false;

        // 관련 데이터 먼저 모두 로딩
        //ResourceManager.GetInstance().GetResource(ResourceType.PlayerAnimationController, _entityTID, true, (obj) =>
        //{
        //    _IsLoaded = true;
        //});

        //await UniTask.WaitUntil(() => _IsLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.EntityGroup, _entityTID, true, (_modelResource) => {

            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            EntitiesGroup _entitiesGroup = _modelObj.GetComponent<EntitiesGroup>();

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            while (true)
            {
                if(EntityManager.GetInstance().CheckContainEntityKey(uUID) == false)
                    break;

                uUID = uUIDGenerator.Generate();
            }

            _entitiesGroup.UniqueID = uUID;
            // UID 세팅

            EntityManager.GetInstance().NewAddEntityGroup(EntityDivision.Rival, _entityTID, ref _entitiesGroup);
            // 선 컴포넌트 세팅 및 UID 발급

            var _navigationElement = RivalMapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _entitiesGroup.NvPos = _navigationElement._mv2_Index;
            // 무빙 에이전트 맵 그리드 세팅

            _entitiesGroup.Pos3D = _position;
            _entitiesGroup.ID = _entityTID;
            // 위치 세팅

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            SceneLoadManager.GetInstance().GetStage(out var _stage);

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅

            _IsCreate = true;

            _onCB_Create?.Invoke(_entitiesGroup);
        });

        await UniTask.WaitUntil(() => _IsCreate == true);
    }
}

public class UserEntityFactory : EntityFactoryBase
{
    public override Entity CreateEntity(int _entityTID , Vector3 _position)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.Entity, _entityTID, true,  ( _modelResource) => {
            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data 세팅

            var _userContoller = _obj.AddComponent<EntityUserContoller>();

            var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            EntityManager.GetInstance().AddEntity(EntityDivision.Player, uUID, ref _cEntity);
            // 선 컴포넌트 세팅 및 UID 발급

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Player, _userContoller);
            // Entity 와 Controller 연결

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // 컨트롤러 셋업

            _userContoller.SetMoveAgent(_moveAgent);
            // 무빙 에이전트 셋업

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder 셋업

            _moveAgent.OnInitialize(uUID, _userContoller);
            // 필수 Entity Script 설정

            var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            _pathFinder.SetNavigationElement(_navigationElement);
            // 무빙 에이전트 맵 그리드 세팅

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            _userContoller.Pos3D = _position;
            // 포지션 세팅

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅
        });
        return null;
    }

    public async UniTask CreateEntity(int _entityTID, Vector3 _position, Action<Entity> _onCB_Create)
    {
        bool _IsCreate = false;
        bool _IsLoaded = false;

        // 관련 데이터 먼저 모두 로딩

        ResourceManager.GetInstance().GetResource(ResourceType.PlayerAnimationController, _entityTID, true, (obj) => 
        {
            _IsLoaded = true;
        });

        await UniTask.WaitUntil(() => _IsLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.Entity, _entityTID, true, (_modelResource) => {

            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data 세팅

            var _userContoller = _obj.AddComponent<EntityUserContoller>();

            var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            while (true)
            {
                if (EntityManager.GetInstance().CheckContainEntityKey(uUID) == false)
                    break;

                uUID = uUIDGenerator.Generate();
            }

            EntityManager.GetInstance().AddEntity(EntityDivision.Player, uUID, ref _cEntity);
            // 선 컴포넌트 세팅 및 UID 발급

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Player, _userContoller);
            // Entity 와 Controller 연결

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // 컨트롤러 셋업

            _userContoller.SetMoveAgent(_moveAgent);
            // 무빙 에이전트 셋업

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder 셋업

            _moveAgent.OnInitialize(uUID, _userContoller);
            // 필수 Entity Script 설정

            var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            // 무빙 에이전트 맵 그리드 세팅

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            _userContoller.Pos3D = _position;
            // 포지션 세팅

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅

            _IsCreate = true;

            _onCB_Create?.Invoke(_cEntity);
        });

        await UniTask.WaitUntil(() => _IsCreate == true);
    }
}

public class RivalEntityFactory : EntityFactoryBase
{
    public override Entity CreateEntity(int _entityTID, Vector3 _position)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.Entity, _entityTID, true, (_modelResource) => {
            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data 세팅

            var _userContoller = _obj.AddComponent<EntityUserContoller>();

            var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            EntityManager.GetInstance().AddEntity(EntityDivision.Rival, uUID, ref _cEntity);
            // 선 컴포넌트 세팅 및 UID 발급

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Rival, _userContoller);
            // Entity 와 Controller 연결

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // 컨트롤러 셋업

            _userContoller.SetMoveAgent(_moveAgent);
            // 무빙 에이전트 셋업

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder 셋업

            _moveAgent.OnInitialize(uUID, _userContoller);
            // 필수 Entity Script 설정

            var _navigationElement = RivalMapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            _pathFinder.SetNavigationElement(_navigationElement);
            // 무빙 에이전트 맵 그리드 세팅

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            _userContoller.Pos3D = _position;
            // 포지션 세팅


            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅
        });
        return null;
    }

    public async UniTask CreateEntity(int _entityTID, Vector3 _position, Action<Entity> _onCB_Create)
    {
        bool _IsCreate = false;
        bool _IsLoaded = false;

        // 관련 데이터 먼저 모두 로딩

        ResourceManager.GetInstance().GetResource(ResourceType.PlayerAnimationController, _entityTID, true, (obj) =>
        {
            _IsLoaded = true;
        });

        await UniTask.WaitUntil(() => _IsLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.Entity, _entityTID, true, (_modelResource) => {

            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data 세팅

            var _userContoller = _obj.AddComponent<EntityUserContoller>();

            var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            while (true)
            {
                if (EntityManager.GetInstance().CheckContainEntityKey(uUID) == false)
                    break;

                uUID = uUIDGenerator.Generate();
            }

            EntityManager.GetInstance().AddEntity(EntityDivision.Rival, uUID, ref _cEntity);
            // 선 컴포넌트 세팅 및 UID 발급

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Rival, _userContoller);
            // Entity 와 Controller 연결

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // 컨트롤러 셋업

            _userContoller.SetMoveAgent(_moveAgent);
            // 무빙 에이전트 셋업

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder 셋업

            _moveAgent.OnInitialize(uUID, _userContoller);
            // 필수 Entity Script 설정

            var _navigationElement = RivalMapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            // 무빙 에이전트 맵 그리드 세팅

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            _userContoller.Pos3D = _position;
            // 포지션 세팅

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅

            _IsCreate = true;

            _onCB_Create?.Invoke(_cEntity);
        });

        await UniTask.WaitUntil(() => _IsCreate == true);
    }
}

public class EnemyEntityFactory : EntityFactoryBase
{
    public override Entity CreateEntity(int _entityTID, Vector3 _position)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.NPCEntity, _entityTID, true, (_modelResource) => {
            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data 세팅

            var _mobContoller = _obj.AddComponent<EntityMonsterController>();

            //var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            //var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            EntityManager.GetInstance().AddEntity(EntityDivision.Enemy, uUID, ref _cEntity);
            // 선 컴포넌트 세팅 및 UID 발급

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Enemy, _mobContoller);
            // Entity 와 Controller 연결

            _mobContoller.SetActPlayer(_actPlayer);
            _mobContoller.SetUp(uUID, _entityTID);
            // 컨트롤러 셋업

            PoolingManager.GetInstance().GetPooledObject(PooledObject.WO, PooledObjectInner.WO_WorldHealBarTag, out var _pooledObject);
            PooledObjectWorldHealBarTag _barTag = _pooledObject as PooledObjectWorldHealBarTag;
            _barTag.SetOwnerEntity(_cEntity);
            _barTag.SetHP();
            // World HealBar 세팅

            _mobContoller._m_worldHaelBarTag = _barTag;
            // World HealBar 세팅

            //_mobContoller.SetMoveAgent(_moveAgent);
            // 무빙 에이전트 셋업

            //_moveAgent.SetPathFinder(_pathFinder);
            // Path Finder 셋업

            //_moveAgent.OnInitialize(uUID, _mobContoller);
            // 필수 Entity Script 설정

            //var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            //_moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            //_pathFinder.SetNavigationElement(_navigationElement);
            // 무빙 에이전트 맵 그리드 세팅

            _mobContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            _mobContoller.Pos3D = _position;
            // 포지션 세팅

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅
        });
        return null;
    }

    public async UniTask CreateEntity(int _entityTID, Vector3 _position, bool _isMySpawner, Action<Entity> _onCB_Create)
    {
        bool _IsCreate = false;
        bool _IsLoaded = false;

        // 관련 데이터 먼저 모두 로딩

        ResourceManager.GetInstance().GetResource(ResourceType.EnemyAnimationController, _entityTID, true, (obj) =>
        {
            _IsLoaded = true;
        });

        await UniTask.WaitUntil(() => _IsLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.NPCEntity, _entityTID, true, (_modelResource) => {

            // 여기서 나오는 obj는 리소스 매니저에서 오는 단순 월드모델 오브젝트
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID 발급

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data 세팅

            var _userContoller = _obj.AddComponent<EntityMonsterController>();

            //var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            //var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            while(true)
            {
                if (EntityManager.GetInstance().CheckContainEntityKey(uUID) == false)
                    break;

                uUID = uUIDGenerator.Generate();
            }

            EntityManager.GetInstance().AddEntity(EntityDivision.Enemy, uUID, ref _cEntity);
            // 선 컴포넌트 세팅 및 UID 발급

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Enemy, _userContoller);
            // Entity 와 Controller 연결

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // 컨트롤러 셋업

            PoolingManager.GetInstance().GetPooledObject(PooledObject.WO, PooledObjectInner.WO_WorldHealBarTag, out var _pooledObject);
            PooledObjectWorldHealBarTag _barTag = _pooledObject as PooledObjectWorldHealBarTag;
            _barTag.SetOwnerEntity(_cEntity);
            _barTag.SetHP();
            // World HealBar 세팅

            _userContoller._m_worldHaelBarTag = _barTag;
            // World HealBar 세팅

            if (_isMySpawner == true)
                _userContoller.SetPlayerZoneProbeList();
            else
                _userContoller.SetRivalZoneProbeList();

            //_userContoller.SetMoveAgent(_moveAgent);
            // 무빙 에이전트 셋업

                //_moveAgent.SetPathFinder(_pathFinder);
                // Path Finder 셋업

                //_moveAgent.OnInitialize(uUID, _userContoller);
                // 필수 Entity Script 설정

                //var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
                //_moveAgent.SetStartPoint(_navigationElement._mv2_Index);
                //_pathFinder.SetNavigationElement(_navigationElement);
                // 무빙 에이전트 맵 그리드 세팅

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // 부모로 세팅

            _userContoller.Pos3D = _position;
            // 포지션 세팅

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // 이름 세팅

            _IsCreate = true;

            _onCB_Create?.Invoke(_cEntity);
        });

        await UniTask.WaitUntil(() => _IsCreate == true);
    }
}

