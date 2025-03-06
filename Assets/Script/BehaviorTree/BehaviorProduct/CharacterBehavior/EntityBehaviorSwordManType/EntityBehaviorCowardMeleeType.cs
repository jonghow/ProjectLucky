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

            // 유저 입력으로 인해 AI 정지
            EntityBehaviorTreeSequenceNode userInputAIStopSequence = new EntityBehaviorTreeSequenceNode(); // 유저 입력으로 인한 or 배틀 페이즈로 인한 AI 종료
            EntityBehaviorTreeConditionNode userInputIAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode idleSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode idleOutofAttackRangeCondition = new EntityBehaviorTreeConditionNode(new ConditionRangeInverseStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode idleAction = new EntityBehaviorTreeActionNode(new IdleStrategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode atkSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode playerInRange = new EntityBehaviorTreeConditionNode(new ConditionRangeStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode atkAction = new EntityBehaviorTreeActionNode(new NormalAtkStrategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode detectSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeActionNode detectEnemyfindAction = new EntityBehaviorTreeActionNode(new EnemyFindStategy(_controller._ml_EntityUID));

            _root.AddChild(userInputAIStopSequence);
            _root.AddChild(idleSequence);
            _root.AddChild(atkSequence);
            _root.AddChild(detectSequence);

            userInputAIStopSequence.AddChild(userInputIAIStopCondition);

            idleSequence.AddChild(idleOutofAttackRangeCondition);//
            idleSequence.AddChild(idleAction);

            atkSequence.AddChild(playerInRange);//
            atkSequence.AddChild(atkAction);

            detectSequence.AddChild(detectEnemyfindAction);
        }
    }
}


