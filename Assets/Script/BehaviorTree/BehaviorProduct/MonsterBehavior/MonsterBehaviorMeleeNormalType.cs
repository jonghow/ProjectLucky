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

            EntityBehaviorTreeSequenceNode userInputAIStopSequence = new EntityBehaviorTreeSequenceNode(); // ���� �Է����� ���� or ��Ʋ ������� ���� AI ����
            EntityBehaviorTreeConditionNode userInputIAIStopCondition = new EntityBehaviorTreeConditionNode(new ConditionUserInputAIStopStategy(_controller._ml_EntityUID));

            EntityBehaviorTreeActionNode monsterProbe = new EntityBehaviorTreeActionNode(new MonsterProbe(_controller._ml_EntityUID));

            _root.AddChild(userInputAIStopSequence);
            _root.AddChild(monsterProbe);

            userInputAIStopSequence.AddChild(userInputIAIStopCondition);
        }
    }
}


