using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EntityBehaviorTree
{
    public abstract class EntityBehaviorTreeDecoratorNode : EntityBehaviorTreeNodeBase
    {
        protected EntityBehaviorTreeNodeBase _childNode;
        public EntityBehaviorTreeDecoratorNode(EntityBehaviorTreeNodeBase childNode)
        {
            this._childNode = childNode;
        }
        public override BTNodeState Evaluate()
        {
            if (_childNode == null)
                return BTNodeState.Failure;

            return OnEvaluate();
        }
        protected abstract BTNodeState OnEvaluate();
    }
    /// <summary>
    /// 일정 조건에 맞춰 사용되는 Decorator Node
    /// </summary>
    public class EntityBehaviorTreeDecoratorConditionNode : EntityBehaviorTreeDecoratorNode, IDisposable
    {
        private Func<bool> _OnProcConditionCallback; // 조건
        public EntityBehaviorTreeDecoratorConditionNode(EntityBehaviorTreeNodeBase childNode, Func<bool> condition) : base(childNode)
        {
            this._OnProcConditionCallback = condition;
        }

        protected override BTNodeState OnEvaluate()
        {
            if(_OnProcConditionCallback == null || !_OnProcConditionCallback.Invoke())
                return BTNodeState.Failure;
            // 자식 노드가 없거나, 실패할 때 Failure

            return _childNode.Evaluate();
        }
        public void Dispose()
        {
        }
    }
    /// <summary>
    /// 일정 반복 횟수에 맞춰 사용되는 Decorator Node
    /// </summary>
    public class EntityBehaviorTreeDecoratorRepeatNode : EntityBehaviorTreeDecoratorNode, IDisposable
    {
        private int _mi_RepeatCount;
        private int _mi_CurrentCount;

        public EntityBehaviorTreeDecoratorRepeatNode(EntityBehaviorTreeNodeBase childNode, int _repeatCount) : base(childNode)
        {
            this._mi_RepeatCount = _repeatCount;
            this._mi_CurrentCount = 0;
        }
        protected override BTNodeState OnEvaluate()
        {
            if (_mi_CurrentCount < _mi_RepeatCount)
            {
                BTNodeState result = _childNode.Evaluate();

                if (result == BTNodeState.Running)
                    return BTNodeState.Running;

                _mi_CurrentCount++;
                return BTNodeState.Running; // 반복 중인 상태
            }

            // 반복 완료
            _mi_CurrentCount = 0;
            return BTNodeState.Success;
        }
        public void Dispose()
        {
        }
    }
    /// <summary>
    /// 실행 결과와 반대로 반환되는 Decorator Node
    /// </summary>
    public class EntityBehaviorTreeInverterNode : EntityBehaviorTreeDecoratorNode
    {
        public EntityBehaviorTreeInverterNode(EntityBehaviorTreeNodeBase childNode)
            : base(childNode)
        {
        }

        protected override BTNodeState OnEvaluate()
        {
            BTNodeState result = _childNode.Evaluate();

            switch (result)
            {
                case BTNodeState.Success:
                    return BTNodeState.Failure;
                case BTNodeState.Failure:
                    return BTNodeState.Success;
                default:
                    return result; // Running은 그대로 반환
            }
        }
    }

    /// <summary>
    /// 지연시간이 지난 후 실행되고, timeout 에 걸리면 실패되는 Timer Decorator Node
    /// </summary>
    public class EntityBehaviorTreeTimerNode : EntityBehaviorTreeDecoratorNode
    {
        private float delayTime;      // 지연 시간
        private float timeout;          // 제한 시간
        private float startTime;        // 타이머 시작 시간
        private bool isStarted;         // 타이머 시작 여부

        public EntityBehaviorTreeTimerNode(EntityBehaviorTreeNodeBase childNode, float delayTime, float timeout)
            : base(childNode)
        {
            this.delayTime = delayTime;
            this.timeout = timeout;
            this.isStarted = false;
        }

        protected override BTNodeState OnEvaluate()
        {
            // 타이머 시작
            if (!isStarted)
            {
                startTime = Time.time; // Unity의 Time.time 사용
                isStarted = true;
            }

            float elapsedTime = Time.time - startTime;

            // 지연 시간이 지나지 않았다면 실행 대기 (Running 반환)
            if (elapsedTime < delayTime)
                return BTNodeState.Running;

            // 제한 시간이 초과되었다면 실패 반환
            if (timeout > 0 && elapsedTime > timeout)
            {
                ResetTimer();
                return BTNodeState.Failure;
            }

            // 자식 노드를 실행
            var result = _childNode.Evaluate();

            // 자식 노드가 완료되면 타이머 초기화
            if (result != BTNodeState.Running)
                ResetTimer();

            return result;
        }

        private void ResetTimer()
        {
            isStarted = false;
        }
    }
}


