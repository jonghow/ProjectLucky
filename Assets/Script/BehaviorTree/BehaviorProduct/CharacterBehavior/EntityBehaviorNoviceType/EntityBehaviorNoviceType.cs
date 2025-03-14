using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public class EntityBehaviorNoviceType : EntityBehaviorTreeBase , IBehaviorTreeSetter
    {
        /*
         * 견습 검사 타입의 AI
         */
        public EntityBehaviorNoviceType(string _name, int _uniqueID, EntityContoller _controller) : base(_name, _uniqueID,_controller) {
            AISetup();
        }

        public virtual void AISetup()
        {
            _root = new EntityBehaviorTreeSelectorNode();

            //EntityBehaviorTreeSequenceNode detectRangeSequence = new EntityBehaviorTreeSequenceNode();

            //EntityBehaviorTreeConditionNode detectRangeCondition = new EntityBehaviorTreeConditionNode(new ConditionDetectRangeStategy(_controller.transform, _controller._m_Target, 5f));
            //EntityBehaviorTreeActionNode detectRangeProbe = new EntityBehaviorTreeActionNode(new MonsterProbe(_controller.transform));

            //EntityBehaviorTreeSequenceNode atkSequence = new EntityBehaviorTreeSequenceNode();
            //EntityBehaviorTreeConditionNode atkPreDelayAction = new EntityBehaviorTreeConditionNode(new ConditionAtkPreDelayStrategy(2f)); // 지금은 선딜
            //EntityBehaviorTreeActionNode atkAction = new EntityBehaviorTreeActionNode(new NormalAtkStrategy(_controller._ml_EntityUID));

            //EntityBehaviorTreeSequenceNode chaseSequence = new EntityBehaviorTreeSequenceNode();
            //EntityBehaviorTreeConditionNode playerInRange = new EntityBehaviorTreeConditionNode(new ConditionRangeStrategy(_controller.transform, _controller._m_Target, 2f));
            //EntityBehaviorTreeActionNode chaseAction = new EntityBehaviorTreeActionNode(new ChaseStrategy(_controller.transform, _controller._m_Target));


            //_root.AddChild(detectRangeSequence);
            //_root.AddChild(atkSequence);
            //_root.AddChild(chaseSequence);

            //detectRangeSequence.AddChild(detectRangeCondition);
            //detectRangeSequence.AddChild(detectRangeProbe);

            //atkSequence.AddChild(atkPreDelayAction);//
            //atkSequence.AddChild(atkAction);

            //chaseSequence.AddChild(playerInRange);
            //chaseSequence.AddChild(chaseAction);
        }

        public void Evaluate()
        {
            this._root.Evaluate();
        }
    }
}


