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
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartGraph.Engine.Dag
{
    #region Directed Acyclic Graph

    public abstract class DAGObject : IGraphObject
	{
		protected DAGObject(IGraph owner, String name)
		{
            Guard.AssertNotNullOrEmpty(name, nameof(name));

			Owner = owner;
            Name = name;
        }

        public String Name { get; } = null;

        public IGraph Owner { get; } = null;
	}
	public class Edge: DAGObject, IEdge
	{
		public Edge(IGraph owner, String name, IVertex source, IVertex target)
            : base(owner, name)
		{
            Guard.AssertNotNull(source, "source");
            Guard.AssertNotNull(target, "target");

			Source = source;
            Target = target;

            source.OutEdges.Add(this);
            target.InEdges.Add(this);
		}

        public Edge(IGraph owner, String name, String source, String target)
            : base(owner, name)
        {
            Guard.AssertNotNullOrEmpty(source, "source");
            Guard.AssertNotNullOrEmpty(target, "target");

            var sourceEdge = owner.Vertices.First(x => x.Name == source);
            Source = sourceEdge;
            var targetEdge = owner.Vertices.First(x => x.Name == target);
            Target = targetEdge;

            sourceEdge.OutEdges.Add(this);
            targetEdge.InEdges.Add(this);
        }

        public IVertex Source { get; } = null;

        public IVertex Target { get; } = null;
    }

    public class Vertex: DAGObject, IVertex
	{
		public Vertex(IGraph o, String n) : base(o, n)
        {
            InEdges = new HashSet<IEdge>();
            OutEdges = new HashSet<IEdge>();
        }

        public ISet<IEdge> InEdges { get; } = null;

        public ISet<IEdge> OutEdges { get; } = null;
    }

	public class DagGraph : DAGObject, IGraph
	{
        public DagGraph(String name) : this(name, null) { }

        public DagGraph(String name, IGraph owner)
            : base(owner, name)
        {
            Edges = new HashSet<IEdge>();
            Vertices = new HashSet<IVertex>();
        }

        public ISet<IVertex> Vertices { get; } = null;

        public ISet<IEdge> Edges { get; } = null;
	}

    #endregion
}
