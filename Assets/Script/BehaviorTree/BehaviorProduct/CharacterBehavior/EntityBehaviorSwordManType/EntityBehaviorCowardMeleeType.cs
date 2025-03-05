using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class EntityBehaviorCowardMeleeType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
        /*
         * �߽� �˻� Ÿ���� AI
         */
        public EntityBehaviorCowardMeleeType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup(){
            _root = new EntityBehaviorTreeSelectorNode();

            EntityBehaviorTreeSequenceNode userInputAIStopSequence = new EntityBehaviorTreeSequenceNode(); // ���� �Է����� ���� or ��Ʋ ������� ���� AI ����
            EntityBehaviorTreeConditionNode userInputIAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode atkSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode playerInRange = new EntityBehaviorTreeConditionNode(new ConditionRangeStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode atkAction = new EntityBehaviorTreeActionNode(new NormalAtkStrategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode detectSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode detectPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // ���� ������
            EntityBehaviorTreeConditionNode detectSequenceCondition = new EntityBehaviorTreeConditionNode(new ConditionDetectRangeStrategy(_controller._ml_EntityUID));
            EntityBehaviorTreeActionNode detectEnemyfindCondition = new EntityBehaviorTreeActionNode(new EnemyFindStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeSequenceNode chaseSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode chasePreDelayAction = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // �̵� ��������
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


