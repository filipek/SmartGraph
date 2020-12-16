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
using SmartGraph.Common;
using SmartGraph.Core.Interfaces;
using SmartGraph.Pipeline;

// A pipeline is used to bind the various policies (pipeline nodes) together to
// form the SmartGraph engine. Default implementation is provided for each:
// IEventPolicy, ISchedulingPolicy, ICalculationPolicy and IPublishingPolicy.
// Basically the idea is this: an IActiveNode raises the 'dirty node' event which
// is handled by the IEventPolicy. In the default implementation it gets queued
// for scheduling. The ISchedulingPolicy schedules nodes (dependencies etc) for
// calculation. After being re-calculated (updated) nodes are handed to the
// IPublishingPolicy which publishes appropriate events:
// e.g. clean-node or clean-graph.

namespace SmartGraph.Core.Policies
{
    public sealed class DefaultCalculationPolicyNode : SimpleAsyncProducerNodeBase<IEngineTask>, ICalculationPipelineNode
	{
		private IEngine	engine;

        #region AsynchProducerNodeBase override

        protected override void InternalProduce(IEngineTask task)
        {
            try
            {
                task.StartMeasurementCapture();
                task.Execute();
                task.EndMeasurementCapture();
            }
            catch (Exception e)
            {
                Diagnostics.WriteLine(this,
                    string.Format(@"unexpected exception= {0}", e.Message));
            }
        }

        #endregion

		public DefaultCalculationPolicyNode()
            : base(typeof(DefaultCalculationPolicyNode).Name) {}

		public void Bind(IEngine engine)
		{
            Guard.AssertNotNull(engine, "engine");
            this.engine = engine;
		}
	}
}
