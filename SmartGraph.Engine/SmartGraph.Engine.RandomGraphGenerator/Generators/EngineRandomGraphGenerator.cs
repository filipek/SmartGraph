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

using SmartGraph.Engine.Dag;
using SmartGraph.Engine.Dag.Interfaces;
using System;
using System.Linq;

namespace SmartGraph.Engine.RandomGraphGenerator.Generators
{
    internal class EngineRandomGraphGenerator
    {
        private readonly RandomGraphGenerator generator;

        public EngineRandomGraphGenerator()
        {
            generator = new RandomGraphGenerator();
        }

        public IGraph Generate(String graphName)
        {
            DagGraph graph = new DagGraph(graphName);

            Action endGraph = () =>
            {
                foreach (var v in graph.Vertices.ToArray())
                {
                    if (!v.InEdges.Any() && !v.OutEdges.Any())
                    {
                        graph.Vertices.Remove(v);
                        Console.WriteLine("Removed disconnected node: {0}", v.Name);
                    }
                    else if (!v.InEdges.Any())
                    {
                        Console.WriteLine("Active node: {0}", v.Name);
                    }
                    else if (!v.OutEdges.Any())
                    {
                        Console.WriteLine("Publisher node: {0}", v.Name);
                    }
                }
            };

            Action<int> createNode = (n) => graph.Vertices.Add(new Vertex(graph, n.ToString()));
            Action<int, int> createEdge = (s, t) => graph.Edges.Add(
                new Edge(graph, String.Format("{0}-{1}", s, t), s.ToString(), t.ToString()));

            generator.Generate(null, endGraph, createEdge, createNode);

            return graph;
        }
    }
}
