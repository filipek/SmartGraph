﻿#region Copyright (C) 2015 Filip Fodemski
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
using SmartGraph.Engine.Pipeline.Interfaces;

namespace SmartGraph.Engine.Pipeline
{
    public abstract class SimplePipelineComponent<T> : MarshalByRefObject, IPipelineNode<T>
    {
        private IPipelineNode<T> moduleSibling;
        private IPipelineNode<T> next;

        protected virtual void SendNext(T msg)
        {
            if (next != null)
            {
                next.Produce(msg);
            }
        }

        protected SimplePipelineComponent(String name)
        {
            Name = name;
        }

        public abstract void Produce(T message);

        public virtual T Consume()
        {
            throw new NotImplementedException();
        }

        public IPipelineNode<T> ModuleSibling
        {
            get { return moduleSibling; }
            set { moduleSibling = value; }
        }

        public String Name { get; private set; }

        public virtual IPipelineNode<T> Next
        {
            get { return next; }
            set { next = value; }
        }
    }
}