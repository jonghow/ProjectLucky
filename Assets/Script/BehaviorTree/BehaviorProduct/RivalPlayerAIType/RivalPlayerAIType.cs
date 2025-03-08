using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class RivalPlayerAIType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
        /*
         * 적군 AI 플레이어 
         */
        public RivalPlayerAIType(string _name, long _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup(){
            _root = new EntityBehaviorTreeSelectorNode();

            // 유저 입력으로 인해 AI 정지
            EntityBehaviorTreeSequenceNode userInputAIStopSequence = new EntityBehaviorTreeSequenceNode(); 
            EntityBehaviorTreeConditionNode userInputIAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionRivalPlayerInputAIStopStategy());
            // AI Stop 으로 구매 조합 등을 컨트롤

            EntityBehaviorTreeSequenceNode checkSupplySequence  = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode checkSupplyCondition = new EntityBehaviorTreeConditionNode(new ConditionIsOverSupplyStategy());
            //  인구가 max 인지 체크하는 구문

            EntityBehaviorTreeSequenceNode combineSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode combineEnableCondition = new EntityBehaviorTreeConditionNode(new ConditionEnableCombineStategy());
            EntityBehaviorTreeConditionNode combinePreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(1f)); // 딜레이
            EntityBehaviorTreeActionNode combineAction = new EntityBehaviorTreeActionNode(new RivalRunCombine());
            // Combine 이 가능한지 체크, 

            EntityBehaviorTreeSequenceNode diaDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode diaDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableDiaDrawPriceStategy());
            EntityBehaviorTreeConditionNode diaDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(2.3f)); // 딜레이
            EntityBehaviorTreeActionNode diaDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunDiaDraw());
            // Dia Draw가 가능한지 체크

            EntityBehaviorTreeSequenceNode goldDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode goldDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableGoldDrawPriceStategy());
            EntityBehaviorTreeConditionNode goldDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(3.3f)); // 딜레이
            EntityBehaviorTreeActionNode goldDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunGoldDraw());
            // Gold Draw가 가능한지 체크

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


