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
    /// ���� ���ǿ� ���� ���Ǵ� Decorator Node
    /// </summary>
    public class EntityBehaviorTreeDecoratorConditionNode : EntityBehaviorTreeDecoratorNode, IDisposable
    {
        private Func<bool> _OnProcConditionCallback; // ����
        public EntityBehaviorTreeDecoratorConditionNode(EntityBehaviorTreeNodeBase childNode, Func<bool> condition) : base(childNode)
        {
            this._OnProcConditionCallback = condition;
        }

        protected override BTNodeState OnEvaluate()
        {
            if(_OnProcConditionCallback == null || !_OnProcConditionCallback.Invoke())
                return BTNodeState.Failure;
            // �ڽ� ��尡 ���ų�, ������ �� Failure

            return _childNode.Evaluate();
        }
        public void Dispose()
        {
        }
    }
    /// <summary>
    /// ���� �ݺ� Ƚ���� ���� ���Ǵ� Decorator Node
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
                return BTNodeState.Running; // �ݺ� ���� ����
            }

            // �ݺ� �Ϸ�
            _mi_CurrentCount = 0;
            return BTNodeState.Success;
        }
        public void Dispose()
        {
        }
    }
    /// <summary>
    /// ���� ����� �ݴ�� ��ȯ�Ǵ� Decorator Node
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
                    return result; // Running�� �״�� ��ȯ
            }
        }
    }

    /// <summary>
    /// �����ð��� ���� �� ����ǰ�, timeout �� �ɸ��� ���еǴ� Timer Decorator Node
    /// </summary>
    public class EntityBehaviorTreeTimerNode : EntityBehaviorTreeDecoratorNode
    {
        private float delayTime;      // ���� �ð�
        private float timeout;          // ���� �ð�
        private float startTime;        // Ÿ�̸� ���� �ð�
        private bool isStarted;         // Ÿ�̸� ���� ����

        public EntityBehaviorTreeTimerNode(EntityBehaviorTreeNodeBase childNode, float delayTime, float timeout)
            : base(childNode)
        {
            this.delayTime = delayTime;
            this.timeout = timeout;
            this.isStarted = false;
        }

        protected override BTNodeState OnEvaluate()
        {
            // Ÿ�̸� ����
            if (!isStarted)
            {
                startTime = Time.time; // Unity�� Time.time ���
                isStarted = true;
            }

            float elapsedTime = Time.time - startTime;

            // ���� �ð��� ������ �ʾҴٸ� ���� ��� (Running ��ȯ)
            if (elapsedTime < delayTime)
                return BTNodeState.Running;

            // ���� �ð��� �ʰ��Ǿ��ٸ� ���� ��ȯ
            if (timeout > 0 && elapsedTime > timeout)
            {
                ResetTimer();
                return BTNodeState.Failure;
            }

            // �ڽ� ��带 ����
            var result = _childNode.Evaluate();

            // �ڽ� ��尡 �Ϸ�Ǹ� Ÿ�̸� �ʱ�ȭ
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


