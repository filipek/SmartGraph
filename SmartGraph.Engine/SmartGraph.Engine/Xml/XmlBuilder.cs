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
using SmartGraph.Engine.Dag;
using SmartGraph.Engine.Nodes;
using SmartGraph.Engine.Xml.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartGraph.Engine.Core;

namespace SmartGraph.Engine.Xml
{
	public class XmlEngineBuilder : IEngineBuilder
	{
		public const String EngineNamespace = "urn:smartgraph:engine";
        public const String NodesNamespace = "urn:smartgraph:engine:nodes";

        private DagGraph dagGraph;
        private CEngine engineXml;
        private IDictionary<String, CNode> engineNodes;

        public XmlEngineBuilder(CEngine xml)
        {
            engineXml = xml;
            engineNodes = new Dictionary<String, CNode>();
        }

		public IGraph CreateGraph()
		{
            dagGraph = new DagGraph(engineXml.name.Name);

            foreach (var node in engineXml.node)
            {
                var nodeName = node.name.Name;
                engineNodes[nodeName] = node;

                var target = new Vertex(dagGraph, nodeName);
                dagGraph.Vertices.Add(target);

                if (node.inputs != null)
                {
                    foreach (var input in node.inputs)
                    {
                        var sourceName = input.@ref.Name;
                        var source = dagGraph.Vertices.First(x => x.Name == sourceName);

                        dagGraph.Edges.Add(new Edge(dagGraph, sourceName, source, target));
                    }
                }
            }

            return dagGraph;
		}

		public INode CreateNode(IVertex vertex)
		{
            Guard.AssertNotNull(vertex, "vertex");

            var name = vertex.Name;
            var node = engineNodes[name];

            Type type = Type.GetType(node.obj.type, true, true);

            NodeBase engineNode;
            if (node.obj.Any == null)
            {
                engineNode = (NodeBase)Activator.CreateInstance(type);
            }
            else
            {
                engineNode = (NodeBase)XmlHelpers.Deserialize(
                    type, node.obj.Any.OuterXml, NodesNamespace);
            }

            engineNode.Name = node.name.Name;
            return engineNode;
        }

        public IDictionary<String, String> GetInputs(IVertex vertex)
        {
            Guard.AssertNotNull(vertex, "vertex");

            var name = vertex.Name;
            var node = engineNodes[name];

            var nodeInputs = new Dictionary<String, String>();

            if (node.inputs != null)
            {
                foreach (var input in node.inputs)
                {
                    nodeInputs.Add(input.name.Name, input.@ref.Name);
                }
            }

            return nodeInputs;
        }
    }
}
