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
using SmartGraph.Engine.Common;

namespace SmartGraph.Engine.Dag.Algorithms
{

    public abstract class DepthFirstSearchVisitor : IDepthFirstSearchAlgorithm
    {
        protected enum Colour { White, Gray, Black }

        protected readonly IGraph graph;
        protected readonly IVertex startVertex;
        protected readonly IDictionary<IVertex, Colour> colourMap = new Dictionary<IVertex, Colour>();
        protected readonly IList<IVertex> visitOrder = new List<IVertex>();
        protected readonly bool visitWholeGraph;
        protected bool hasRun;

        protected DepthFirstSearchVisitor(IVertex start)
        {
            Guard.AssertNotNull(start, nameof(start));
            Guard.AssertNotNull(start.Owner, nameof(start.Owner));

            graph = start.Owner;
            startVertex = start;

            Reset();
        }

        protected DepthFirstSearchVisitor(IGraph graph)
        {
            Guard.AssertNotNull(graph, nameof(graph));
            Guard.AssertNotNull(graph.Vertices, nameof(graph.Vertices));

            if (graph.Vertices.Count == 0)
            {
                throw new ArgumentException("The graph has no vertices");
            }

            this.graph = graph;
            visitWholeGraph = true;

            Reset();
        }

        public void Run()
        {
            if (!hasRun)
            {
                RunDepthFirstSearch();

                hasRun = true;
            }
        }

        public void Reset()
        {
            hasRun = false;
            visitOrder.Clear();

            graph.Vertices.ForEach(v => colourMap[v] = Colour.White);
        }

        public IList<IVertex> VisitOrder
        {
            get
            {
                Run();
                return visitOrder;
            }
        }

        #region Default Visitor Implementation

        protected virtual void BackEdge(IEdge edge)
        {
            throw new CycleDetectedException(String.Format("Cycle detected on edge '{edge.Name}'"));
        }

        protected virtual void StartVertex(IVertex v) { }

        protected virtual void DiscoverVertex(IVertex v) { }

        protected virtual void FinishVertex(IVertex v) { }

        protected virtual void ExamineEdge(IEdge e) { }

        protected virtual void TreeEdge(IEdge e) { }

        protected virtual void ForwardOrCrossEdge(IEdge e) { }

        #endregion

        #region Depth First Search

        private void RunDepthFirstSearch()
        {
            var graphVertices = graph.Vertices;
            var start = visitWholeGraph ? graphVertices.FirstOrDefault() : startVertex;
            if (start == null) { return; }

            StartVertex(start);
            Visit(start);

            if (visitWholeGraph)
            {
                graphVertices
                    .Where(v => Colour.White == colourMap[v])
                    .ForEach(v => Visit(v));
            }
        }

        private void Visit(IVertex v)
        {
            colourMap[v] = Colour.Gray;
            DiscoverVertex(v);

            if (v.OutEdges.Count > 0)
            {
                foreach (var e in v.OutEdges)
                {
                    var t = e.Target;

                    ExamineEdge(e);

                    switch (colourMap[t])
                    {
                        case Colour.White:
                            TreeEdge(e);
                            Visit(t);
                            break;

                        case Colour.Gray:
                            BackEdge(e);
                            break;

                        case Colour.Black:
                            ForwardOrCrossEdge(e);
                            break;
                    }
                }
            }

            colourMap[v] = Colour.Black;
            FinishVertex(v);
        }

        #endregion
    }
}
