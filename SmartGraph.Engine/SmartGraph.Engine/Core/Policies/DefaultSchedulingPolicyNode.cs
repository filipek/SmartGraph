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
using SmartGraph.Engine.Dag.Algorithms;
using SmartGraph.Engine.Dag;
using SmartGraph.Engine.Pipeline;
using SmartGraph.Engine.Pipeline.Interfaces;
using System;
using System.Collections.Generic;

// A pipeline is used to bind the various policies (pipeline nodes) together to
// form the SmartGraph engine. Default implementation is provided for each:
// IEventPolicy, ISchedulingPolicy, ICalculationPolicy and IPublishingPolicy.
// Basically the idea is this: an IActiveNode raises the 'dirty node' event which
// is handled by the IEventPolicy. In the default implementation it gets queued
// for scheduling. The ISchedulingPolicy schedules nodes (dependencies etc) for
// calculation. After being re-calculated (updated) nodes are handed to the
// IPublishingPolicy which publishes appropriate events:
// e.g. clean-node or clean-graph.

namespace SmartGraph.Engine.Core.Policies
{
    public sealed class DefaultSchedulingPolicyNode : SimplePipelineComponentBase<IEngineTask>, ISchedulingPipelineNode
	{
        private class TopSortMap : Dictionary<String, IList<IVertex>>
        {
            public TopSortMap(int capacity) : base(capacity) {}
        }

        private bool isInitialized = false;
		private IEngine engine;
        private TopSortMap topsortMap;

        public DefaultSchedulingPolicyNode()
            : base(typeof(DefaultSchedulingPolicyNode).Name) { }

        public void Start() { }

        public void Stop() { }

    	public void Bind(IEngine engine)
		{
            Guard.AssertNotNull(engine, "engine");
            this.engine = engine;

			var graph = engine.Graph;
            var graphVertices = graph.Vertices;

            topsortMap = new TopSortMap(graphVertices.Count);
            foreach (var vertex in graphVertices)
			{
				var host = engine[vertex.Name];

                // We only produce a top sort for nodes which can be originators 
                // of change ie IActiveNodes. All other nodes will take part
				// in top sorts, but they will not be first in the visit order.
				if ( ! (host.Node is IActiveNode) )
					continue;

				var vertexSort = new TopologicalSortAlgorithm(vertex);
				vertexSort.Run();

                var topsort = new List<IVertex>();
				foreach (var dependentVertex in vertexSort.VisitOrder)
				{
					var dependentHost = engine[dependentVertex.Name];
                    var dependentNode = dependentHost.Node;

					// We will only ever need to update/calculate nodes which
					// can change *and* are not potential originators of change.
                    if (!(dependentNode is IActiveNode))
					{
						// The InvariantNodeAttribute can be used to filter nodes out
						// of the update list here.
						var invAttrs = dependentNode.GetType().GetCustomAttributes(
                            typeof(InvariantNodeAttribute), false);

						if ( invAttrs == null || invAttrs.Length == 0 )
							topsort.Add( dependentVertex );
					}
				}

				topsortMap[vertex.Name] = topsort;
			}
		}

        public override void Produce(IEngineTask task)
        {
            Guard.AssertNotNull(task, "task");

            var dirtyNode = task.DirtyNode;

            Diagnostics.WriteLine(this,
                String.Format("scheduled task for node= {0}", dirtyNode.Node.Name));

            if (isInitialized)
            {
                task.CalculationOrder = topsortMap[dirtyNode.Vertex.Name];
            }
            else
            {
                // First time around we need to calculate (Update) all nodes
                // so here we sort the whole graph and use the visit order to
                // force calculation of everything.

                var graphSort = new TopologicalSortAlgorithm(engine.Graph);
                graphSort.Run();

                task.CalculationOrder = graphSort.VisitOrder;

                isInitialized = true;
            }

            EngineCounters.RemoveDirtyNodeEvent();

            SendNext(task);
        }
	}
}
