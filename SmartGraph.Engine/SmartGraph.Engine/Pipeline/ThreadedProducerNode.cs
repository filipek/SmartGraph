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
using System.Threading;
using SmartGraph.Engine.Common;
using SmartGraph.Engine.Pipeline.Interfaces;

namespace SmartGraph.Engine.Pipeline
{
    public abstract class ThreadedProducerNode<T> : SimplePipelineNode<T>
        where T : class
    {
        private readonly IMessageBus<T> messageBus;
        private Action<T> threadedAction;
        private Thread thread;

        public ThreadedProducerNode(String name)
            : base(name)
        {
            Guard.AssertNotNullOrEmpty(name, nameof(name));

            messageBus = new MessageBus<T>();
        }

        protected void SetAction(Action<T> threadedAction)
        {
            this.threadedAction = threadedAction;
        }

        public override void Produce(T message)
        {
            messageBus.Produce(message);
        }

        public virtual void Start()
        {
            thread = new Thread(new ThreadStart(ThreadFunc));
            thread.Start();
        }

        public virtual void Stop()
        {
            if (thread != null)
            {
                thread.Interrupt();
                thread.Join();
                thread = null;
            }
        }

        private void ThreadFunc()
        {
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
                    Diagnostics.DebugException(e, "Doing a bus Consume");
                }

                // Produce
                try
                {
                    threadedAction(message);
                }
                catch (Exception e)
                {
                    Diagnostics.DebugException(e, "Processing");
                }

                // SendNext
                try
                {
                    SendNext(message);
                }
                catch (Exception e)
                {
                    Diagnostics.DebugException(e, "Doing a SendNext");
                }
            } while (true);
        }
    }
}
