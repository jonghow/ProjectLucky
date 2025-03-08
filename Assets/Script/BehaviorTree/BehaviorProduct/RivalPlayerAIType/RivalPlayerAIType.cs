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

            // Combine 이 가능한지 체크, 현재는 미구현.

            EntityBehaviorTreeSequenceNode diaDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode diaDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // 딜레이
            EntityBehaviorTreeConditionNode diaDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableDiaDrawPriceStategy());
            EntityBehaviorTreeActionNode diaDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunDiaDraw());
            // Dia Draw가 가능한지 체크

            EntityBehaviorTreeSequenceNode goldDrawSequence = new EntityBehaviorTreeSequenceNode();
            EntityBehaviorTreeConditionNode goldDrawPreDelayCondition = new EntityBehaviorTreeConditionNode(new ConditionPreDelayStrategy(0.1f)); // 딜레이
            EntityBehaviorTreeConditionNode goldDrawUseableCondition = new EntityBehaviorTreeConditionNode(new ConditionConsumableGoldDrawPriceStategy());
            EntityBehaviorTreeActionNode goldDrawSpawnAction = new EntityBehaviorTreeActionNode(new RivalRunGoldDraw());
            // Gold Draw가 가능한지 체크

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


