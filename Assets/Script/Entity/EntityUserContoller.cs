using System.Collections.Generic;
using UnityEngine;
using EntityBehaviorTree;
using GlobalGameDataSpace;

public class EntityUserContoller : EntityContoller 
{
    private float _mf_MoveSpeed;
    public float MoveSpeed{
        get { 
            if(_mf_MoveSpeed <= 0f){
                Entity _entity;
                EntityManager.GetInstance().GetEntity(EntityDivision.Player, _ml_EntityUID, out _entity);
                _mf_MoveSpeed = _entity.Info.MoveSpeed;
            }
            return _mf_MoveSpeed; }
    }

    // ���� ���� ��� ����?
    private Avatar _avatar; // ����ȭ�� �� ���̽�
    private EntityBehaviorTreeBase _behaviorTree;

    public override void SetUp(long _entityUID, int _entityID)
    {
        _ml_EntityUID = _entityUID;
        _mi_EntityTID = _entityID;
        TurnOnAI(); // �ʱ�� ����.
        TransformSetUp();

        _m_ActPlayer.SetOwnerUID(_entityUID, _entityID);
    }
    public void AISetUp()
    {
        _behaviorTree = new EntityBehaviorCowardMeleeType($"CowardSwordMan", _ml_EntityUID, this);
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

        if(Input.GetKeyDown(KeyCode.Z))
        {
            Vector2Int newPos = new Vector2Int(Random.Range(0, 7), Random.Range(0, 7));
            EntityMoveAgent _moveAgent;
            GetMoveAgent(out _moveAgent);
            _moveAgent.CommandMove(newPos);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this._mv2_Pos, 0.1f);
    }
    public override void OnDieEvent(Entity _entity)
    {
        _m_ActPlayer.ClearActionInfos();

        var _Lt_EntitiesGroup = EntityManager.GetInstance().NewGetEntityGroups(new EntityDivision[2] { EntityDivision.Player, EntityDivision.Rival} );

        if(_Lt_EntitiesGroup != null)
        {
            for(int i = 0; i < _Lt_EntitiesGroup.Count; ++i)
            {
                if((_Lt_EntitiesGroup[i].UniqueID == _ml_EntityGroupUID) && (_Lt_EntitiesGroup[i].ID == _mi_EntityTID))
                {
                    EntityDivision _division = _Lt_EntitiesGroup[i].GetEntityDivision();

                    switch (_division)
                    {
                        case EntityDivision.Player:
                            PlayerManager.GetInstance().AddSupply(-1);
                            UnityLogger.GetInstance().Log($"Player Supply {PlayerManager.GetInstance().GetSupply()}");
                            break;
                        case EntityDivision.Rival:
                            RivalPlayerAIManager.GetInstance().AddSupply(-1);
                            UnityLogger.GetInstance().Log($"Rival Supply {RivalPlayerAIManager.GetInstance().GetSupply()}");
                            break;
                    }
                }
            }
        }
    }
}
