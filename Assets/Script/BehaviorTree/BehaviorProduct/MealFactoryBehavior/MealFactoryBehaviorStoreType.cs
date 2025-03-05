using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class MealFactoryBehaviorStoreType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
         // 식당 공장 타입의 AI
        public MealFactoryBehaviorStoreType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup()
        {
            _root = new EntityBehaviorTreeSelectorNode();

            EntityBehaviorTreeSequenceNode mealFactoryDeadSequence = new EntityBehaviorTreeSequenceNode(); // 유저 입력으로 인한 or 배틀 페이즈로 인한 AI 종료
            EntityBehaviorTreeConditionNode mealFactoryDeadCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckEntityDeadStrategy(_controller._ml_EntityUID));
            // 내 체력이 0 인지 체크

            EntityBehaviorTreeSequenceNode mealFactoryAIStopSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));
            // 내 AIStop 이 되어있는지 - (유저 체크)

            EntityBehaviorTreeSequenceNode mealFactoryCheckEntityLimitCountSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode mealFactoryCheckEntityLimitCountCondition = new EntityBehaviorTreeConditionNode(new ConditionCheckChildEntityStoreFactoryStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeConditionNode mealFactoryPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.3f)); // 생성 선딜
            // Entity를 한계 카운터까지 만들었는지

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


