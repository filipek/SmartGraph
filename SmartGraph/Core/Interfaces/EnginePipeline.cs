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

using SmartGraph.Pipeline.Interfaces;
using System.Collections.Generic;

namespace SmartGraph.Core.Interfaces
{
    public interface IEnginePipeline : IPipeline<IEngineTask>
	{
		IList<IEnginePipelineNode> Nodes { get; }

		void Bind(IEngine engine);
	}

    public interface IEnginePipelineNode : IPipelineNode<IEngineTask>
	{
		void Bind(IEngine engine);
		void Start();
		void Stop();
	}

	public interface IEventPipelineNode : IEnginePipelineNode {}

	public interface ISchedulingPipelineNode : IEnginePipelineNode {}

	public interface ICalculationPipelineNode : IEnginePipelineNode {}

    public delegate void CleanNodeEventHandler(IEngine engine, INode node);
    public delegate void CleanGraphEventHandler(IEngine engine);

	public interface IPublishingPipelineNode : IEnginePipelineNode
	{
		event CleanGraphEventHandler CleanGraphEvent;
		event CleanNodeEventHandler CleanNodeEvent;
	}
}
