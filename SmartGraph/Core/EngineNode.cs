#region Copyright (c) 2020 Filip Fodemski
// 
// Copyright (c) 2020 Filip Fodemski
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

using System;
using System.Collections.Generic;
using System.Linq;
using SmartGraph.Common;
using SmartGraph.Core.Interfaces;
using SmartGraph.Dag.Interfaces;

namespace SmartGraph.Core
{
    internal class EngineNode : IEngineNode
	{
		private EngineCore engine;
        private IDictionary<string, string> inputMap;

        private string ReverseInput(string name)
        {
            return inputMap.First(i => i.Value == name).Key;
        }

		public EngineNode(EngineCore engine, INode node, IVertex vertex) : base()
		{
            Guard.AssertNotNull(engine, "engine");
			Guard.AssertNotNull(node, "node");

            this.engine = engine;

            Vertex = vertex;
            Node = node;

            inputMap = engine.Builder.GetInputs(vertex);
            
            InputValues = new Dictionary<string, IEngineNode>();

			Bind();
		}

        public void Dispose()
        {
            if (Node is IDisposable)
            {
                ((IDisposable)Node).Dispose();
            }
        }

        public IDictionary<string, IEngineNode> InputValues { get; private set; }

        public IVertex Vertex { get; private set; }
        
        public INode Node { get; private set; }

        public object Value { get; set; }

        public void MarkNodeAsDirty()
		{
            if (!(Node is IActiveNode))
            {
                var msg = string.Format(
                    "A non-IActiveNode '{0}' raised the dirty event.", Node.Name);
                throw new InvalidOperationException(msg);
            }

            engine.MarkNodeAsDirty(this);
        }

		public void Update()
		{
			// Update inputs and then we can update.

            // Remember to disconnect the key collection because the
            // collection is updated for each input (see the ForEach action).
            var inputValues = InputValues.Keys.ToArray();
            inputValues.ForEach(s => InputValues[s] = engine[inputMap[s]]);

            Node.Update();
		}

		public void Bind()
	    {
            // Initialize the InputValues collection with all the names of inputs.

            Vertex.InEdges.ForEach(e => InputValues.Add(ReverseInput(e.Source.Name), null));

            Node.Bind(this);
		}

		public void Activate()
		{
            if (Node is IActiveNode)
            {
                ((IActiveNode)Node).Activate();
            }
		}
	}
}
