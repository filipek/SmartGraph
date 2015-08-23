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
using System.Collections.Generic;

namespace SmartGraph.Engine.Pipeline.Interfaces
{
	// This set of interfaces provides a framework for
	// managing pipelines of processing. A IPipeline
	// contains an ordered IList of IPipelineModule's.
	// Each pipeline module contains two processing tasks i.e. 
	// producer and consumer.
	// This design was inspired by the ACE Toolkit, read more at
	// http://www.cs.wustl.edu/~schmidt/PDF/C++-USENIX-94.pdf

	/// <summary>
	/// A message consumer.
	/// </summary>
	public interface IMessageConsumer<T>
	{
		/// <summary>
		/// Receive a message - blocking call.
		/// </summary>
		/// <param name="message">Message returned by ref</param>
        T Consume();
	}

	/// <summary>
	/// A message producer.
	/// </summary>
    public interface IMessageProducer<T>
	{
		/// <summary>
		/// Send object as message.
		/// </summary>
		/// <param name="message">Message to send</param>
        void Produce(T message);
	}

	/// <summary>
	/// Message Bus - a consumer and producer of messages.
	/// </summary>
    public interface IMessageBus<T> : IMessageProducer<T>, IMessageConsumer<T> { }

	/// <summary>
	/// Can be used as an element in an IPipeline
	/// </summary>
    public interface IPipelineComponent<T> : IMessageBus<T>
	{
		/// <summary>
		/// Next item in an IPipeline
		/// </summary>
        IPipelineComponent<T> Next { get; set; }
	}

	/// <summary>
	/// A pipeline node can be a publisher
	/// (called on its Send methods) or
	/// a consumer (called on its Receive methods).
	/// The IPipelineComponent.Next property returns the next
	/// IPipelineNode in the pipeline.
	/// </summary>
    public interface IPipelineNode<T> : IPipelineComponent<T>
	{
		/// <summary>
		/// Returns this node's sibling in an IPipelineModule.
		/// </summary>
        IPipelineNode<T> ModuleSibling { get; }
	}

	/// <summary>
	/// This is a pipeline processing layer. Application functionality
	/// is defined in here. IPipelineModule must forward its implementation
	/// of IMessageBus to Consumer (read Receive side) and Producer
	/// (Send side). Uni-directional pipelines will have one of their
	/// Consumer or Publisher node instances undefined (i.e. null).
	/// The IPipelineComponent.Next property returns the next
	/// IPipelineModule in the pipeline.
	/// </summary>
    public interface IPipelineModule<T> : IPipelineComponent<T>
	{
		/// <summary>
		/// The IMessageBus side of an IPipelineModule will forward
		/// its Receive method calls to the node returned by this property.
		/// </summary>
        IPipelineNode<T> Consumer { get; }

		/// <summary>
		/// The IMessageBus side of an IPipelineModule will forward
		/// its Send method calls to the node returned by this property.
		/// </summary>
        IPipelineNode<T> Producer { get; }
	}

	/// <summary>
	/// IPipeline must forward its implementation
	/// of IMessageBus to the first (0th) item of the
	/// IList returned from Modules, for both IMessageConsumer
	/// and IMessageProducer. The IPipelineComponent.Next property
	/// returns the next IPipeline in the pipeline.
	/// </summary>
    public interface IPipeline<T> : IPipelineComponent<T>
	{
		/// <summary>
		/// Ordered collection of IPipelineModule's
		/// </summary>
        IList<IPipelineModule<T>> Modules { get; }
	}
}
