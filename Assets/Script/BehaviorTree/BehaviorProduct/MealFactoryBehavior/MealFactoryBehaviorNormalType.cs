using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class MealFactoryBehaviorNormalType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
         // 떡볶이 공장 타입의 AI
        public MealFactoryBehaviorNormalType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup()
        {
            _root = new EntityBehaviorTreeSelectorNode();

            EntityBehaviorTreeSequenceNode mealFactoryDeadSequence = new EntityBehaviorTreeSequenceNode(); // 유저 입력으로 인한 or 배틀 페이즈로 인한 AI 종료
            EntityBehaviorTreeConditionNode mealFactoryDeadCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckEntityDeadStrategy(_controller._ml_EntityUID));
            // 내 체력이 0 인지 체크

            EntityBehaviorTreeSequenceNode mealFactoryContainMealKitSequence = new EntityBehaviorTreeSequenceNode(); // 유저 입력으로 인한 or 배틀 페이즈로 인한 AI 종료
            EntityBehaviorTreeConditionNode mealFactoryContainMealKitCondition = new EntityBehaviorTreeConditionNode(new ConditionIsContainMealkitStrategy(_controller._ml_EntityUID));
            // 내 솥에 밀키드가 부어져있는지 체크

            EntityBehaviorTreeSequenceNode mealFactoryAIStopSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));
            // 내 AIStop 이 되어있는지 - (유저 체크)

            EntityBehaviorTreeSequenceNode mealFactoryCheckEntityLimitCountSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryCheckEntityLimitCountCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckChildEntityLimitStrategy(_controller._ml_EntityUID));
            // Entity를 한계 카운터까지 만들었는지

            EntityBehaviorTreeSequenceNode mealFactoryCheckOverHeatingSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryCheckOverHeatingCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckOverHeatingStrategy(_controller._ml_EntityUID));
            // 현재 솥의 상태가 가열 상태인지? 

            EntityBehaviorTreeSequenceNode mealFactoryProcessCookTimeSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryProcessCooktTimeCondition = new EntityBehaviorTreeConditionNode(new ConditionProcessCookingTimeStrategy(_controller._ml_EntityUID));
            // 현재 만드는 시간인지 판단하고

            EntityBehaviorTreeActionNode mealFactoryCreateEntity = new EntityBehaviorTreeActionNode(new MealFactoryCreateEntity(_controller._ml_EntityUID));
            // 요리시간 되었다면 만든다.

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


