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
using SmartGraph.Engine.Common;
using SmartGraph.Engine.Core;
using SmartGraph.Engine.Dag;

namespace SmartGraph.Engine
{
    public class SmartEngine : IDisposable
	{
        private enum State
        {
            Stopped,
            Started
        }

        private readonly EngineCore core;
        private readonly IPublishingPipelineNode publisher;
        private State state;

        public SmartEngine(String name, IEngineBuilder builder)
		{
            Guard.AssertNotNullOrEmpty(name, "name");
            Guard.AssertNotNull(builder, "builder");

            var pipeline = new DefaultEnginePipeline();
            publisher = pipeline.Publisher;

            core = new EngineCore(name, builder, pipeline);
            core.Bind();

            state = State.Stopped;
        }

        public void Dispose()
        {
            Stop();
            core.Dispose();
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

        public event CleanGraphEventHandler CleanGraphEvent
        {
            add { publisher.CleanGraphEvent += value; }
            remove { publisher.CleanGraphEvent -= value; }
        }

        public event CleanNodeEventHandler CleanNodeEvent
        {
            add { publisher.CleanNodeEvent += value; }
            remove { publisher.CleanNodeEvent -= value; }
        }

        public IGraph Graph
        {
            get { return core.Graph; }
        }
    }
}
