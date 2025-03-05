using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class MealFactoryBehaviorNormalType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
         // ������ ���� Ÿ���� AI
        public MealFactoryBehaviorNormalType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup()
        {
            _root = new EntityBehaviorTreeSelectorNode();

            EntityBehaviorTreeSequenceNode mealFactoryDeadSequence = new EntityBehaviorTreeSequenceNode(); // ���� �Է����� ���� or ��Ʋ ������� ���� AI ����
            EntityBehaviorTreeConditionNode mealFactoryDeadCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckEntityDeadStrategy(_controller._ml_EntityUID));
            // �� ü���� 0 ���� üũ

            EntityBehaviorTreeSequenceNode mealFactoryContainMealKitSequence = new EntityBehaviorTreeSequenceNode(); // ���� �Է����� ���� or ��Ʋ ������� ���� AI ����
            EntityBehaviorTreeConditionNode mealFactoryContainMealKitCondition = new EntityBehaviorTreeConditionNode(new ConditionIsContainMealkitStrategy(_controller._ml_EntityUID));
            // �� �ܿ� ��Ű�尡 �ξ����ִ��� üũ

            EntityBehaviorTreeSequenceNode mealFactoryAIStopSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));
            // �� AIStop �� �Ǿ��ִ��� - (���� üũ)

            EntityBehaviorTreeSequenceNode mealFactoryCheckEntityLimitCountSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryCheckEntityLimitCountCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckChildEntityLimitStrategy(_controller._ml_EntityUID));
            // Entity�� �Ѱ� ī���ͱ��� ���������

            EntityBehaviorTreeSequenceNode mealFactoryCheckOverHeatingSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryCheckOverHeatingCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckOverHeatingStrategy(_controller._ml_EntityUID));
            // ���� ���� ���°� ���� ��������? 

            EntityBehaviorTreeSequenceNode mealFactoryProcessCookTimeSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryProcessCooktTimeCondition = new EntityBehaviorTreeConditionNode(new ConditionProcessCookingTimeStrategy(_controller._ml_EntityUID));
            // ���� ����� �ð����� �Ǵ��ϰ�

            EntityBehaviorTreeActionNode mealFactoryCreateEntity = new EntityBehaviorTreeActionNode(new MealFactoryCreateEntity(_controller._ml_EntityUID));
            // �丮�ð� �Ǿ��ٸ� �����.

            _root.AddChild(mealFactoryDeadSequence);
            _root.AddChild(mealFactoryContainMealKitSequence);
            _root.AddChild(mealFactoryAIStopSequence);
            _root.AddChild(mealFactoryCheckEntityLimitCountSequence);
            _root.AddChild(mealFactoryCheckOverHeatingSequence);
            _root.AddChild(mealFactoryProcessCookTimeSequence);

            mealFactoryDeadSequence.AddChild(mealFactoryDeadCondition);

            mealFactoryContainMealKitSequence.AddChild(mealFactoryContainMealKitCondition);

            mealFactoryAIStopSequence.AddChild(mealFactoryAIStopCondition);

            mealFactoryCheckEntityLimitCountSequence.AddChild(mealFactoryCheckEntityLimitCountCondition);

            mealFactoryCheckOverHeatingSequence.AddChild(mealFactoryCheckOverHeatingCondition);

            mealFactoryProcessCookTimeSequence.AddChild(mealFactoryProcessCooktTimeCondition);
            mealFactoryProcessCookTimeSequence.AddChild(mealFactoryCreateEntity);
        }
    }
}


