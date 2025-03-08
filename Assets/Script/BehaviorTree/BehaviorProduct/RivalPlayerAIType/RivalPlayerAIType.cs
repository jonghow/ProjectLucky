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

            EntityBehaviorTreeSequenceNode combineSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode combineEnableCondition = new EntityBehaviorTreeConditionNode(new ConditionEnableCombineStategy());
            EntityBehaviorTreeConditionNode combinePreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(1f)); // ������
            EntityBehaviorTreeActionNode combineAction = new EntityBehaviorTreeActionNode(new RivalRunCombine());
            // Combine �� �������� üũ, 

            EntityBehaviorTreeSequenceNode diaDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode diaDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableDiaDrawPriceStategy());
            EntityBehaviorTreeConditionNode diaDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(2.3f)); // ������
            EntityBehaviorTreeActionNode diaDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunDiaDraw());
            // Dia Draw�� �������� üũ

            EntityBehaviorTreeSequenceNode goldDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode goldDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableGoldDrawPriceStategy());
            EntityBehaviorTreeConditionNode goldDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(3.3f)); // ������
            EntityBehaviorTreeActionNode goldDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunGoldDraw());
            // Gold Draw�� �������� üũ

            _root.AddChild(userInputAIStopSequence);
            _root.AddChild(checkSupplySequence);
            _root.AddChild(combineSequence);
            _root.AddChild(diaDrawSequence);
            _root.AddChild(goldDrawSequence);

            userInputAIStopSequence.AddChild(userInputIAIStopCondition);

            checkSupplySequence.AddChild(checkSupplyCondition);//

            combineSequence.AddChild(combineEnableCondition);
            combineSequence.AddChild(combinePreDelayCondition);
            combineSequence.AddChild(combineAction);

            diaDrawSequence.AddChild(diaDrawUseableCondition);
            diaDrawSequence.AddChild(diaDrawPreDelayCondition);//
            diaDrawSequence.AddChild(diaDrawSpawnAction);

            goldDrawSequence.AddChild(goldDrawUseableCondition);
            goldDrawSequence.AddChild(goldDrawPreDelayCondition);
            goldDrawSequence.AddChild(goldDrawSpawnAction);
        }
    }
}


