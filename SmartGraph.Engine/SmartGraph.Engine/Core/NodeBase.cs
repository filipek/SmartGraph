#region Copyright (C) 2015 Filip Fodemski
// 
// Copyright (c) 2015 Filip Fodemski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
// (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
//
#endregion

using SmartGraph.Engine.Common;
using SmartGraph.Engine.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace SmartGraph.Engine.Nodes
{
    [Serializable]
    public abstract class NodeBase : INode
    {
        protected IEngineNode nodeHost;
        protected String nodeName;

        protected IDictionary<String, IEngineNode> InputValues { get { return nodeHost.InputValues; } }

        protected Object Value
        {
            get { return nodeHost.Value; }
            set { nodeHost.Value = value; }
        }

        protected T GetInputNode<T>(String inputName)
        {
            return (T)((IEngineNode)InputValues[inputName]).Value;
        }

        public abstract void Update();

        public void Bind(IEngineNode host)
        {
            Guard.AssertNotNull(host, "host");

            nodeHost = host;
        }

        public String Name
        {
            get { return nodeName; }
            set
            {
                Guard.AssertNotNullOrEmpty(value, "value");

                nodeName = value;
            }
        }
    }

    [Serializable]
    public abstract class ActiveNodeBase : NodeBase, IActiveNode
    {
        protected virtual void InnerMarkNodeAsDirty()
        {
            nodeHost.MarkNodeAsDirty();
        }

        public abstract void Activate();

        public virtual void MarkNodeAsDirty()
        {
            nodeHost.MarkNodeAsDirty();
        }

        public override void Update()
        {
            throw new InvalidOperationException("IActiveNodes cannot be updated");
        }
    }
}
