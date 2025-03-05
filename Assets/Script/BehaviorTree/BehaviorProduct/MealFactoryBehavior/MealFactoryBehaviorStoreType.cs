using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class MealFactoryBehaviorStoreType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
         // �Ĵ� ���� Ÿ���� AI
        public MealFactoryBehaviorStoreType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup()
        {
            _root = new EntityBehaviorTreeSelectorNode();

            EntityBehaviorTreeSequenceNode mealFactoryDeadSequence = new EntityBehaviorTreeSequenceNode(); // ���� �Է����� ���� or ��Ʋ ������� ���� AI ����
            EntityBehaviorTreeConditionNode mealFactoryDeadCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckEntityDeadStrategy(_controller._ml_EntityUID));
            // �� ü���� 0 ���� üũ

            EntityBehaviorTreeSequenceNode mealFactoryAIStopSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));
            // �� AIStop �� �Ǿ��ִ��� - (���� üũ)

            EntityBehaviorTreeSequenceNode mealFactoryCheckEntityLimitCountSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryCheckEntityLimitCountCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckChildEntityStoreFactoryStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeConditionNode mealFactoryPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.3f)); // ���� ����
            // Entity�� �Ѱ� ī���ͱ��� ���������

            EntityBehaviorTreeActionNode mealFactoryCreateEntity = new EntityBehaviorTreeActionNode(new StoreFactoryCreateEntity(_controller._ml_EntityUID));

            _root.AddChild(mealFactoryDeadSequence);
            _root.AddChild(mealFactoryAIStopSequence);
            _root.AddChild(mealFactoryCheckEntityLimitCountSequence);

            mealFactoryDeadSequence.AddChild(mealFactoryDeadCondition);

            mealFactoryAIStopSequence.AddChild(mealFactoryAIStopCondition);

            mealFactoryCheckEntityLimitCountSequence.AddChild(mealFactoryCheckEntityLimitCountCondition);
            mealFactoryCheckEntityLimitCountSequence.AddChild(mealFactoryPreDelayCondition);
            mealFactoryCheckEntityLimitCountSequence.AddChild(mealFactoryCreateEntity);
        }
    }
}


