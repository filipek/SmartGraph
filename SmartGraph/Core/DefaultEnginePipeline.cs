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

using SmartGraph.Core.Interfaces;
using SmartGraph.Pipeline;
using SmartGraph.Pipeline.Interfaces;
using System.Collections.Generic;
using System.Linq;

// A pipeline is used to bind the various policies (pipeline nodes) together to
// form the SmartGraph engine. Default implementation is provided for each:
// IEventPolicy, ISchedulingPolicy, ICalculationPolicy and IPublishingPolicy.
// Basically the idea is this: an IActiveNode raises the 'dirty node' event which
// is handled by the IEventPolicy. In the default implementation it gets queued
// for scheduling. The ISchedulingPolicy schedules nodes (dependencies etc) for
// calculation. After being re-calculated (updated) nodes are handed to the
// IPublishingPolicy which publishes appropriate events:
// e.g. clean-node or clean-graph.

namespace SmartGraph.Core
{
	public class DefaultEnginePipeline : SimplePipeline<IEngineTask>, IEnginePipeline
	{
        protected readonly IList<IPipelineNode<IEngineTask>> enginePipelineNodes;

        private static IList<IPipelineModule<IEngineTask>> EnginePipeline
        {
            get
            {
                var modules = new List<IPipelineModule<IEngineTask>>
                {
		            new DefaultEventModule(),
			        new DefaultSchedulingModule(),
			        new DefaultCalculationModule(),
			        new DefaultPublishingModule()
                };

                modules[0].Producer.Next = modules[1].Producer;
                modules[1].Producer.Next = modules[2].Producer;
                modules[2].Producer.Next = modules[3].Producer;

                return modules;
            }
        }

        public DefaultEnginePipeline()
            : base("EnginePipeline", EnginePipeline)
		{
            enginePipelineNodes = new List<IPipelineNode<IEngineTask>>()
            {
			    Modules[0].Producer,
			    Modules[1].Producer,
			    Modules[2].Producer,
			    Modules[3].Producer
            };
		}

		public void Bind(IEngine engine)
		{
            foreach (IEnginePipelineNode node in enginePipelineNodes)
            {
                node.Bind(engine);
            }
		}

        public IList<IEnginePipelineNode> Nodes
        {
            get { return enginePipelineNodes.Cast<IEnginePipelineNode>().ToList(); }
        }
	}
}
