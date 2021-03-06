#region Copyright (c) 2020 Filip Fodemski
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SmartGraph.Common;
using SmartGraph.Core.Interfaces;
using SmartGraph.Dag.Interfaces;

namespace SmartGraph.Core
{
    internal class DefaultEngineTask : IEngineTask
	{
        private readonly EngineCore engine;
        private Stopwatch stopWatch;

        internal DefaultEngineTask(EngineCore engine, EngineNode adaptor)
        {
            Guard.AssertNotNull(engine, "engine");
            Guard.AssertNotNull(adaptor, "adaptor");

            this.engine = engine;
            DirtyNode = adaptor;
        }

        public void StartMeasurementCapture()
        {
            stopWatch = Stopwatch.StartNew();
        }

        public void EndMeasurementCapture()
        {
            stopWatch.Stop();
        }

        public IEngineNode DirtyNode { get; private set; }
        public IList<IVertex> CalculationOrder { get; set; }

		public void Execute()
		{
            if (CalculationOrder == null || CalculationOrder.Count == 0)
            {
                throw new InvalidOperationException("Null or empty calculation order.");
            }

            var updateOrder = new List<string>();

            var calculationList = CalculationOrder
                .Select(v => engine[v.Name])
                .Where(h => !(h.Node is IActiveNode)).ToArray();
            
            foreach (var host in calculationList)
            {
                host.Update();
                updateOrder.Add(host.Node.Name);
            }

			Diagnostics.WriteLine(
				this, string.Format( "updated {0} nodes [{1} -> {2}]",
				updateOrder.Count,
				DirtyNode.Node.Name,
				updateOrder.ToFlatString()));
		}
	}
}