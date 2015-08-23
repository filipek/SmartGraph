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
using SmartGraph.Engine.Dag.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartGraph.Engine.Dag.Algorithms
{
    public interface IAlgorithm
    {
        void Run();

        void Reset();
    }

    public interface IDFSAlgorithm
    {
        IList<IVertex> VisitOrder { get; }
    }

    public class EquivalenceClasses : IAlgorithm
    {
        private int[] dad;
        private IGraph graph;
        private IDictionary<IVertex, int> visitOrder;
        private bool hasRun;

#if false

      // This code has been left here for illustration
      // purposes - its easier to understand what this algorithm
      // is doing, but the actual implementation used is optimized.
      private bool Find( IVertex vx, IVertex vy, bool doit)
      {
         int i = (int) visitOrder[vx];
         int j = (int) visitOrder[vy];
         
         while ( dad[i] > 0 ) i = dad[i];
         while ( dad[j] > 0 ) j = dad[j];

         if ( doit && (i != j) ) dad[j] = i;

         return (i != j);
      }

#else

        private bool Find(IVertex vx, IVertex vy, bool doit)
        {
            int x = (int)visitOrder[vx];
            int y = (int)visitOrder[vy];

            int i = x;
            int j = y;

            while (dad[i] > 0) i = dad[i];
            while (dad[j] > 0) j = dad[j];

            int t = 0;
            while (dad[x] > 0)
            {
                t = x;
                x = dad[x];
                dad[t] = i;
            }

            while (dad[y] > 0)
            {
                t = y;
                y = dad[y];
                dad[t] = j;
            }

            if (doit && (i != j))
            {
                if (dad[j] < dad[i])
                {
                    dad[j] += dad[i] - 1;
                    dad[i] = j;
                }
                else
                {
                    dad[i] += dad[j] - 1;
                    dad[j] = i;
                }
            }

            return (i != j);
        }

#endif

        public EquivalenceClasses(IGraph graph)
        {
            Guard.AssertNotNull(graph, "graph");

            if (graph.Vertices.Count == 0)
            {
                throw new ArgumentException("Graph has no vertices");
            }

            this.graph = graph;

            Reset();
        }


        public void Run()
        {
            if (!hasRun)
            {
                foreach (var edge in graph.Edges)
                {
                    // NOTE: the inversion of source -> target direction
                    Find(edge.Target, edge.Source, true);
                }

                hasRun = true;
            }
        }

        public void Reset()
        {
            hasRun = false;

            var vertexCount = graph.Vertices.Count;
            dad = new int[vertexCount];
            visitOrder = new Dictionary<IVertex, int>(vertexCount);

            for (int i = 0; i < vertexCount; ++i)
            {
                dad[i] = 0;
            }

            int vi = 0;
            foreach (var v in graph.Vertices)
            {
                visitOrder[v] = vi++;
            }
        }

        public bool IsEquivalent(IVertex x, IVertex y)
        {
            return x == y || !Find(x, y, false);
        }

        /// <summary>
        /// Return an IList of IList's. Each list corresponds to an equivalence
        /// class and contains all the vertices belonging to the equivalence class.
        /// </summary>
        public IList<IList<IVertex>> GetClasses()
        {
            Run();

            var graphVertices = graph.Vertices;

            var eqmap = new List<IList<IVertex>>();

            foreach (var x in graphVertices)
            {
                foreach (var y in graphVertices)
                {
                    if (IsEquivalent(x, y))
                    {
                        bool found = false;

                        foreach (var eqlist in eqmap)
                        {
                            IVertex first = eqlist[0];

                            if (IsEquivalent(x, first))
                            {
                                found = true;

                                if (!eqlist.Contains(x))
                                {
                                    eqlist.Add(x);
                                }

                                if (!eqlist.Contains(y))
                                {
                                    eqlist.Add(y);
                                }
                            }
                        }

                        if (!found)
                        {
                            var eqlist = new List<IVertex>();

                            eqlist.Add(x);

                            if (x != y)
                            {
                                eqlist.Add(y);
                            }

                            eqmap.Add(eqlist);
                        }
                    }
                }
            }

            return eqmap;
        }
    }

    public abstract class DFSVisitor : IDFSAlgorithm
    {
        protected enum Colour { White, Gray, Black }

        protected IGraph graph;
        protected IVertex startVertex;
        protected IDictionary<IVertex, Colour> colourMap = new Dictionary<IVertex, Colour>();
        protected IList<IVertex> visitOrder = new List<IVertex>();
        protected bool visitWholeGraph;
        protected bool hasRun;

        protected DFSVisitor(IVertex start)
        {
            Guard.AssertNotNull(start, "start");
            Guard.AssertNotNull(start.Owner, "start.Owner");

            graph = start.Owner;
            startVertex = start;

            Reset();
        }

        protected DFSVisitor(IGraph graph)
        {
            Guard.AssertNotNull(graph, "graph");
            Guard.AssertNotNull(graph.Vertices, "graph.Vertices");

            if (graph.Vertices.Count == 0)
            {
                throw new ArgumentException("The graph has no vertices");
            }

            this.graph = graph;
            visitWholeGraph = true;

            Reset();
        }

        #region DFS Implementation

        private void RunDFS()
        {
            var graphVertices = graph.Vertices;

            var first = graphVertices.First();

            if (visitWholeGraph)
            {
                startVertex = first;
            }

            if (startVertex != null)
            {
                StartVertex(startVertex);
                Visit(startVertex);
            }

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

        #region IAlgorithm Implementation

        public virtual void Run()
        {
            if (!hasRun)
            {
                RunDFS();

                hasRun = true;
            }
        }

        public virtual void Reset()
        {
            hasRun = false;
            visitOrder.Clear();

            graph.Vertices.ForEach(v => colourMap[v] = Colour.White);
        }

        #endregion

        public virtual IList<IVertex> VisitOrder
        {
            get
            {
                Run();
                return visitOrder;
            }
        }

        #region DFSVisitor Default Implementation

        protected virtual void BackEdge(IEdge edge)
        {
            throw new InvalidOperationException("Cycle detected on edge " + edge.Name);
        }

        protected virtual void StartVertex(IVertex v) { }

        protected virtual void DiscoverVertex(IVertex v) { }

        protected virtual void FinishVertex(IVertex v) { }

        protected virtual void ExamineEdge(IEdge e) { }

        protected virtual void TreeEdge(IEdge e) { }

        protected virtual void ForwardOrCrossEdge(IEdge e) { }

        #endregion
    }

    public class DepthFirstSearch : DFSVisitor
    {
        public DepthFirstSearch(IVertex start) : base(start) { }

        public DepthFirstSearch(IGraph graph) : base(graph) { }

        protected override void BackEdge(IEdge edge)
        {
            throw new InvalidOperationException("Cycle detected on edge " + edge.Name);
        }

        protected override void FinishVertex(IVertex v)
        {
            visitOrder.Add(v);
        }
    }

    public class TopologicalSort : DFSVisitor
    {
        public TopologicalSort(IVertex start) : base(start) { }

        public TopologicalSort(IGraph graph) : base(graph) { }

        protected override void BackEdge(IEdge edge)
        {
            throw new InvalidOperationException("Cycle detected on edge " + edge.Name);
        }

        protected override void FinishVertex(IVertex v)
        {
            visitOrder.Insert(0, v);
        }
    }
}
