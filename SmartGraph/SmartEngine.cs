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

using SmartGraph.Common;
using SmartGraph.Core;
using SmartGraph.Core.Interfaces;
using SmartGraph.Dag.Interfaces;
using System;
using System.Linq;

namespace SmartGraph
{
	public class SmartEngine : IDisposable
	{
        private enum State
        {
            Stopped,
            Started
        }

        private readonly EngineCore core;
		private State state;

        public SmartEngine(string name, IEngineBuilder builder)
            : this(name, builder, new DefaultEnginePipeline()) {}

		public SmartEngine(string name, IEngineBuilder builder, IEnginePipeline pipeline)
		{
            Guard.AssertNotNullOrEmpty(name, "name");
            Guard.AssertNotNull(builder, "builder");
            Guard.AssertNotNull(pipeline, "pipeline");

            var modules = pipeline.Modules;
            if (modules == null || modules.Count == 0)
            {
                throw new ArgumentException("Engine pipeline is empty");
            }

			core = new EngineCore(name, builder, pipeline);
			state = State.Stopped;
		}

        public void Dispose()
        {
            Stop();
            core.Dispose();
        }

		public void Bind()
		{
            if (state != State.Stopped)
            {
                throw new InvalidOperationException("Engine must be stopped");
            }

			core.Bind();
		}

		public void Start()
		{
            if (state != State.Stopped)
            {
                throw new InvalidOperationException("Engine must be stopped");
            }

			core.Start();
			state = State.Started;
		}

		public void Stop()
		{
			core.Stop();
			state = State.Stopped;
		}

		public void Execute(IEngineTask task)
		{
			core.Pipeline.Produce(task);
		}

		public IPublishingPipelineNode Publisher
		{
			get { return core.Pipeline.Nodes.Last() as IPublishingPipelineNode; }
		}

        public IGraph Graph
        {
            get { return core.Graph; }
        }
	}
}
