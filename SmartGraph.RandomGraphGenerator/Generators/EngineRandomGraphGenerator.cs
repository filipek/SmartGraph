﻿#region Copyright (c) 2020 Filip Fodemski
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

using SmartGraph.Dag;
using SmartGraph.Dag.Interfaces;
using System;
using System.Linq;

namespace SmartGraph.RandomGraphGenerator.Generators
{
    internal class EngineRandomGraphGenerator
    {
        private readonly RandomGraphGenerator generator;

        public EngineRandomGraphGenerator()
        {
            generator = new RandomGraphGenerator();
        }

        public IGraph Generate(string graphName)
        {
            var graph = new DagGraph(graphName);
            var vertices = graph.Vertices.ToArray();

            Action endGraph = () =>
            {
                foreach (var v in vertices)
                {
                    var hasInEdges = v.InEdges.None();
                    var hasOutEdges = v.OutEdges.None();

                    if (hasInEdges && hasOutEdges)
                    {
                        graph.Vertices.Remove(v);
                        Console.WriteLine("Removed disconnected node: {0}", v.Name);
                    }
                    else if (hasInEdges)
                    {
                        Console.WriteLine("Active node: {0}", v.Name);
                    }
                    else if (hasOutEdges)
                    {
                        Console.WriteLine("Publisher node: {0}", v.Name);
                    }
                }
            };

            Action<int> createNode =
                v => graph.Vertices.Add(new Vertex(graph, v.ToString()));
            Action<int, int> createEdge =
                (s, t) => graph.Edges.Add(new Edge(graph, string.Format("{0}-{1}", s, t), s.ToString(), t.ToString()));

            generator.Generate(null, endGraph, createEdge, createNode);

            return graph;
        }
    }
}
