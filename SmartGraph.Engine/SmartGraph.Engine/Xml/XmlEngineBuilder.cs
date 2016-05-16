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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SmartGraph.Engine.Common;
using SmartGraph.Engine.Core;
using SmartGraph.Engine.Dag;
using SmartGraph.Engine.Nodes;
using SmartGraph.Engine.Xml.Schema;

namespace SmartGraph.Engine.Xml
{
    public class XmlEngineBuilder : IEngineBuilder
	{
		public const String EngineNamespace = "urn:smartgraph:engine";
        public const String NodesNamespace = "urn:smartgraph:engine:nodes";

        private readonly CEngine engineXml;
        private readonly Dictionary<String, CNode> engineNodes = new Dictionary<String, CNode>();

        public XmlEngineBuilder(string url)
        {
            Guard.AssertNotNullOrWhitespace(url, nameof(url));

            using (var xmlReader = new XmlTextReader(url))
            {
                engineXml = (CEngine)XmlHelpers.Deserialize(
                    typeof(CEngine), xmlReader, XmlEngineBuilder.EngineNamespace);
            }

            if (engineXml == null)
            {
                throw new ArgumentException($"Failed to load engine from url '{url}'");
            }

            if (engineXml.name == null || String.IsNullOrWhiteSpace(engineXml.name.Name))
            {
                throw new ArgumentException($"Engine at url '{url}' has no name");
            }
        }

        public IGraph CreateGraph(string name)
		{
            Guard.AssertNotNullOrWhitespace(name, nameof(name));

            if (!String.Equals(engineXml.name.Name, name))
            {
                throw new ArgumentException($"Unknown graph '{name}'");
            }

            engineNodes.Clear();

            var dagGraph = new DagGraph(name);

            foreach (var node in engineXml.node)
            {
                var nodeName = node.name.Name;
                engineNodes[nodeName] = node;

                var target = new Vertex(dagGraph, nodeName);
                dagGraph.Vertices.Add(target);

                if (node.inputs == null) { continue; }

                foreach (var input in node.inputs)
                {
                    var sourceName = input.@ref.Name;
                    var source = dagGraph.Vertices.First(x => x.Name == sourceName);

                    dagGraph.Edges.Add(new Edge(dagGraph, sourceName, source, target));
                }
            }

            return dagGraph;
		}

		public INode CreateNode(IVertex vertex)
        {
            Guard.AssertNotNull(vertex, "vertex");

            var name = vertex.Name;
            var node = engineNodes[name];
            var type = Type.GetType(node.obj.type, true, true);

            return CreateEngineNode(node, type);
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

        private static INode CreateEngineNode(CNode node, Type type)
        {
            NodeBase engineNode;
            if (node.obj.Any == null)
            {
                engineNode = (NodeBase)Activator.CreateInstance(type);
            }
            else
            {
                engineNode = (NodeBase)XmlHelpers.Deserialize(type, node.obj.Any.OuterXml, NodesNamespace);
            }

            engineNode.Name = node.name.Name;
            return engineNode;
        }
    }
}
