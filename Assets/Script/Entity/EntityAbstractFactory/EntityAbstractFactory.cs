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

public class UserEntityFactory : EntityFactoryBase
{
    public override Entity CreateEntity(int _entityTID , Vector3 _position)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.Entity, _entityTID, true,  ( _modelResource) => {
            // ���⼭ ������ obj�� ���ҽ� �Ŵ������� ���� �ܼ� ����� ������Ʈ
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID �߱�

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data ����

            var _userContoller = _obj.AddComponent<EntityUserContoller>();

            var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            EntityManager.GetInstance().AddEntity(EntityDivision.Player, uUID, ref _cEntity);
            // �� ������Ʈ ���� �� UID �߱�

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Player, _userContoller);
            // Entity �� Controller ����

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // ��Ʈ�ѷ� �¾�

            _userContoller.SetMoveAgent(_moveAgent);
            // ���� ������Ʈ �¾�

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder �¾�

            _moveAgent.OnInitialize(uUID, _userContoller);
            // �ʼ� Entity Script ����

            var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            _pathFinder.SetNavigationElement(_navigationElement);
            // ���� ������Ʈ �� �׸��� ����

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // �θ�� ����

            _userContoller.Pos3D = _position;
            // ������ ����


            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // �̸� ����
        });
        return null;
    }

    public async UniTask CreateEntity(int _entityTID, Vector3 _position, Action<Entity> _onCB_Create)
    {
        bool _IsCreate = false;
        bool _IsLoaded = false;

        // ���� ������ ���� ��� �ε�

        ResourceManager.GetInstance().GetResource(ResourceType.PlayerAnimationController, _entityTID, true, (obj) => 
        {
            _IsLoaded = true;
        });

        await UniTask.WaitUntil(() => _IsLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.Entity, _entityTID, true, (_modelResource) => {

            // ���⼭ ������ obj�� ���ҽ� �Ŵ������� ���� �ܼ� ����� ������Ʈ
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID �߱�

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data ����

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
            // �� ������Ʈ ���� �� UID �߱�

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Player, _userContoller);
            // Entity �� Controller ����

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // ��Ʈ�ѷ� �¾�

            _userContoller.SetMoveAgent(_moveAgent);
            // ���� ������Ʈ �¾�

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder �¾�

            _moveAgent.OnInitialize(uUID, _userContoller);
            // �ʼ� Entity Script ����

            var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            // ���� ������Ʈ �� �׸��� ����

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // �θ�� ����

            _userContoller.Pos3D = _position;
            // ������ ����

            SceneLoadManager.GetInstance().GetStage(out var _stage);

            if(_stage is BattleStage _battleStage)
            {
                _battleStage.GetStageMachine(out var _machine);
                _machine.GetCurrentState(out var _IStage);

                if(_IStage is BattleState)
                {
                    _userContoller.TurnOnAI();
                }
                else
                {
                    _userContoller.TurnOffAI();
                }
            }
            // �������� ���� Ȯ���ϰ� AI ų�� ������Ȯ��



            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // �̸� ����

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
        ResourceManager.GetInstance().GetResource(ResourceType.NPCEntity, _entityTID, true, (_modelResource) => {
            // ���⼭ ������ obj�� ���ҽ� �Ŵ������� ���� �ܼ� ����� ������Ʈ
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID �߱�

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data ����

            var _mobContoller = _obj.AddComponent<EntityMonsterController>();

            var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            EntityManager.GetInstance().AddEntity(EntityDivision.Enemy, uUID, ref _cEntity);
            // �� ������Ʈ ���� �� UID �߱�

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Enemy, _mobContoller);
            // Entity �� Controller ����

            _mobContoller.SetActPlayer(_actPlayer);
            _mobContoller.SetUp(uUID, _entityTID);
            // ��Ʈ�ѷ� �¾�

            _mobContoller.SetMoveAgent(_moveAgent);
            // ���� ������Ʈ �¾�

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder �¾�

            _moveAgent.OnInitialize(uUID, _mobContoller);
            // �ʼ� Entity Script ����

            var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            _pathFinder.SetNavigationElement(_navigationElement);
            // ���� ������Ʈ �� �׸��� ����

            _mobContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // �θ�� ����

            _mobContoller.Pos3D = _position;
            // ������ ����

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // �̸� ����
        });
        return null;
    }

    public async UniTask CreateEntity(int _entityTID, Vector3 _position , Action<Entity> _onCB_Create)
    {
        bool _IsCreate = false;
        bool _IsLoaded = false;

        // ���� ������ ���� ��� �ε�

        ResourceManager.GetInstance().GetResource(ResourceType.EnemyAnimationController, _entityTID, true, (obj) =>
        {
            _IsLoaded = true;
        });

        await UniTask.WaitUntil(() => _IsLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.NPCEntity, _entityTID, true, (_modelResource) => {

            // ���⼭ ������ obj�� ���ҽ� �Ŵ������� ���� �ܼ� ����� ������Ʈ
            var _modelObj = GameObject.Instantiate(_modelResource);
            var _actPlayer = _modelObj.GetComponent<ActionPlayer>();
            // Model Instantiate

            UUIDGenerator<long> uUIDGenerator = UUIDGenerator<long>.GetInstance();
            var uUID = uUIDGenerator.Generate();
            // UID �߱�

            GameObject _obj = new GameObject();

            var _cEntity = _obj.AddComponent<Entity>();
            // Entity Data ����

            var _userContoller = _obj.AddComponent<EntityMonsterController>();

            var _moveAgent = _obj.AddComponent<EntityMoveAgent>();
            var _pathFinder = _obj.AddComponent<EntityPathFinder>();

            while(true)
            {
                if (EntityManager.GetInstance().CheckContainEntityKey(uUID) == false)
                    break;

                uUID = uUIDGenerator.Generate();
            }

            EntityManager.GetInstance().AddEntity(EntityDivision.Enemy, uUID, ref _cEntity);
            // �� ������Ʈ ���� �� UID �߱�

            _cEntity.InitEntityData(uUID, _entityTID, EntityDivision.Enemy, _userContoller);
            // Entity �� Controller ����

            _userContoller.SetActPlayer(_actPlayer);
            _userContoller.SetUp(uUID, _entityTID);
            // ��Ʈ�ѷ� �¾�

            _userContoller.SetMoveAgent(_moveAgent);
            // ���� ������Ʈ �¾�

            _moveAgent.SetPathFinder(_pathFinder);
            // Path Finder �¾�

            _moveAgent.OnInitialize(uUID, _userContoller);
            // �ʼ� Entity Script ����

            var _navigationElement = MapManager.GetInstance().GetMyNavigationByPos3D(_position);
            _moveAgent.SetStartPoint(_navigationElement._mv2_Index);
            _pathFinder.SetNavigationElement(_navigationElement);
            // ���� ������Ʈ �� �׸��� ����

            _userContoller.AISetUp();
            // AI SetUp

            _modelObj.transform.SetParent(_obj.transform);
            // �θ�� ����

            _userContoller.Pos3D = _position;
            // ������ ����

            _obj.name = $"{_modelObj.name.Replace("(Clone)", "")}_{uUID}";
            // �̸� ����

            _IsCreate = true;

            _onCB_Create?.Invoke(_cEntity);
        });

        await UniTask.WaitUntil(() => _IsCreate == true);
    }
}

