using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{

    public class MonsterBehaviorMeleeNormalType : EntityBehaviorTreeBase, IBehaviorTreeSetter
    {
        public MonsterBehaviorMeleeNormalType(string _name, int _uniqueID, EntityContoller _controller) : base(_name, _uniqueID, _controller)
        {
            AISetup();
        }

        public virtual void AISetup()
        {
            _root = new EntityBehaviorTreeSelectorNode();

            EntityBehaviorTreeSequenceNode userInputAIStopSequence = new EntityBehaviorTreeSequenceNode(); // 유저 입력으로 인한 or 배틀 페이즈로 인한 AI 종료
            EntityBehaviorTreeConditionNode userInputIAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode enemyfindSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode enemyfindCondition = new EntityBehaviorTreeConditionNode(new ConditionEnemyFindStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode atkSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode playerInRange = new EntityBehaviorTreeConditionNode(new ConditionRangeStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode atkAction = new EntityBehaviorTreeActionNode(new NormalAtkStrategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode chaseSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode chasePreDelayAction = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // 이동 선딜레이
            EntityBehaviorTreeConditionNode chaseInRange = new EntityBehaviorTreeConditionNode(new ConditionRangeInverseStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode chaseAction = new EntityBehaviorTreeActionNode(new ChaseStrategy(_controller._ml_EntityUID));

            _root.AddChild(userInputAIStopSequence);
            _root.AddChild(enemyfindSequence);
            _root.AddChild(atkSequence);
            _root.AddChild(chaseSequence);

            userInputAIStopSequence.AddChild(userInputIAIStopCondition);

            enemyfindSequence.AddChild(enemyfindCondition);

            atkSequence.AddChild(playerInRange);//
            atkSequence.AddChild(atkAction);

            chaseSequence.AddChild(chasePreDelayAction);
            chaseSequence.AddChild(chaseInRange);
            chaseSequence.AddChild(chaseAction);
        }
    }
}


