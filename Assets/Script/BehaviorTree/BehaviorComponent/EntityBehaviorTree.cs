using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviorTree
{
    public enum BTNodeState
    {
        Success,
        Failure,
        Running
    }

    public interface IBehaviorTreeSetter
    {
        void AISetup();
    }

    public  class EntityBehaviorTreeBase 
    {
        private string _m_name;
        private long _ml_UniqueID;
        protected EntityBehaviorTreeSelectorNode _root;
        protected EntityContoller _controller;

        public EntityBehaviorTreeBase(string _name, long _uniqueID, EntityContoller _controller)
        {
            this._m_name = _name;
            this._ml_UniqueID = _uniqueID;
            this._controller = _controller;
        }

        public void Evaluate()
        {
            _root.Evaluate();
        }
    }
}


