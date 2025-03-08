using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class RivalPlayerAIType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
        /*
         * ���� AI �÷��̾� 
         */
        public RivalPlayerAIType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup(){
            _root = new EntityBehaviorTreeSelectorNode();

            // ���� �Է����� ���� AI ����
            EntityBehaviorTreeSequenceNode userInputAIStopSequence = new EntityBehaviorTreeSequenceNode(); 
            EntityBehaviorTreeConditionNode userInputIAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionRivalPlayerInputAIStopStategy());
            // AI Stop ���� ���� ���� ���� ��Ʈ��

            EntityBehaviorTreeSequenceNode checkSupplySequence  = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode checkSupplyCondition = new EntityBehaviorTreeConditionNode(new ConditionIsOverSupplyStategy());
            //  �α��� max ���� üũ�ϴ� ����

            // Combine �� �������� üũ, ����� �̱���.

            EntityBehaviorTreeSequenceNode diaDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode diaDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // ������
            EntityBehaviorTreeConditionNode diaDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableDiaDrawPriceStategy());
            EntityBehaviorTreeActionNode diaDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunDiaDraw());
            // Dia Draw�� �������� üũ

            EntityBehaviorTreeSequenceNode goldDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode goldDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // ������
            EntityBehaviorTreeConditionNode goldDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableGoldDrawPriceStategy());
            EntityBehaviorTreeActionNode goldDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunGoldDraw());
            // Gold Draw�� �������� üũ

            _root.AddChild(userInputAIStopSequence);
            _root.AddChild(checkSupplySequence);
            _root.AddChild(diaDrawSequence);
            _root.AddChild(goldDrawSequence);

            userInputAIStopSequence.AddChild(userInputIAIStopCondition);

            checkSupplySequence.AddChild(checkSupplyCondition);//

            diaDrawSequence.AddChild(diaDrawPreDelayCondition);//
            diaDrawSequence.AddChild(diaDrawUseableCondition);
            diaDrawSequence.AddChild(diaDrawSpawnAction);

            goldDrawSequence.AddChild(goldDrawPreDelayCondition);
            goldDrawSequence.AddChild(goldDrawUseableCondition);
            goldDrawSequence.AddChild(goldDrawSpawnAction);
        }
    }
}


