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
    public abstract class SimpleAsyncProducerNodeBase<T> : SimplePipelineComponent<T>
        where T : class
    {
        private Thread thread;

        public SimpleAsyncProducerNodeBase(String name)
            : this(name, new SimpleMessageBus<T>()) { }

        private SimpleAsyncProducerNodeBase(String name, IMessageBus<T> msgBus)
            : base(name)
        {
            MessageBus = new SimpleMessageBus<T>();
        }

        public override void Produce(T message)
        {
            MessageBus.Produce(message);
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
                    message = MessageBus.Consume();
                }
                catch (Exception e)
                {
                    Diagnostics.DebugException(e, "Doing a bus Consume");
                }

                // Produce
                try
                {
                    InternalProduce(message);
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

        protected IMessageBus<T> MessageBus { get; private set; }

        protected abstract void InternalProduce(T message);
    }
}
