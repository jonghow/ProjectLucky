using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class EntityBehaviorCowardMeleeType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
        /*
         * 견습 검사 타입의 AI
         */
        public EntityBehaviorCowardMeleeType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup(){
            _root = new EntityBehaviorTreeSelectorNode();

            EntityBehaviorTreeSequenceNode userInputAIStopSequence = new EntityBehaviorTreeSequenceNode(); // 유저 입력으로 인한 or 배틀 페이즈로 인한 AI 종료
            EntityBehaviorTreeConditionNode userInputIAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode atkSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode playerInRange = new EntityBehaviorTreeConditionNode(new ConditionRangeStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode atkAction = new EntityBehaviorTreeActionNode(new NormalAtkStrategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode detectSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode detectPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // 감지 딜레이
            EntityBehaviorTreeConditionNode detectSequenceCondition = new EntityBehaviorTreeConditionNode(new ConditionDetectRangeStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode detectEnemyfindCondition = new EntityBehaviorTreeActionNode(new EnemyFindStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode chaseSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode chasePreDelayAction = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // 이동 선딜레이
            EntityBehaviorTreeActionNode chaseAction = new EntityBehaviorTreeActionNode(new ChaseStrategy(_controller._ml_EntityUID));

            _root.AddChild(userInputAIStopSequence);
            _root.AddChild(atkSequence);
            _root.AddChild(detectSequence);
            _root.AddChild(chaseSequence);

            userInputAIStopSequence.AddChild(userInputIAIStopCondition); 

            atkSequence.AddChild(playerInRange);//
            atkSequence.AddChild(atkAction);

            detectSequence.AddChild(detectSequenceCondition);
            detectSequence.AddChild(detectEnemyfindCondition);

            chaseSequence.AddChild(chasePreDelayAction);
            chaseSequence.AddChild(chaseAction);
        }
    }
}


