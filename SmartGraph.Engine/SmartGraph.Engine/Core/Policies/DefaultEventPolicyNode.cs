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

using SmartGraph.Engine.Core.Interfaces;
using SmartGraph.Engine.Pipeline;
using SmartGraph.Engine.Pipeline.Interfaces;

namespace SmartGraph.Engine.Core.Policies
{
    /// <summary>
    /// Represents the entry point to the Engine processing pipeline. It does not
    /// do any processing on its own and forwards all the messages it receives to
    /// the next (scheduling) component.
    /// </summary>
    public class DefaultEventPolicyNode : SimplePipelineComponentBase<IEngineTask>, IEventPipelineNode
	{
        private IEngine engine;

        public DefaultEventPolicyNode() : base(typeof(DefaultEventPolicyNode).Name) { }

		public void Bind(IEngine engineCore)
		{
			engine = engineCore;
		}

		public void Start() {}

		public void Stop() {}

        public override void Produce(IEngineTask message)
		{
            EngineCounters.AddDirtyNodeEvent();
			SendNext(message);
		}
	}
}
