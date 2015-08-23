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

namespace SmartGraph.Engine.Dag
{
	#region Directed Acyclic Graph
	public abstract class DAGObject : IDAGObject
	{
		protected String name;
		protected IGraph owner;

		protected DAGObject(IGraph owner, String name)
		{
            Guard.AssertNotNullOrEmpty(name, "name");

			this.owner = owner;
            this.name = name;
        }
		
		#region IDAGObject Members

		public virtual String Name { get { return name; } }

		public virtual IGraph Owner { get { return owner; } }

		#endregion
	}
	public class Edge: DAGObject, IEdge
	{
		protected IVertex source;
		protected IVertex target;

		public Edge(IGraph owner, String name, IVertex source, IVertex target)
            : base(owner, name)
		{
            Guard.AssertNotNull(source, "source");
            Guard.AssertNotNull(target, "target");

			this.source = source;
            this.target = target;

            source.OutEdges.Add(this);
            target.InEdges.Add(this);
		}

        public Edge(IGraph owner, String name, String source, String target)
            : base(owner, name)
        {
            Guard.AssertNotNullOrEmpty(source, "source");
            Guard.AssertNotNullOrEmpty(target, "target");

            var sourceEdge = owner.Vertices.First(x => x.Name == source);
            this.source = sourceEdge;
            var targetEdge = owner.Vertices.First(x => x.Name == target);
            this.target = targetEdge;

            sourceEdge.OutEdges.Add(this);
            targetEdge.InEdges.Add(this);
        }

		#region IEdge Members

		public virtual IVertex Source 
		{
			get { return source; }
		}

		public virtual IVertex Target 
		{
			get { return target; }
		}

		#endregion
	}
	public class Vertex: DAGObject, IVertex
	{
		public Vertex(IGraph o, String n) : base(o, n)
        {
            InEdges = new HashSet<IEdge>();
            OutEdges = new HashSet<IEdge>();
        }

		#region IVertex Members

        public virtual ISet<IEdge> InEdges { get; private set; }

        public virtual ISet<IEdge> OutEdges { get; private set; }

		#endregion
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

		#region IGraph Members

        public virtual ISet<IVertex> Vertices { get; private set; }

        public virtual ISet<IEdge> Edges { get; private set; }

		#endregion
	}
	#endregion
}
