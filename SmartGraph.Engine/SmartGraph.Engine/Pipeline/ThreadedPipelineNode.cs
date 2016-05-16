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
using System.Threading.Tasks;
using SmartGraph.Engine.Common;
using SmartGraph.Engine.Pipeline.Interfaces;

namespace SmartGraph.Engine.Pipeline
{
    public class ThreadedPipelineNode<T> : PipelineNode<T>
        where T : class
    {
        private readonly IMessageBus<T> messageBus = new MessageBus<T>();
        private Task task;

        private void InternalTaskAction()
        {
            Diagnostics.WriteLine(this, $"Starting task");

            do
            {
                // Consume message
                var message = default(T);

                try
                {
                    message = messageBus.Consume();
                }
                catch (Exception e)
                {
                    Diagnostics.DebugException(e, "Consuming task");
                }

                if (TaskAction != null)
                {
                    try
                    {
                        TaskAction(message);
                    }
                    catch (Exception e)
                    {
                        Diagnostics.DebugException(e, "Processing task");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Cannot process task - TaskAction is not set.");
                }

                // SendNext
                try
                {
                    SendNext(message);
                }
                catch (Exception e)
                {
                    Diagnostics.DebugException(e, "Sending task to next");
                }
            } while (true);
        }

        public ThreadedPipelineNode(String name)
            : base(name)
        {
            Guard.AssertNotNullOrEmpty(name, nameof(name));
        }

        public override void Produce(T message)
        {
            messageBus.Produce(message);
        }

        public void Start()
        {
            task = Task.Factory.StartNew(InternalTaskAction);
        }

        public void Stop()
        {
            if (task == null) { return; }

            task.Wait();
            task = null;
        }

        public Action<T> TaskAction { get; set; }
    }
}
