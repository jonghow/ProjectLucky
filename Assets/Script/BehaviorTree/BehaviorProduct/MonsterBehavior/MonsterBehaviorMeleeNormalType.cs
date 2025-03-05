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

            EntityBehaviorTreeActionNode monsterProbe = new EntityBehaviorTreeActionNode(new MonsterProbe(_controller._ml_EntityUID));

            _root.AddChild(userInputAIStopSequence);
            _root.AddChild(monsterProbe);

            userInputAIStopSequence.AddChild(userInputIAIStopCondition);
        }
    }
}


